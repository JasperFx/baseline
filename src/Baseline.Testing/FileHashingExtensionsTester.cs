using System;
using System.Threading;
using Shouldly;
using Xunit;

namespace Baseline.Testing
{
    
    public class FileHashingExtensionsTester : IDisposable
    {
        private readonly TestDirectory _testDirectory;

        public FileHashingExtensionsTester()
        {
            _testDirectory = new TestDirectory();
            _testDirectory.ChangeDirectory();
        }

        [Fact]
        public void hash_by_modified_is_repeatable()
        {
            var file1 = "a.txt";
            var file2 = "b.txt";
            new FileSystem().WriteStringToFile(file1, "something");
            new FileSystem().WriteStringToFile(file2, "else");

            file1.GetModifiedDateFileText().ShouldBe(file1.GetModifiedDateFileText());
            file2.GetModifiedDateFileText().ShouldBe(file2.GetModifiedDateFileText());
            file2.GetModifiedDateFileText().ShouldNotBe(file1.GetModifiedDateFileText());

            file1.HashByModifiedDate().ShouldBe(file1.HashByModifiedDate());
            file2.HashByModifiedDate().ShouldBe(file2.HashByModifiedDate());
            file2.HashByModifiedDate().ShouldNotBe(file1.HashByModifiedDate());
        
        }

        [Fact]
        public void hash_by_modified_is_dependent_upon_the_last_modified_time()
        {
            var file1 = "a.txt";
            new FileSystem().WriteStringToFile(file1, "something");
			
			Thread.Sleep(1000);
			
            var hash1 = file1.HashByModifiedDate();

            new FileSystem().WriteStringToFile(file1, "else");
			
			Thread.Sleep(1000);

            var hash2 = file1.HashByModifiedDate();

            hash1.ShouldNotBe(hash2);
        }

        [Fact]
        public void hash_group_of_files_by_modified_date()
        {
            var file1 = "a.txt";
            var file2 = "b.txt";
            var file3 = "c.txt";
            new FileSystem().WriteStringToFile(file1, "something");
            new FileSystem().WriteStringToFile(file2, "else");
            new FileSystem().WriteStringToFile(file3, "altogether");
			
			Thread.Sleep(1000);
			
            // Isn't dependent upon order of the files
            var hash1 = new string[]{file1, file2, file3}.HashByModifiedDate();
            var hash2 = new string[] { file2, file3, file1 }.HashByModifiedDate();
            var hash3 = new string[] { file2, file1, file3 }.HashByModifiedDate();

            hash1.ShouldBe(hash2);
            hash1.ShouldBe(hash3);

            var hash4 = new string[] { file1, file2 }.HashByModifiedDate();
            hash4.ShouldNotBe(hash1);

            new FileSystem().WriteStringToFile(file1, "else");
			
			Thread.Sleep(1000);
            
			var hash5 = new string[] { file2, file1, file3 }.HashByModifiedDate();

            hash5.ShouldNotBe(hash1);
        }

        public void Dispose()
        {
            _testDirectory.Dispose();
        }
    }
}