using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu
{
    [AttributeUsage(AttributeTargets.Method)]
    public class JimuReturnCommentAttribute : Attribute
    {
        public JimuReturnCommentAttribute() { }
        public JimuReturnCommentAttribute(string comment)
        {
            this.Comment = comment;
        }
        public JimuReturnCommentAttribute(string comment, string format)
        {
            this.Comment = comment;
            this.Format = format;
        }
        /// <summary>
        /// <summary>
        /// comment can be used for swagger
        /// </summary>
        public string Comment { get; set; }
        public string Format { get; set; }
    }
}
