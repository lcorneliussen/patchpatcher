
using GithubSharp.Core.Models;

namespace PatchPatcher.Github
{
    public class UrlAnalyzeResult
    {
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }

        public bool IsCommit { get; set; }
        public bool IsBranch { get; set; }
        public string Path { get; set; }

        public string User { get; set; }
        public string Repository { get; set; }
        public string CommitOrBranch { get; set; }

        public Commit HeadCommit { get; set; }
        public Commit LastSvnCommit { get; set; }
        public Commit[] Changes { get; set; }

        public string SvnUrl { get; set; }
        public int SvnRevision { get; set; }

        public string CompareUrl { get; set; }
        public string DownloadUrl { get; set; }
        public string ViewUrl { get; set; }
        public string PermaLink { get; set; }
    }
}