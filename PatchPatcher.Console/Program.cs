using System;
using System.Diagnostics;
using CommandLine;

namespace PatchPatcher.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            int exitCode = RunProgramm(args);

            if (Debugger.IsAttached)
                System.Console.ReadLine();

            Environment.Exit(exitCode);
        }

        private static int RunProgramm(string[] args)
        {
            var options = new Options();
            ICommandLineParser parser = new CommandLineParser();
            if (parser.ParseArguments(args, options, System.Console.Error))
            {
                try
                {
                    new PatchPatcher(options).Run();
                    return 0;
                }
                catch (Exception e)
                {
                    System.Console.Error.WriteLine(e.ToString());

                    if (Debugger.IsAttached)
                        throw;

                    return -1;
                }
            }

            return -1;
        }
    }
}