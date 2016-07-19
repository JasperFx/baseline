using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Baseline.Testing
{
    public class StreamExtensionsTests
    {
        [Fact]
        public void can_read_all_text_synchronously()
        {
            var helloWorld = "Hello world.";

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, Encoding.Unicode);
            writer.WriteLine(helloWorld);
            writer.Flush();

            stream.Position = 0;
            stream.ReadAllText().Trim()
                .ShouldBe(helloWorld);
        }

        [Fact]
        public async Task can_read_all_text_asynchronously()
        {
            var stream = new MemoryStream();
            new StreamWriter(stream) { AutoFlush = true }
            .WriteLine("Hello world.");

            stream.Position = 0;

            (await stream.ReadAllTextAsync().ConfigureAwait(false)).Trim()
                .ShouldBe("Hello world.");
        }

        [Fact]
        public void read_all_bytes()
        {
            var helloWorld = "Hello world.";
            var bytes = Encoding.Unicode.GetBytes(helloWorld);

            var stream = new MemoryStream();
            stream.Write(bytes, 0, bytes.Length);

            stream.Position = 0;

            var actual = stream.ReadAllBytes();

            actual.ShouldBe(bytes);
        }

        [Fact]
        public async Task read_all_bytes_async()
        {
            var helloWorld = "Hello world.";
            var bytes = Encoding.Unicode.GetBytes(helloWorld);

            var stream = new MemoryStream();
            stream.Write(bytes, 0, bytes.Length);

            stream.Position = 0;

            var actual = await stream.ReadAllBytesAsync().ConfigureAwait(false);

            actual.ShouldBe(bytes);
        }
    }
}