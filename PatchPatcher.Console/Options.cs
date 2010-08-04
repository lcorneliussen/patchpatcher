using System;
using System.Collections.Generic;
using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace PatchPatcher.Console
{
    internal class Options
    {
        private const string ApplicationName = "PatchPatcher - Makes git patches usable from TortoiseDiff";

        [ValueList(typeof (List<string>), MaximumElements = -1)] public IList<string> InputPaths;

        /*[Option("q", "quiet", HelpText = "Omits console output.")]
        public bool Quiet;*/

        [Option("o", "output-dir", HelpText = "Location for the patched patch-files to be stored.")] public string
            OutputDir;

        [HelpOption("?", "help", HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            var heading = new HeadingInfo(ApplicationName, version.Major + "." + version.Minor);

            var help = new HelpText(heading);
            help.Copyright = new CopyrightInfo(true, "Lars Corneliussen", 2010);
            help.AddOptions(this);
            return help;
        }
    }
}