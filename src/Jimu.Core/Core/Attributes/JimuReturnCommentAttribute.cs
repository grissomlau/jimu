using System;

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
        public JimuReturnCommentAttribute(string comment, string defaultVal)
        {
            this.Comment = comment;
            this.Default = defaultVal;
        }
        /// <summary>
        /// <summary>
        /// comment can be used for swagger
        /// </summary>
        public string Comment { get; set; }
        public string Default { get; set; }
    }
}
