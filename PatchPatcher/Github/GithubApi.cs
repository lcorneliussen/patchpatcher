using System.Collections.Generic;
using GithubSharp.Core.API;
using GithubSharp.Core.Models;
using GithubSharp.Core.Services;
using GithubSharp.Plugins.CacheProviders.WebCache;

namespace PatchPatcher.Github
{
    public class GithubApi : IGithubApi
    {
        private Commits _commits = new Commits(_webCacher, _logger);
        private static ICacheProvider _webCacher = new WebCacher();
        private static ILogProvider _logger = new NoOpLogger();

        public IEnumerable<Commit> CommitsInBranch(string username, string repository, string branch)
        {
            return _commits.CommitsForBranch(username, repository, branch);
        }

        public string BuildCompareUrl(string user, string repository, Commit newCommit, Commit olderCommit)
        {
            return "http://github.com/" + user + "/" + repository + "/compare/" +
                   olderCommit.id + "..." + newCommit.id;
        }
    }
}
