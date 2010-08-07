using System;

namespace PatchPatcher.Github
{
    public interface IUrlBuilder
    {
        Uri BuildDownloadUrl(Uri uri);
        Uri BuildViewUrl(Uri uri);
        Uri BuildPermaLink(Uri uri);
    }
}