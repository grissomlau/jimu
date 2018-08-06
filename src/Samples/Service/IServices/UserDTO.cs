using System;
using System.Collections.Generic;
using System.Text;

namespace IServices
{
    public class UserDTO
    {
        [Jimu.JimuFieldCommentAttribute(Comment = "user name")]
        public string Name { get; set; }
        public bool IsAlive { get; set; }

        [Jimu.JimuFieldCommentAttribute(@"创建时间
//换行了
")]
        public DateTime CreatedTime { get; set; }
    }
}
