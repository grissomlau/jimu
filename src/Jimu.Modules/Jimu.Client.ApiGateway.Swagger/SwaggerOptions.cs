namespace Jimu.Client.ApiGateway.Swagger
{
    public class SwaggerOptions
    {
        /// <summary>
        ///  api title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        ///  api version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// wether enable swagger
        /// </summary>
        public bool Enable { get; set; } = true;

        public SwaggerOptions(string title, string version = "v1")
        {
            this.Title = title;
            this.Version = version;

        }

        public SwaggerOptions()
        {
            this.Title = "Jimu";
            this.Version = "v1";
        }

    }
}
