using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using PatchPatcher.Github;

namespace PatchPatcher.Web.Controllers
{
    public class ConvertPatchController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AnalyzePermaLink(string path)
        {
            ViewData["path"] = "http://github.com/" + path;
            return View("Index", "Site");
        }

        [HttpPost]
        public ActionResult Analyze(string path)
        {
            var result = urlAnalyzer().Analyze(new Uri(path));
            return Json(new { success = true, result }, JsonRequestBehavior.AllowGet);
        }

        private Uri absolutePath()
        {
            var host = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host +
                       (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);

            return new Uri(new Uri(host), Url.Content("~"));
        }

        public ActionResult DownloadPatch(string path)
        {
            try
            {
                UrlAnalyzeResult result;
                string patchContents = createPatch(path, out result);

                return File(Encoding.UTF8.GetBytes(patchContents), "text/plain", result.CommitOrBranch + ".patch");
            }
            catch (WebException)
            {
                return View("NotFound");
            }
        }

        public ActionResult ViewPatch(string path)
        {
            try
            {
                UrlAnalyzeResult result;
                string patchContents = createPatch(path, out result);

                return Content(patchContents, "text/plain");
            }
            catch (WebException)
            {
                return View("NotFound");
            }
        }

        private string createPatch(string path, out UrlAnalyzeResult analyzeResult)
        {
            string originUrl = "http://github.com/" + path;
            analyzeResult = urlAnalyzer().Analyze(new Uri(originUrl));

            string originDiffUrl = analyzeResult.CompareUrl + ".diff";

            string patchContents;
            patchContents = new WebClient().DownloadString(originDiffUrl);

            var lines = new[]
                            {
                                "Online: " + analyzeResult.PermaLink,
                                "Download: " + analyzeResult.DownloadUrl,
                                "View: " + analyzeResult.ViewUrl,
                                "Based on r" + analyzeResult.SvnRevision + " in " + analyzeResult.SvnUrl,
                                "",
                                "Included commits:"
                            };

            var changes =
                analyzeResult.Changes.Select(
                    c => string.Format(" - {0} (by {1} on {2})", c.message, c.author.name + "<" + c.author.email + ">", c.authored_date))
                    .SelectMany(m => m.Replace("\n", "\n   ").Replace("\r", "").Split('\n'));

            lines = lines
                .Union(changes)
                .ToArray();

            var prependInfoHeader = new PrependInfoHeader(lines);
            IEnumerable<IConversionStep> steps = new IConversionStep[]
                                                     {
                                                         prependInfoHeader,
                                                         new RemoveLeadingFilePathStep(),
                                                         new InsertTortoiseSegmentationLineStep(),
                                                         new CorrectNullReferencesStep()
                                                     };

            patchContents = steps.Aggregate(patchContents,
                                            (current, conversionStep) => conversionStep.Convert(current));
            return patchContents;
        }

        private UrlAnalyzer urlAnalyzer()
        {
            return new UrlAnalyzer(new GithubApi(), new UrlBuilder(absolutePath()));
        }
    }
}