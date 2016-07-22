using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace Baseline.Testing
{
    
    public class FileSystemTester : IDisposable
    {
        private readonly TestDirectory _testDirectory;

        public FileSystemTester()
        {
            _testDirectory = new TestDirectory();
            _testDirectory.ChangeDirectory();
        }      

        private string fullPath(params string[] paths)
        {
            return Path.GetFullPath(paths.Join(Path.DirectorySeparatorChar.ToString()));
        }

        [Fact]
        public void copy_with_preserve()
        {
            var system = new FileSystem();
            system.WriteStringToFile("a.txt", "something");
            system.WriteStringToFile("b.txt", "else");
            system.Copy("a.txt", "b.txt", CopyBehavior.preserve);

            system.ReadStringFromFile("b.txt").ShouldBe("else");

        }

        [Fact]
        public void copy_with_overwrite()
        {
            var system = new FileSystem();
            system.WriteStringToFile("a.txt", "something");
            system.WriteStringToFile("b.txt", "else");
            system.Copy("a.txt", "b.txt", CopyBehavior.overwrite);

            system.ReadStringFromFile("b.txt").ShouldBe("something"); 
        }

        [Fact]
        public void get_relative_path()
        {
            fullPath("a", "b", "1.bat").PathRelativeTo(fullPath("a", "b"))
                .ShouldBe("1.bat");

            fullPath("a", "b", "c", "1.bat").PathRelativeTo(fullPath("a", "b"))
                .ShouldBe("c{0}1.bat".ToFormat(Path.DirectorySeparatorChar));

            fullPath("a", "b", "c", "d", "1.bat").PathRelativeTo(fullPath("a", "b"))
                .ShouldBe("c{0}d{0}1.bat".ToFormat(Path.DirectorySeparatorChar));

            fullPath("a", "1.bat").PathRelativeTo(fullPath("a", "b"))
                .ShouldBe("..{0}1.bat".ToFormat(Path.DirectorySeparatorChar));

            fullPath("a", "1.bat").PathRelativeTo(fullPath("a", "b", "c"))
                .ShouldBe("..{0}..{0}1.bat".ToFormat(Path.DirectorySeparatorChar));

            fullPath("a", "1.bat").PathRelativeTo(fullPath("a", "b", "c", "d"))
                .ShouldBe("..{0}..{0}..{0}1.bat".ToFormat(Path.DirectorySeparatorChar));

            fullPath("A", "b", "1.bat").PathRelativeTo(fullPath("A", "b"))
                .ShouldBe("1.bat");

            fullPath("A", "b").PathRelativeTo(fullPath("A", "b"))
                .ShouldBeEmpty();
        }

        [Fact]
        public void path_relative_in_parallel_paths()
        {
            fullPath("folder2", "file2.txt")
                .PathRelativeTo(fullPath("folder1"))
                .ShouldBe("..{0}folder2{0}file2.txt".ToFormat(Path.DirectorySeparatorChar));
        }

        [Fact]
        public void combine_when_it_is_only_one_value()
        {
            FileSystem.Combine("a").ShouldBe("a");
        }

        [Fact]
        public void combine_with_two_values()
        {
            FileSystem.Combine("a", "b").ShouldBe("a{0}b".ToFormat(Path.DirectorySeparatorChar));
        }

        [Fact]
        public void combine_with_three_values()
        {
            FileSystem.Combine("a", "b", "c").ShouldBe("a{0}b{0}c".ToFormat(Path.DirectorySeparatorChar));
        }

        [Fact]
        public void combine_with_four_values()
        {
            FileSystem.Combine("a", "b", "c", "d").ShouldBe("a{0}b{0}c{0}d".ToFormat(Path.DirectorySeparatorChar));
        }

        [Fact]
        public void combine_with_rooted_first_value()
        {
            FileSystem.Combine("{0}a".ToFormat(Path.DirectorySeparatorChar), "b", "c").ShouldBe("{0}a{0}b{0}c".ToFormat(Path.DirectorySeparatorChar));
        }

        [Fact]
        public void combine_with_trailing_slashes()
        {
            FileSystem.Combine("a{0}".ToFormat(Path.DirectorySeparatorChar), "b", "c{0}".ToFormat(Path.DirectorySeparatorChar)).
				ShouldBe("a{0}b{0}c{0}".ToFormat(Path.DirectorySeparatorChar));
        }

        public void Dispose()
        {
            _testDirectory.Dispose();
        }
    }

    
    public class FileSystemIntegrationTester : IDisposable
    {
        private readonly TestDirectory _testDirectory;
        private FileSystem _fileSystem;
        private string _basePath;

        public FileSystemIntegrationTester()
        {
            _testDirectory = new TestDirectory();
            _testDirectory.ChangeDirectory();
            _fileSystem = new FileSystem();
            _basePath = Path.GetTempPath();
        }




        [Fact]
        public void folders_should_be_created_when_writing_to_a_file_path_having_folders_that_do_not_exist()
        {
            var pathDoesNotExist = Path.Combine(_basePath, randomName());
            var stream = new MemoryStream(new byte[] { 55, 66, 77, 88 });

            _fileSystem.WriteStreamToFile(Path.Combine(pathDoesNotExist, "file.txt"), stream);

            Directory.Exists(pathDoesNotExist).ShouldBeTrue();
        }

        [Fact]
        public void writing_a_large_file()
        {
            const string OneKLoremIpsum = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur varius arcu eget nisi porta sit amet aliquet enim laoreet. Mauris at lorem velit, in venenatis augue. Pellentesque dapibus eros ac ipsum rutrum varius. Mauris non velit euismod odio tincidunt fermentum eget a enim. Pellentesque in erat nisl, consectetur lacinia leo. Suspendisse hendrerit blandit justo, sed aliquet libero eleifend sed. Fusce nisi tortor, ultricies sed tempor sit amet, viverra at quam. Vivamus sem mi, semper nec cursus vel, vehicula sit amet nunc. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Cras commodo commodo tortor congue bibendum. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Pellentesque vel magna vitae dui accumsan venenatis. Nullam sed ante mauris, nec iaculis erat. Cras eu nibh vel ante adipiscing volutpat. Integer ullamcorper tempus facilisis. Vestibulum eu magna sit amet dolor condimentum vestibulum non a ligula. Nunc purus nibh amet.";
            var path = Path.Combine(_basePath, randomName());

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            for (var i = 0; i < FileSystem.BufferSize / 512; i++)
            {
                writer.Write(OneKLoremIpsum);
            }
            stream.Position = 0;

            _fileSystem.WriteStreamToFile(path, stream);

            var fileInfo = new FileInfo(path);
            fileInfo.Exists.ShouldBeTrue();
            fileInfo.Length.ShouldBe(stream.Length);
        }

        [Fact]
        public void moving_a_file_should_create_the_target_directory_path_if_necessary()
        {
            var fromDir = Path.Combine(_basePath, randomName());
            var fromPath = Path.Combine(fromDir, "file.txt");
            var stream = new MemoryStream(new byte[] { 55, 66, 77, 88 });
            _fileSystem.WriteStreamToFile(fromPath, stream);

            var toDir = Path.Combine(_basePath, randomName());
            var toPath = Path.Combine(toDir, "newfilename.txt");

            _fileSystem.MoveFile(fromPath, toPath);

            Directory.Exists(toDir).ShouldBeTrue();
        }

        [Fact]
        public void moving_a_file()
        {
            var stream = new MemoryStream(new byte[] { 55, 66, 77, 88 });
            var fromPath = Path.Combine(_basePath, randomName());
            _fileSystem.WriteStreamToFile(fromPath, stream);

            var toDir = Path.Combine(_basePath, randomName());
            var toPath = Path.Combine(toDir, "newfilename.txt");

            _fileSystem.MoveFile(fromPath, toPath);

            File.Exists(toPath).ShouldBeTrue();
        }

        private static string randomName()
        {
            return Guid.NewGuid().ToString().Replace("-", String.Empty);
        }


        [Fact]
        public void copy_directory()
        {
            var system = new FileSystem();

            system.ResetDirectory("dagobah");
            system.WriteStringToFile("dagobah".AppendPath("f1", "a.txt"), "something");
            system.WriteStringToFile("dagobah".AppendPath("f2", "a.txt"), "something");
            system.WriteStringToFile("dagobah".AppendPath("f3", "a.txt"), "something");
            system.WriteStringToFile("dagobah".AppendPath("f1", "f1a", "a.txt"), "something");
            system.WriteStringToFile("dagobah".AppendPath("f1", "f1a", "f1b", "a.txt"), "something");
            system.WriteStringToFile("dagobah".AppendPath("a.txt"), "something");

            system.DeleteDirectory("rhenvar");
            system.Copy("dagobah", "rhenvar");
            
            system.FindFiles("rhenvar", FileSet.Everything()).Select(x => x.PathRelativeTo("rhenvar")).OrderBy(x => x)
                .ShouldHaveTheSameElementsAs(
                    "a.txt",
                    FileSystem.Combine("f1", "a.txt"),
                    FileSystem.Combine("f1", "f1a", "a.txt"),
                    FileSystem.Combine("f1", "f1a", "f1b", "a.txt"),
                    FileSystem.Combine("f2", "a.txt"),
                    FileSystem.Combine("f3", "a.txt")
                );
        }

        public void Dispose()
        {
            _testDirectory.Dispose();
        }
    }


    
    public class Searching_up_the_tree_for_a_dir : IDisposable
    {
        private readonly TestDirectory _testDirectory;
        private IFileSystem _fileSystem;

        public Searching_up_the_tree_for_a_dir()
        {
            _testDirectory = new TestDirectory();
            _testDirectory.ChangeDirectory();
            _fileSystem = new FileSystem();
            _fileSystem.CreateDirectory("deep".AppendPath("a", "b", "c"));
            _fileSystem.CreateDirectory("deep".AppendPath("config"));
        }

        [Fact]
        public void found()
        {
            var expected = Directory.GetCurrentDirectory().AppendPath("deep".AppendPath("config"));
            
            var dir = _fileSystem.SearchUpForDirectory("deep/a/b/c".ToFullPath(), "config");

            dir.ShouldBe(expected);
        }
        [Fact]
        public void miss()
        {
           
            var dir = _fileSystem.SearchUpForDirectory("deep".AppendPath("a","b","c").ToFullPath(), "conig");

            dir.ShouldBeNull();
        }

        public void Dispose()
        {
            _testDirectory.Dispose();
        }
    }

}