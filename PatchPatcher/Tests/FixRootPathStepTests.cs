using System;
using NUnit.Framework;
using SharpTestsEx;

namespace PatchPatcher.Tests
{
    [TestFixture]
    public class FixRootPathStepTests
    {
        [Test]
        public void ChangeFileRefMidLine_DoesNotChange()
        {
            new RemoveLeadingFilePathStep().Convert(" +++ a/b")
                .Should().Be.EqualTo(" +++ a/b");
        }

        [Test]
        public void ChangeFileRefOnSecondLine_RemovesFirstPathSegment()
        {
            new RemoveLeadingFilePathStep().Convert(Environment.NewLine + "+++ a/b")
                .Should().Be.EqualTo(Environment.NewLine + "+++ b");
        }

        [Test]
        public void ChangeFileRef_RemovesFirstPathSegment()
        {
            new RemoveLeadingFilePathStep().Convert("+++ a/b")
                .Should().Be.EqualTo("+++ b");
        }

        [Test]
        public void MultiplePaths_RemovesFirstPathSegmentOnly()
        {
            new RemoveLeadingFilePathStep().Convert("--- a/b/c")
                .Should().Be.EqualTo("--- b/c");
        }

        [Test]
        public void OrigFileRefMidLine_DoesNotChange()
        {
            new RemoveLeadingFilePathStep().Convert(" --- a/b")
                .Should().Be.EqualTo(" --- a/b");
        }

        [Test]
        public void OrigFileRefOnSecondLine_RemovesFirstPathSegment()
        {
            new RemoveLeadingFilePathStep().Convert(Environment.NewLine + "--- a/b")
                .Should().Be.EqualTo(Environment.NewLine + "--- b");
        }

        [Test]
        public void OrigFileRef_RemovesFirstPathSegment()
        {
            new RemoveLeadingFilePathStep().Convert("--- a/b")
                .Should().Be.EqualTo("--- b");
        }
    }
}