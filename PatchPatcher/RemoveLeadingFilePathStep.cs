using System.Text.RegularExpressions;

namespace PatchPatcher
{
    ///<summary>
    ///  When creating patches with <code>git format-patch</code>, the file references start with
    ///  <code>
    ///  --- a/
    ///  +++ b/
    ///  </code>
    ///  by default. The pathes can be specified as commandline parameters, though. This stepp 
    ///  just removes the leading file pathes in the references.
    ///</summary>
    public class RemoveLeadingFilePathStep : IConversionStep
    {
        private readonly Regex _change = new Regex(@"^\+\+\+ .*?\/", RegexOptions.Multiline);
        private readonly Regex _orig = new Regex(@"^--- .*?\/", RegexOptions.Multiline);

        #region IConversionStep Members

        public string Convert(string patchContents)
        {
            patchContents = _orig.Replace(patchContents, "--- ");
            return _change.Replace(patchContents, "+++ ");
        }

        #endregion
    }
}