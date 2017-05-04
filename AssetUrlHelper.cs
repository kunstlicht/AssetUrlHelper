using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Kunstlicht.Mvc.Helpers
{
    public class AssetUrlHelper
    {
        private readonly IHostingEnvironment _hosting;
        private readonly IUrlHelper _url;

        public AssetUrlHelper(IActionContextAccessor actionContext, IHostingEnvironment hosting)
        {
            _hosting = hosting;
            _url = new UrlHelperFactory().GetUrlHelper(actionContext.ActionContext);
        }

        /// <summary>
        /// Content("/some/path", "vendor.*.js"); => /some/path/vendor.a72fe773ec0d163dbec9.js
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public string Content(string path, string pattern)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("can't be null or white space", nameof(path));

            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentException("can't be null or white space", nameof(pattern));

            var fileMatches = Directory.GetFileSystemEntries(Path.Combine(_hosting.WebRootPath, path), pattern);
            if (fileMatches == null || fileMatches.Length == 0)
                throw new InvalidOperationException($"No matching file found using '{path}' and '{pattern}'.");

            if (fileMatches.Length > 1)
                throw new InvalidOperationException($"Parameters '{path}' and '{pattern}' don't uniquely match file: found " 
                    + $"{fileMatches.Length} matches instead of 1.");

            var fileName = Path.GetFileName(fileMatches.Single());
            return _url.Content($"{path}/{fileName}");

        }
    }
}
