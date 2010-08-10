using System;
using System.Linq;
using GithubSharp.Core.Models;
using Moq;
using NUnit.Framework;
using PatchPatcher.Github;
using SharpTestsEx;

namespace PatchPatcher.Tests.Github
{
    [TestFixture]
    public class UrlAnalyzerTests
    {
        [Test]
        public void UrlAnalyzer_ValidBranch_ExtractsProperties()
        {
            string url = "http://github.com/user/repo/tree/branch";
            var res =
                new UrlAnalyzer(new Mock<IGithubApi>().Object).Analyze(
                    new Uri(url));
            res.IsValid.Should().Be.True();
            res.IsBranch.Should().Be.True();
            res.IsCommit.Should().Be.False();
            res.User.Should().Be.EqualTo("user");
            res.Repository.Should().Be.EqualTo("repo");
            res.CommitOrBranch.Should().Be.EqualTo("branch");
        }

        [Test]
        public void UrlAnalyzer_ValidCommitsUrl_IsRecognizedAsBranch()
        {
            string url = "http://github.com/user/repo/commits/branch";
            var res =
                new UrlAnalyzer(new Mock<IGithubApi>().Object).Analyze(
                    new Uri(url));
            res.IsValid.Should().Be.True();
            res.IsBranch.Should().Be.True();
            res.IsCommit.Should().Be.False();
            res.User.Should().Be.EqualTo("user");
            res.Repository.Should().Be.EqualTo("repo");
            res.CommitOrBranch.Should().Be.EqualTo("branch");
        }

        [Test]
        public void UrlAnalyzer_ValidCommit_ExtractsProperties()
        {
            var res =
                new UrlAnalyzer(new Mock<IGithubApi>().Object).Analyze(
                    new Uri("hhttp://github.com/user/repo/commit/sha"));
            res.IsValid.Should().Be.True();
            res.IsCommit.Should().Be.True();
            res.IsBranch.Should().Be.False();
            res.User.Should().Be.EqualTo("user");
            res.Repository.Should().Be.EqualTo("repo");
            res.CommitOrBranch.Should().Be.EqualTo("sha");
        }

        [Test]
        public void UrlAnalyzer_ValidBranch_FindsLastSvnCommit()
        {
            var github = new Mock<IGithubApi>();

            github.Setup(g => g.CommitsInBranch("user", "repo", "branch"))
                .Returns(new[]
                             {
                                 new Commit { id = "head-sha" }, 
                                 new Commit { id = "svn-sha", message = "git-svn-id: url@123 abcd" },
                                 new Commit { id = "svn-sha-2", message = "git-svn-id: url@234 abcd" }
                             });

            var res =
                new UrlAnalyzer(github.Object).Analyze(
                    new Uri("http://github.com/user/repo/tree/branch"));

            github.Verify(g => g.CommitsInBranch("user", "repo", "branch"));

            res.HeadCommit.Should().Not.Be.Null();
            res.LastSvnCommit.Should().Not.Be.Null();

            res.HeadCommit.id.Should().Be.EqualTo("head-sha");
            res.LastSvnCommit.id.Should().Be.EqualTo("svn-sha");
        }

        [Test]
        public void UrlAnalyzer_ValidBranch_FindsChanges()
        {
            var github = new Mock<IGithubApi>();

            github.Setup(g => g.CommitsInBranch("user", "repo", "branch"))
                .Returns(new[]
                             {
                                 new Commit { id = "head-sha" }, 
                                 new Commit { id = "change1-sha" },
                                 new Commit { id = "change2-sha" },
                                 new Commit { id = "svn-sha", message = "git-svn-id: url@123 abcd" }
                             });

            var res =
                new UrlAnalyzer(github.Object).Analyze(
                    new Uri("http://github.com/user/repo/tree/branch"));

            github.Verify(g => g.CommitsInBranch("user", "repo", "branch"));

            res.Changes.Should().Not.Be.Null();
            res.Changes.Select(c => c.id).Should().Have.SameSequenceAs("head-sha", "change1-sha", "change2-sha");
        }

        [Test]
        public void UrlAnalyzer_WithValidSvnCommit_ExractsSvnInfo()
        {
            var github = new Mock<IGithubApi>();

            github.Setup(g => g.CommitsInBranch("user", "repo", "branch"))
                .Returns(new[] { new Commit { id = "head-sha" }, new Commit { id = "svn-sha", message = "git-svn-id: https://svn.apache.org/repos/asf/maven/release/trunk@980883 13f79535-47bb-0310-9956-ffa450edef68" } });

            var res =
                new UrlAnalyzer(github.Object).Analyze(
                    new Uri("http://github.com/user/repo/tree/branch"));

            res.SvnUrl.Should().Be.EqualTo("https://svn.apache.org/repos/asf/maven/release/trunk");
            res.SvnRevision.Should().Be.EqualTo(980883);

        }

    }
}