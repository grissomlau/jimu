using IService.User;
using Jimu;

namespace Service.User
{
    public class RedirectService : IRedirectService
    {
        public JimuRedirect Go(string url)
        {
            return new JimuRedirect(url);
        }
    }
}
