using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Baseline
{
    public static class FileSystemExtensions
    {
        /// <summary>
        /// Shortcut to delete and recreate a directory
        /// </summary>
        /// <param name="fileSystem"></param>
        /// <param name="directory"></param>
        public static void ResetDirectory(this IFileSystem fileSystem, string directory)
        {
            fileSystem.DeleteDirectory(directory);
            fileSystem.CreateDirectory(directory);
        }



        public static void CopyToDirectory(this IFileSystem fileSystem, string source, string destination)
        {
            fileSystem.CreateDirectory(destination);
            fileSystem.Copy(source, destination);
        }

        public static void WriteToFlatFile(this IFileSystem system, string path, Action<IFlatFileWriter> configuration)
        {
            system.AlterFlatFile(path, list => configuration(new FlatFileWriter(list)));
        }


        public static bool DirectoryExists(this IFileSystem fileSystem, params string[] pathParts)
        {
            return fileSystem.DirectoryExists(FileSystem.Combine(pathParts));
        }

        public static void LaunchEditor(this IFileSystem fileSystem, params string[] pathParts)
        {
            fileSystem.LaunchEditor(FileSystem.Combine(pathParts));
        }

        public static bool FileExists(this IFileSystem fileSystem, params string[] pathParts)
        {
            return fileSystem.FileExists(FileSystem.Combine(pathParts));
        }

        public static T LoadFromFile<T>(this IFileSystem fileSystem, params string[] pathParts) where T : new()
        {
            return fileSystem.LoadFromFile<T>(FileSystem.Combine(pathParts));
        }

        public static IEnumerable<string> ChildDirectoriesFor(this IFileSystem fileSystem, params string[] pathParts)
        {
            return fileSystem.ChildDirectoriesFor(FileSystem.Combine(pathParts));
        }

        public static IEnumerable<string> FileNamesFor(this IFileSystem fileSystem, FileSet set, params string[] pathParts)
        {
            return fileSystem.FindFiles(FileSystem.Combine(pathParts), set);
        }

        public static string ReadStringFromFile(this IFileSystem fileSystem, params string[] pathParts)
        {
            return fileSystem.ReadStringFromFile(FileSystem.Combine(pathParts));
        }

        public static void PersistToFile(this IFileSystem fileSystem, object target, params string[] pathParts)
        {
            fileSystem.WriteObjectToFile(FileSystem.Combine(pathParts), target);
        }

        public static void DeleteDirectory(this IFileSystem fileSystem, params string[] pathParts)
        {
            fileSystem.DeleteDirectory(FileSystem.Combine(pathParts));
        }

        public static void CreateDirectory(this IFileSystem fileSystem, params string[] pathParts)
        {
            fileSystem.CreateDirectory(FileSystem.Combine(pathParts));
        }

        public static string? SearchUpForDirectory(this IFileSystem fileSystem, string startingPoint, string directoryToFind)
        {
            var dirs = fileSystem.ChildDirectoriesFor(startingPoint).Select(dir => new DirectoryInfo(dir));
            if(!dirs.Any(dir=>dir.Name.EqualsIgnoreCase(directoryToFind)))
            {
                var par = Directory.GetParent(startingPoint);
                if(par?.Parent == null)
                {
                    return null;
                }
                return fileSystem.SearchUpForDirectory(par.FullName, directoryToFind);
            }

            //need a break clause

            return dirs.First(dir=>dir.Name.EqualsIgnoreCase(directoryToFind)).FullName;
        }


        public static string? SearchUpForDirectory(this IFileSystem fileSystem, string directoryToFind)
        {
            return fileSystem.SearchUpForDirectory(".", directoryToFind);
        }

        // Basic integration coverage on this but having to rely mostly on manual testing here
        /// <summary>
        /// Does a "smart" cleanup of the contents of a folder by finding every file
        /// and deleting one file at a time from longest path to shortest.  Can get around
        /// file locking issues with a straight up IFileSystem.CleanDirectory()
        /// </summary>
        /// <param name="system"></param>
        /// <param name="path"></param>
        public static void ForceClean(this IFileSystem system, string path)
        {
            if (path.IsEmpty()) return;
            if (!Directory.Exists(path)) return;

            try
            {
                cleanDirectory(path, false);
            }
            catch
            {
                // just retry it
                cleanDirectory(path, false);
            }
        }

        private static void cleanDirectory(string directory, bool remove = true)
        {
            string[] files = Directory.GetFiles(directory);
            string[] children = Directory.GetDirectories(directory);

            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            Thread.Sleep(10);

            foreach (var child in children)
            {
                cleanDirectory(child);
            }

            if (remove)
            {
                Thread.Sleep(10);
                Directory.Delete(directory, false);
            }
        }
    }
}