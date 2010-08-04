using System;
using System.Reflection;
using System.Text;

namespace PatchPatcher
{
    public class PrependInfoHeader : IConversionStep
    {
        private readonly string[] _additionalLines;

        public PrependInfoHeader()
        {
        }

        public PrependInfoHeader(string[] additionalLines)
        {
            _additionalLines = additionalLines;
        }

        #region IConversionStep Members

        public string Convert(string patchContents)
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("## Automatically converted by PatchPatcher {0}.{1}", version.Major,
                                        version.Minor));
            sb.AppendLine("##   lcorneliussen.de (C) 2010");
            sb.AppendLine("##   ");
            sb.AppendLine("##   PatchPatcher modifies git patches to be applyable on svn working copies.");
            sb.AppendLine("##   ");
            sb.AppendLine("##   More at http://lcorneliussen.de/patchpatcher");

            if (_additionalLines != null)
            {
                sb.AppendLine("##   ");
                foreach (string line in _additionalLines)
                {
                    sb.AppendLine("##   " + line);
                }
            }
            sb.AppendLine();
            sb.AppendLine(patchContents);
            return sb.ToString();
        }

        #endregion
    }
}