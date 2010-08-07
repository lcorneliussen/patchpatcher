using System.Collections.Generic;
using GithubSharp.Core.Models;

namespace PatchPatcher.Github
{
    public interface IGithubApi
    {
        IEnumerable<Commit> CommitsInBranch(string username, string repository, string branch);
        string BuildCompareUrl(string user, string repository, Commit newCommit, Commit olderCommit);
    }
}