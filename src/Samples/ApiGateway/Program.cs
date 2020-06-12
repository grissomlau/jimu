using Microsoft.AspNetCore.Builder;

namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Jimu.Client.ApplicationWebClient.Instance.Run(null, (env, app) => app.UseStaticFiles());
        }
    }
}
