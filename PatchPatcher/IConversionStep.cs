namespace PatchPatcher
{
    public interface IConversionStep
    {
        string Convert(string patchContents);
    }
}