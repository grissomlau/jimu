using Jimu;
using System;
using System.Collections.Generic;
using System.Text;

namespace IService.User
{
    /// <summary>
    /// file upload and download, open /file/index to demo
    /// </summary>
    [Jimu("/{Service}")]
    public interface IFileService : IJimuService
    {
        /// <summary>
        /// upload file
        /// </summary>
        /// <param name="files"></param>
        [JimuPost(true)]
        void Upload(List<JimuFile> files);

        /// <summary>
        /// download file
        /// </summary>
        /// <returns></returns>
        [JimuGet(true)]
        JimuFile Download(string fileName);
    }
}
