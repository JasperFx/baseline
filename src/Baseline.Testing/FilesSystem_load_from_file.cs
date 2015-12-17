using System;
using System.IO;
using System.Xml.Serialization;
using Shouldly;
using Xunit;

namespace Baseline.Testing
{
	[XmlType("serializeMe")]
	public class SerializeMe
	{
		public static string SerializedXml = @"<?xml version=""1.0""?><serializeMe xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""><Name>Serialized Name</Name><Index>42</Index></serializeMe>";

		public string Name { get; set; }
		public int Index { get; set; }
	}
	
	
	public class FilesSystem_load_from_file : IDisposable
	{
	    private readonly TestDirectory _testDirectory;

	    public FilesSystem_load_from_file()
	    {
            _testDirectory = new TestDirectory();
            _testDirectory.ChangeDirectory();
	    }

		[Fact]
		public void should_deserialize_xml()
		{
			var fileSystem = new FileSystem();
			var fileName = Path.GetTempFileName();
			fileSystem.WriteStringToFile(fileName, SerializeMe.SerializedXml);

			var result = fileSystem.LoadFromFile<SerializeMe>(fileName);

			result.Name.ShouldBe("Serialized Name");
			result.Index.ShouldBe(42);
		}

		[Fact]
		public void should_return_empty_instance_when_file_does_not_exist()
		{
			var fileSystem = new FileSystem();
			const string fileName = "does not exist";

			var result = fileSystem.LoadFromFile<SerializeMe>(fileName);

			result.Name.ShouldBeNull();
			result.Index.ShouldBe(0);
		}

		[Fact]
		public void should_thrown_when_file_is_not_xml()
		{
			var fileSystem = new FileSystem();
			var fileName = Path.GetTempFileName();
			fileSystem.WriteStringToFile(fileName, "not xml!");

            Exception<Exception>.ShouldBeThrownBy(() => fileSystem.LoadFromFile<SerializeMe>(fileName));
		}

		[Fact]
		public void load_from_file_or_throw_shuld_throw_when_file_does_not_exist()
		{
			var fileSystem = new FileSystem();
			const string fileName = "does not exist";

            Exception<Exception>.ShouldBeThrownBy(() => fileSystem.LoadFromFileOrThrow<SerializeMe>(fileName));
		}

	    public void Dispose()
	    {
            _testDirectory.Dispose();
	    }
	}

	
	public class FilesSystem_load_from_file_or_throw
	{
		[Fact]
		public void should_throw_when_file_does_not_exist()
		{
			var fileSystem = new FileSystem();
			const string fileName = "does not exist";

            Exception<Exception>.ShouldBeThrownBy(() => fileSystem.LoadFromFileOrThrow<SerializeMe>(fileName));
		}
	}
}