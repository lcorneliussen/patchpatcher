using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PatchPatcher.Console
{
    internal class PatchPatcher
    {
        private static readonly string _baseDir = new DirectoryInfo(".").FullName;
        private readonly Options _options;

        public PatchPatcher(Options options)
        {
            _options = options;
        }

        public void Run()
        {
            IEnumerable<IConversionStep> steps = new IConversionStep[]
                                                     {
                                                         new RemoveLeadingFilePathStep(),
                                                         new InsertTortoiseSegmentationLineStep(),
                                                         new CorrectNullReferencesStep()
                                                     };

            foreach (FileInfo patchFile in FindPatches(_options.InputPaths))
            {
                System.Console.Write(patchFile.Name);
                System.Console.Write(" ..");
                string contents;
                using (StreamReader t = patchFile.OpenText())
                    contents = t.ReadToEnd();

                foreach (IConversionStep step in steps)
                {
                    System.Console.Write(".");
                    contents = step.Convert(contents);
                }

                string svnDir = _options.OutputDir ?? patchFile.DirectoryName;
                if (!Path.IsPathRooted(svnDir))
                    svnDir = Path.Combine(_baseDir, svnDir);

                var svnPatch = new FileInfo(Path.Combine(svnDir, "svn-" + patchFile.Name));
                if (svnPatch.Exists)
                    svnPatch.Delete();

                using (StreamWriter t = svnPatch.CreateText())
                    t.Write(contents);

                System.Console.WriteLine(".  Done.");
            }
        }

        private static IEnumerable<FileInfo> FindPatches(IList<string> inputPaths)
        {
            if (inputPaths.Count == 0)
                inputPaths.Add(".");

            // make all path's rooted
            IEnumerable<string> rootedPaths = inputPaths.Select(p =>
                                                                Path.IsPathRooted(p) ? p : Path.Combine(_baseDir, p)
                );

            foreach (string path in rootedPaths)
            {
                bool isDir = Directory.Exists(path);
                bool isFile = File.Exists(path);

                if (!(isDir || isFile))
                    throw new Exception("Could not find file or directory: " + path);

                if (isDir)
                {
                    var dir = new DirectoryInfo(path);
                    foreach (FileInfo file in dir.GetFiles("*.patch"))
                    {
                        if (!file.Name.StartsWith("svn-"))
                            yield return file;
                    }
                }
                else
                {
                    yield return new FileInfo(path);
                }
            }

            yield break;
        }
    }
}