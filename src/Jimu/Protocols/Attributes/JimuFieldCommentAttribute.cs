using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public class JimuFieldCommentAttribute : Attribute
    {
        public JimuFieldCommentAttribute() { }
        public JimuFieldCommentAttribute(string comment)
        {
            this.Comment = comment;
        }
        public JimuFieldCommentAttribute(string field, string comment)
        {
            this.FieldName = field;
            this.Comment = comment;
        }
        /// <summary>
        /// <summary>
        /// comment can be used for swagger
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        ///     the name of field which comment for
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// default value for the field
        /// </summary>
        public string Default { get; set; }
    }
}
