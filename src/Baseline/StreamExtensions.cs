using System.IO;
using System.Threading.Tasks;

namespace Baseline
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Read the contents of a Stream from its current location
        /// into a String
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ReadAllText(this Stream stream)
        {
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Read all the bytes in a Stream from its current
        /// location to a byte[] array
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
		public static byte[] ReadAllBytes(this Stream stream)
		{
			using (var content = new MemoryStream())
			{
				var buffer = new byte[4096];

				int read = stream.Read(buffer, 0, 4096);
				while (read > 0)
				{
					content.Write(buffer, 0, read);

					read = stream.Read(buffer, 0, 4096);
				}

				return content.ToArray();
			}
		}

        /// <summary>
        /// Asynchronously read the contents of a Stream from its current location
        /// into a String
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Task<string> ReadAllTextAsync(this Stream stream)
        {
            var reader = new StreamReader(stream);
            return reader.ReadToEndAsync();
        }

        /// <summary>
        /// Asynchronously read all the bytes in a Stream from its current
        /// location to a byte[] array
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<byte[]> ReadAllBytesAsync(this Stream stream)
        {
            using (var content = new MemoryStream())
            {
                var buffer = new byte[4096];

                int read = await stream.ReadAsync(buffer, 0, 4096).ConfigureAwait(false);
                while (read > 0)
                {
                    content.Write(buffer, 0, read);
                    read = await stream.ReadAsync(buffer, 0, 4096).ConfigureAwait(false);
                }

                return content.ToArray();
            }
        }
    }
}