using System.Threading.Tasks;
using Jimu.Core.Protocols;
using Jimu.Core.Protocols.Attributes;

namespace Simple.IServices
{
    [ServiceRoute("api/user")]
    public interface IUserService : IService
    {
        [Service(Director = "grissom", Name = "get name")]
        Task<string> GetName();
        [Service(Director = "grissom", Name = "get id")]
        string GetId();

        [Service(Director = "grissom", Name = "set id")]
        void SetId();

        [Service(Director = "grissom", Name = "set name")]
        Task SetName(string name);
    }
}
