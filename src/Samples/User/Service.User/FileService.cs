using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IService.User;
using Jimu;

namespace Service.User
{
    public class FileService : IFileService
    {
        public JimuFile Download(string fileName)
        {
            using (var ms = new MemoryStream())
            {
                var content = "success!";
                ms.Write(Encoding.UTF8.GetBytes("success!"), 0, content.Length);
                return new JimuFile($"{fileName}.txt", ms.ToArray());
            }
        }

        public void Upload(List<JimuFile> files)
        {
            foreach (var file in files)
            {
                using (var fs = new FileStream(file.FileName, FileMode.OpenOrCreate))
                {
                    fs.Write(file.Data, 0, file.Data.Length);
                }
            }
        }
    }
}
