namespace Jimu.Client.ApiGateway.Swagger
{
    public class JimuSwaggerOptions
    {

        /// <summary>
        /// 描述 api title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 描述 api 版本
        /// </summary>
        public string Version { get; set; }

        public JimuSwaggerOptions(string title, string version = "v1")
        {
            this.Title = title;
            this.Version = version;

        }

        public JimuSwaggerOptions() { }
    }
}
