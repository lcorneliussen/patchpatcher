using NUnit.Framework;
using SharpTestsEx;

namespace PatchPatcher.Tests
{
    [TestFixture]
    public class CorrectNullReferencesStepTests
    {
        [Test]
        public void MinmalWithCRLF_CorrectsPath()
        {
            new CorrectNullReferencesStep().Convert("--- /dev/null\r\n+++ a")
                .Should().Be.EqualTo("--- a\r\n+++ a");
        }

        [Test]
        public void MinmalWithLF_CorrectsPath()
        {
            new CorrectNullReferencesStep().Convert("--- /dev/null\n+++ a")
                .Should().Be.EqualTo("--- a\n+++ a");
        }

        [Test]
        public void _matcher_BetweenLines_ShouldMatch()
        {
            new CorrectNullReferencesStep()._matcher.IsMatch("\n--- /dev/null\n+++ a\n").Should().Be.True();
        }

        [Test]
        public void _matcher_LeadingSpace_ShouldNotMatch()
        {
            new CorrectNullReferencesStep()._matcher.IsMatch(" --- /dev/null\n+++ a").Should().Be.False();
        }

        [Test]
        public void _matcher_OnNewLine_ShouldMatch()
        {
            new CorrectNullReferencesStep()._matcher.IsMatch("\n--- /dev/null\n+++ a").Should().Be.True();
        }

        [Test]
        public void _matcher_Simple_ShouldMatch()
        {
            new CorrectNullReferencesStep()._matcher.IsMatch("--- /dev/null\n+++ a").Should().Be.True();
        }
    }
}