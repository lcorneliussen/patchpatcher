using System.Text.RegularExpressions;

namespace PatchPatcher
{
    ///<summary>
    ///  When creating patches that include new files, the file-header will look like this:
    ///  <code>
    ///  --- /dev/null
    ///  +++ b/myfile.txt
    ///  </code>.
    ///  TortoiseDiff needs the correct reference in both, so this step will rework the first path 
    ///  to the same as the second path.
    ///</summary>
    public class CorrectNullReferencesStep : IConversionStep
    {
        internal readonly Regex _matcher = new Regex(
            @"^(?<start>--- )(\/)?dev\/null(?<mid>(\r)?\n\+\+\+ )(?<path>.*)$",
            RegexOptions.Multiline | RegexOptions.ExplicitCapture);

        #region IConversionStep Members

        public string Convert(string patchContents)
        {
            return _matcher.Replace(patchContents,
                                    m => m.Groups["start"].Value + m.Groups["path"].Value
                                         + m.Groups["mid"].Value + m.Groups["path"].Value);
        }

        #endregion
    }
}