using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Baseline
{
    public static class FileHashingExtensions
    {
        /// <summary>
        /// Creates a file hash based off of the file's modified date and full path
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string HashByModifiedDate(this string filename)
        {
            return filename.GetModifiedDateFileText().ToHash();
        }


        public static string GetModifiedDateFileText(this string filename)
        {
            var fullPath = filename.ToFullPath();
            return fullPath + ":" + File.GetLastWriteTime(fullPath).Ticks;
        }

        /// <summary>
        /// Creates a hash of several files based on both file path and last modified time
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static string HashByModifiedDate(this IEnumerable<string> files)
        {
            return files.OrderBy(x => x).Select(x => x.GetModifiedDateFileText()).Join("|").ToHash();
        }
    }
}