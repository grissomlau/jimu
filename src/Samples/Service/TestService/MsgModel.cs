namespace TestService
{
    public class MsgModel<T> where T : class
    {
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
        public static MsgModel<T> Success()
        {
            return new MsgModel<T>
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
        public static MsgModel<T> Success(T data, string msg = "")
        {
            return new MsgModel<T>
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
        public static MsgModel<T> Failure(string msg, T data = null)
        {
            return new MsgModel<T>
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
        public static MsgModel<T> Failure(string msg, string errorCode, T data = null)
        {
            return new MsgModel<T>
            {
                Status = false,
                Data = data,
                ErrorCode = errorCode,
                Msg = msg

            };
        }
    }
}
