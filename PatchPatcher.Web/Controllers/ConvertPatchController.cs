using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace PatchPatcher.Web.Controllers
{
    public class ConvertPatchController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Convert(string path)
        {
            // TODO: Refactor to .NET 4.0 HttpClient
            string originUrl = "http://github.com/" + path;
            string originPatchUrl = originUrl + ".patch";

            WebRequest webRequest = WebRequest.Create(originPatchUrl);
            HttpWebResponse webResponse = null;
            try
            {
                webResponse = (HttpWebResponse) webRequest.GetResponse();
                string patchContents;
                using (var content = new StreamReader(webResponse.GetResponseStream()))
                {
                    patchContents = content.ReadToEnd();
                }

                var lines = new[]
                                {
                                    "Online Diff: " + originUrl,
                                    "Location: " + Request.Url
                                };
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

                return Content(patchContents, "text/plain");
            }
            catch (WebException)
            {
                return View("NotFound");
            }
            finally
            {
                if (webResponse != null) webResponse.Close();
            }
        }
    }
}