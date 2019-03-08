using System;
using System.Collections.Generic;
using System.Text;

namespace IServices
{
    public class UserDTO
    {
        //[Jimu.JimuFieldCommentAttribute(Comment = "user name")]
        /// <summary>
        /// user name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否或者"
        /// </summary>
        public bool IsAlive { get; set; }

        /// <summary>
        /// {创建时间
        /// </summary>
        //[Jimu.JimuFieldCommentAttribute(@"创建时间
        //换行了
        //")]
        public DateTime CreatedTime { get; set; }
    }
}
