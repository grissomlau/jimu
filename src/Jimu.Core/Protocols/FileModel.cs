namespace Jimu.Core.Protocols
{
    /// <summary>
    ///     transfer file, use in  upload and download file
    /// </summary>
    public class FileModel
    {
        public FileModel()
        {
        }

        public FileModel(string name, byte[] data)
        {
            FileName = name;
            Data = data;
        }

        public string FileName { get; set; }
        public byte[] Data { get; set; }
    }
}