using System.Linq;

namespace Baseline
{
    public static class UrlExtensions
    {
        /// <summary>
        /// Smart helper to append two url strings together.  Takes care of the
        /// "/" joining for you.  
        /// </summary>
        /// <param name="url"></param>
        /// <param name="part"></param>
        /// <returns></returns>
        public static string AppendUrl(this string url, string part)
        {
            var composite = (url ?? string.Empty).TrimEnd('/') + "/" + part.TrimStart('/');

            return composite.TrimEnd('/');
        }

        /// <summary>
        /// Removes the first segment of a Url string
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ChildUrl(this string url)
        {
            return url.Split('/').Skip(1).Join("/");
        }

        /// <summary>
        /// Removes the last segment of a Url string
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ParentUrl(this string url)
        {
            url = url.Trim('/');
            return url.Contains("/") ? url.Split('/').Reverse().Skip(1).Reverse().Join("/") : string.Empty;
        }

        /// <summary>
        /// Slightly smarter version of ChildUrl() that handles
        /// empty url's
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        public static string MoveUp(this string relativeUrl)
        {
            if (relativeUrl.IsEmpty()) return relativeUrl;

            var segments = relativeUrl.Split('/');
            if (segments.Count() == 1) return string.Empty;

            return segments.Skip(1).Join("/");
        }
    }
}