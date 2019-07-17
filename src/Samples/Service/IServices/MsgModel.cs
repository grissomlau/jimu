using System;
using System.Collections.Generic;
using System.Text;

namespace IServices
{
    public class MsgModel2<T>    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 成功或失败消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 返回的业务数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 错误编码
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// 处理成功
        /// </summary>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static MsgModel2<T> Success()
        {
            return new MsgModel2<T>
            {
                Status = true
            };
        }


        /// <summary>
        /// 处理成功
        /// </summary>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static MsgModel2<T> Success(T data, string msg = "")
        {
            return new MsgModel2<T>
            {
                Status = true,
                Data = data,
                Msg = msg
            };
        }

        /// <summary>
        /// 处理失败
        /// </summary>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static MsgModel2<T> Failure(string msg, T data = default(T))
        {
            return new MsgModel2<T>
            {
                Status = false,
                Data = data,
                Msg = msg

            };
        }
        /// <summary>
        /// 处理失败
        /// </summary>
        /// <param name="data"></param>
        /// <param name="errorCode"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static MsgModel2<T> Failure(string msg, string errorCode, T data = default(T))
        {
            return new MsgModel2<T>
            {
                Status = false,
                Data = data,
                ErrorCode = errorCode,
                Msg = msg

            };
        }
    }
}
