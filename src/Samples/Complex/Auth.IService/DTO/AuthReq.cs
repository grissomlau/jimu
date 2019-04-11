using System;
using System.Collections.Generic;
using System.Text;

namespace Auth.IService.DTO
{
    public class AuthReq
    {
        public string Account { get; set; }
        public string Pwd { get; set; }
    }
}
