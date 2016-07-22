namespace Baseline
{
    public interface IFlatFileWriter
    {
        /// <summary>
        /// Writes the pattern [name]=[value] to a flat file
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void WriteProperty(string name, string value);
        void WriteLine(string line);

        /// <summary>
        /// Sort the lines in the file alphabetically
        /// </summary>
        void Sort();
    }
}