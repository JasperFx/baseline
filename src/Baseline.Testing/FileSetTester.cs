using System.Collections.Generic;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace Baseline.Testing
{
    
    public class FileSetTester
    {
        private FileSet theFileSet;

        public FileSetTester()
        {
            if (Directory.Exists("target"))
            {
                Directory.Delete("target", true);
            }

            Directory.CreateDirectory("target");

            theFileSet = new FileSet();
        }



        [Fact]
        public void append_include()
        {
            theFileSet.Include.ShouldBe("*.*");
            theFileSet.AppendInclude("*.config");

            theFileSet.Include.ShouldBe("*.config");
            theFileSet.AppendInclude("*.as*x");

            theFileSet.Include.ShouldBe("*.config;*.as*x");
        }

        private void writeFile(string name)
        {
            name = Path.Combine("target", name).ToFullPath();

            var directory = Path.GetDirectoryName(name);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (File.Exists(name))
            {
                File.Delete(name);
            }

            File.WriteAllText(name, "");
        }

        public IEnumerable<string> includedFiles()
        {
            return theFileSet.IncludedFilesFor("target").Select(Path.GetFileName).OrderBy(x => x);
        }

        public IEnumerable<string> excludedFiles()
        {
            return theFileSet.ExcludedFilesFor("target").Select(Path.GetFileName).OrderBy(x => x);
        }
		
		[Fact]
		public void find_includes_directoy_as_part_of_include_pattern_with_one()
		{
			writeFile("config/zee.config");
			theFileSet.Include = "config/*.config";
			
			includedFiles().ShouldHaveTheSameElementsAs("zee.config");			
		}
		
		[Fact]
		public void find_includes_directoy_as_part_of_include_pattern_with_many()
		{
			writeFile("config/a.config");
			writeFile("config/b.config");
			writeFile("config/zeppelin.yaml");
			writeFile("hitchhiker.config");
			
			theFileSet.Include = "config/*.config;config/zeppelin.yaml";			
			includedFiles().ShouldHaveTheSameElementsAs("a.config", "b.config", "zeppelin.yaml");			
		}

        [Fact]
        public void find_does_not_wig_out_when_the_exclude_pattern_is_an_invalid_directory()
        {
            writeFile("config/a.config");
            writeFile("config/b.config");
            writeFile("config/zeppelin.yaml");
            writeFile("hitchhiker.config");

            new FileSystem().DeleteDirectory("data");

            theFileSet = new FileSet()
            {
                Include = "*.as*x;*.master;Content{0}*.*;*.config".ToFormat(Path.DirectorySeparatorChar),
                Exclude = "data/*"
            };


            includedFiles().ShouldHaveTheSameElementsAs("a.config", "b.config", "hitchhiker.config");	
        }

        [Fact]
        public void a_null_include_finds_everything()
        {
            writeFile("a.txt");
            writeFile("a.xml");
            writeFile("b.txt");

            theFileSet.Include = null;

            includedFiles().ShouldHaveTheSameElementsAs("a.txt", "a.xml", "b.txt");
        }

        [Fact]
        public void find_includes_in_flat_directory_with_only_one_include()
        {
            writeFile("a.txt");
            writeFile("a.xml");
            writeFile("b.txt");
            writeFile("b.xml");
            writeFile("c.txt");
            writeFile("c.xml");

            theFileSet.Include = "*.txt";

            includedFiles().ShouldHaveTheSameElementsAs("a.txt", "b.txt", "c.txt");
        }

        [Fact]
        public void find_includes_in_flat_directory_with_multiple_includes()
        {
            writeFile("a.txt");
            writeFile("a.xml");
            writeFile("b.txt");
            writeFile("b.xml");
            writeFile("c.txt");
            writeFile("c.xml");

            theFileSet.Include = "*.txt;c.xml";

            includedFiles().ShouldHaveTheSameElementsAs("a.txt", "b.txt", "c.txt", "c.xml");
        }

        [Fact]
        public void find_includes_in_flat_directory_with_overlapping_includes_returns_distinct()
        {
            writeFile("a.txt");
            writeFile("a.xml");
            writeFile("b.txt");
            writeFile("b.xml");
            writeFile("c.txt");
            writeFile("c.xml");

            theFileSet.Include = "*.txt;c.*";

            includedFiles().ShouldHaveTheSameElementsAs("a.txt", "b.txt", "c.txt", "c.xml");
        }

        [Fact]
        public void find_includes_in_deep_directory_with_one_filter()
        {
            writeFile("a.txt");
            writeFile("a.xml");
            writeFile("f1/b.txt");
            writeFile("b.xml");
            writeFile("f1/f2/c.txt");
            writeFile("c.xml");

            theFileSet.Include = "*.txt";

            includedFiles().ShouldHaveTheSameElementsAs("a.txt", "b.txt", "c.txt");
        }

        [Fact]
        public void a_null_exclude_does_nothing()
        {
            writeFile("a.txt");
            writeFile("a.xml");
            writeFile("f1/b.txt");
            writeFile("b.xml");
            writeFile("f1/f2/c.txt");
            writeFile("c.xml");

            theFileSet.Exclude = null;

            excludedFiles().Any().ShouldBeFalse();
        }

        [Fact]
        public void mixed_include_and_exclude()
        {
            writeFile("a.txt");
            writeFile("a.xml");
            writeFile("f1/b.txt");
            writeFile("b.xml");
            writeFile("f1/f2/c.txt");
            writeFile("c.xml");

            theFileSet.Include = "*.xml";
            theFileSet.Exclude = "a.xml";

            new FileSystem().FindFiles("target", theFileSet).Select(x => Path.GetFileName(x)).ShouldHaveTheSameElementsAs("b.xml", "c.xml");
        }

        [Fact]
        public void get_fileset_for_assembly_names()
        {
            var names = new string[]{"a", "b", "c", "d"};
            var set = FileSet.ForAssemblyNames(names);
            set.Exclude.ShouldBeNull();
            set.Include.ShouldBe("a.dll;a.exe;b.dll;b.exe;c.dll;c.exe;d.dll;d.exe");
        }

        [Fact]
        public void get_fileset_for_assembly_debug_files()
        {
            var names = new string[] { "a", "b", "c", "d" };
            var set = FileSet.ForAssemblyDebugFiles(names);
            set.Exclude.ShouldBeNull();
            set.Include.ShouldBe("a.pdb;b.pdb;c.pdb;d.pdb");
        }
    }
}