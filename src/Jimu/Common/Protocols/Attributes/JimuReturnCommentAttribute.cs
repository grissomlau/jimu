using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public class JimuReturnCommentAttribute : Attribute
    {
        public JimuReturnCommentAttribute() { }
        public JimuReturnCommentAttribute(string comment)
        {
            this.Comment = comment;
        }
        /// <summary>
        /// <summary>
        /// comment can be used for swagger
        /// </summary>
        public string Comment { get; set; }
    }
}
