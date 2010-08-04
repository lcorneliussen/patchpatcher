using System;
using System.Text.RegularExpressions;

namespace PatchPatcher
{
    /// <summary>
    /// Tortoise-Diff requires each new file-change block to start with a <code>=</code> followed by <code>---</code>
    /// on the next line. When you create a patch from tortoise, it prints 68 equals-signs, but
    /// the parser is satisfied with one.
    /// </summary>
    public class InsertTortoiseSegmentationLineStep : IConversionStep
    {
        private readonly Regex _orig = new Regex(@"^--- ", RegexOptions.Multiline);

        #region IConversionStep Members

        public string Convert(string patchContents)
        {
            return _orig.Replace(patchContents, m => "=" + Environment.NewLine + m.Value);
        }

        #endregion
    }
}