using System;
using System.Collections.Generic;
using System.Text;

namespace IServices
{
    public class UserDTO
    {
        public string Name { get; set; }
        public bool IsAlive { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
