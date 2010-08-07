using System;
using GithubSharp.Core.Services;

namespace PatchPatcher.Github
{
    internal class NoOpLogger : ILogProvider
    {
        public void LogMessage(string message, params object[] arguments)
        {

        }

        public bool HandleAndReturnIfToThrowError(Exception error)
        {
            return true;
        }

        public bool DebugMode
        {
            get;
            set;
        }
    }
}