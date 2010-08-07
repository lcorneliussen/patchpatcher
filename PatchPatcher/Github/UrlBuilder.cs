using System;

namespace PatchPatcher.Github
{
    public class UrlBuilder : IUrlBuilder
    {
        private Uri _base;

        public UrlBuilder(Uri @base)
        {
            _base = @base;
        }

        public Uri BuildDownloadUrl(Uri uri)
        {
            return new Uri(_base, "download/github.com" + uri.AbsolutePath);
        }

        public Uri BuildViewUrl(Uri uri)
        {
            return new Uri(_base, "view/github.com" + uri.AbsolutePath);
        }

        public Uri BuildPermaLink(Uri uri)
        {
            return new Uri(_base, "github.com" + uri.AbsolutePath);
        }
    }
}