using Jimu;
using System;
using System.Collections.Generic;
using System.Text;

namespace User.IService
{
    /// <summary>
    /// 文件上存下载（file upload download）, 打开 /file/index 页面演示
    /// </summary>
    [JimuServiceRoute("/{Service}")]
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
