using System.Collections.Generic;

namespace IServices
{
    public class UserFriendDTO
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FriendId { get; set; }
        public UserDTO FriendInfo { get; set; }
        public List<UserDTO> FriendInfos { get; set; }
        /// <summary>
        /// str
        /// </summary>
        public string[] Str { get; set; }
        /// <summary>
        /// ints
        /// </summary>
        public List<int> MyInt { get; set; }
        /// <summary>
        /// dics
        /// </summary>
        public Dictionary<string, int> MyDic { get; set; }
        /// <summary>
        /// mydic2222
        /// mydic2222
        /// </summary>
        public Dictionary<string, UserDTO> MyDic2 { get; set; }
    }
}
