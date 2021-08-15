﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Baseline
{
    public class FileSet
    {
        /// <summary>
        /// Does a deep search in the folder
        /// </summary>
        /// <param name="include">Semicolon delimited list of search criteria to be included in the results</param>
        /// <param name="exclude">Semicolon delimited list of search criteria to be excluded in the results</param>
        /// <returns></returns>
        public static FileSet Deep(string include, string? exclude = null)
        {
            return new FileSet{
                DeepSearch = true,
                Exclude = exclude,
                Include = include
            };
        }

        /// <summary>
        /// Does a shallow search in the immediate folder for files matching the path search
        /// </summary>
        /// <param name="include">Semicolon delimited list of search criteria to be included in the results</param>
        /// <param name="exclude">Semicolon delimited list of search criteria to be excluded in the results</param>
        /// <returns></returns>
        public static FileSet Shallow(string include, string? exclude = null)
        {
            return new FileSet{
                DeepSearch = false,
                Exclude = exclude,
                Include = include
            };
        }

        public FileSet()
        {
            Include = "*.*";
            DeepSearch = true;
        }

        public void AppendInclude(string include)
        {
            if (Include == "*.*")
            {
                Include = string.Empty;
            }

            if (Include.IsEmpty())
            {
                Include = include;
            }
            else
            {
                Include += ";" + include;
            }

        }

        [XmlAttribute]
        public string Include { get; set; }

        [XmlAttribute]
        public string? Exclude { get; set; }

        /// <summary>
        /// Search in child directories?
        /// </summary>
        public bool DeepSearch { get; set; }

        public void AppendExclude(string exclude)
        {
            if (Exclude.IsEmpty())
            {
                Exclude = exclude;
            }
            else
            {
                Exclude += ";" + exclude;
            }
        }

        public IEnumerable<string> IncludedFilesFor(string path)
        {
            var directory = new DirectoryInfo(path);

            return directory.Exists
                ? getAllDistinctFiles(path, Include.IsEmpty() ? "*.*" : Include)
                : new string[0];
        }

        private IEnumerable<string> getAllDistinctFiles(string path, string? pattern)
        {
            if (pattern.IsEmpty()) return new string[0];

            return pattern.Split(';').SelectMany(x =>
            {
                var fullPath = path;
                var dirParts = x.Replace("\\", "/").Split('/');
                var filePattern = x;

                if (dirParts.Length > 1)
                {
                    var subFolder = dirParts.Take(dirParts.Length - 1).Join(Path.DirectorySeparatorChar.ToString());
                    fullPath = Path.Combine(fullPath, subFolder);
                    filePattern = dirParts.Last();
                }

                var directory = new DirectoryInfo(fullPath);
                var searchOption = DeepSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                try
                {
                    return directory.Exists
                               ? Directory.GetFiles(fullPath, filePattern, searchOption)
                               : new string[0];
                }
                catch (DirectoryNotFoundException)
                {
                    return new string[0];
                }
            }).Distinct();
        }

        public IEnumerable<string> ExcludedFilesFor(string path)
        {
            return getAllDistinctFiles(path, Exclude);
        }

        public static FileSet ForAssemblyNames(IEnumerable<string> assemblyNames)
        {
            return new FileSet(){
				DeepSearch = false,
                Exclude = null,
                Include = assemblyNames.OrderBy(x => x).Select(x => "{0}.dll;{0}.exe".ToFormat(x)).Join(";")
            };
        }

        public static FileSet ForAssemblyDebugFiles(IEnumerable<string> assemblyNames)
        {
            return new FileSet(){
				DeepSearch = false,
                Exclude = null,
                Include = assemblyNames.OrderBy(x => x).Select(x => "{0}.pdb".ToFormat(x)).Join(";")
            };
        }

        public bool Equals(FileSet? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Include, Include) && Equals(other.Exclude, Exclude) && other.DeepSearch.Equals(DeepSearch);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (FileSet)) return false;
            return Equals((FileSet) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Include != null ? Include.GetHashCode() : 0);
                result = (result*397) ^ (Exclude != null ? Exclude.GetHashCode() : 0);
                result = (result*397) ^ DeepSearch.GetHashCode();
                return result;
            }
        }

        public override string ToString()
        {
            return $"Include: {Include}, Exclude: {Exclude}";
        }

        public static FileSet Everything()
        {
            return new FileSet(){DeepSearch = true, Include = "*.*"};
        }
    }
}