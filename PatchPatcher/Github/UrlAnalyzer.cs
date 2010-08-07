using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PatchPatcher.Github
{
    public class UrlAnalyzer
    {
        private readonly IGithubApi _githubApi;
        private readonly IUrlBuilder _urlBuilder;

        readonly Regex _gitSvnPart = new Regex(@"git-svn-id: (?<url>.+?)@(?<rev>\d+) [a-fA-F0-9\-]+$");

        public UrlAnalyzer(IGithubApi githubApi)
            : this(githubApi, null)
        {
        }

        public UrlAnalyzer(IGithubApi githubApi, IUrlBuilder urlBuilder)
        {
            _githubApi = githubApi;
            _urlBuilder = urlBuilder;
        }

        public UrlAnalyzeResult Analyze(Uri uri)
        {
            string[] segments = uri.Segments
                .Select(s => s.Replace("/", ""))
                .Where(s2 => !String.IsNullOrEmpty(s2))
                .ToArray();

            var res = new UrlAnalyzeResult
                                       {
                                           Path = uri.ToString(),
                                           IsValid = true,
                                           IsBranch = segments[2] == "tree",
                                           IsCommit = segments[2] == "commit",
                                           User = segments[0],
                                           Repository = segments[1],
                                           CommitOrBranch = segments[3]
                                       };

            if (res.IsBranch)
            {
                var commits = _githubApi.CommitsInBranch(res.User, res.Repository, res.CommitOrBranch);
                res.HeadCommit = commits.FirstOrDefault();
                res.LastSvnCommit = commits.FirstOrDefault(c => c.message != null && _gitSvnPart.IsMatch(c.message));

                if (res.LastSvnCommit != null)
                {
                    res.Changes = commits.TakeWhile(c => c != res.LastSvnCommit).ToArray();

                    // blabla git-svn-id: url@rev uuid
                    string message = res.LastSvnCommit.message;

                    var groups = _gitSvnPart.Match(message).Groups;
                    res.SvnUrl = groups["url"].Value;
                    res.SvnRevision = int.Parse(groups["rev"].Value);

                    res.CompareUrl = _githubApi.BuildCompareUrl(res.User, res.Repository, res.HeadCommit, res.LastSvnCommit);
                }

                if (_urlBuilder != null)
                {
                    res.DownloadUrl = _urlBuilder.BuildDownloadUrl(uri).ToString();
                    res.ViewUrl = _urlBuilder.BuildViewUrl(uri).ToString();
                    res.PermaLink = _urlBuilder.BuildPermaLink(uri).ToString();
                }
            }


            return res;


        }
    }
}