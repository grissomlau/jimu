namespace Jimu
{
    /// <summary>
    ///     transfer file, use in  upload and download file
    /// </summary>
    public class JimuFile
    {
        public JimuFile()
        {
        }

        public JimuFile(string name, byte[] data)
        {
            FileName = name;
            Data = data;
        }

        public string FileName { get; set; }
        public byte[] Data { get; set; }
    }
}