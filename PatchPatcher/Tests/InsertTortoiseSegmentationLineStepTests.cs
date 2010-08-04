using System;
using NUnit.Framework;
using SharpTestsEx;

namespace PatchPatcher.Tests
{
    [TestFixture]
    public class InsertTortoiseSegmentationLineStepTests
    {
        [Test]
        public void Midline_LeavesUnchanged()
        {
            new InsertTortoiseSegmentationLineStep().Convert(" --- ")
                .Should().Be.EqualTo(" --- ");
        }

        [Test]
        public void Minmal_InsertsEqualsSign()
        {
            new InsertTortoiseSegmentationLineStep().Convert("--- ")
                .Should().Be.EqualTo("=" + Environment.NewLine + "--- ");
        }

        [Test]
        public void Multiple_ChangesAll()
        {
            new InsertTortoiseSegmentationLineStep().Convert(
                @"blabla
--- a.java
+++ a.java

blabla
--- b.java
+++ b.java")
                .Should().Be.EqualTo(@"blabla
=" + Environment.NewLine + @"--- a.java
+++ a.java

blabla
=" +
                                     Environment.NewLine + @"--- b.java
+++ b.java");
        }
    }
}