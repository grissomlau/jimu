using System;
using System.Collections.Generic;
using System.Text;

namespace IServices
{
    public class UserFriendDTO
    {
        public string UserId { get; set; }
        public string FriendId { get; set; }
        public UserDTO FriendInfo { get; set; }
    }
}
