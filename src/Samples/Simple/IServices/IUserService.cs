using System.Threading.Tasks;
using Jimu.Core.Protocols;
using Jimu.Core.Protocols.Attributes;

namespace Simple.IServices
{
    [ServiceRoute("api/user")]
    public interface IUserService : IService
    {
        [Service(CreatedBy = "grissom", CreatedDate = "20180530", Comment = "get name")]
        Task<string> GetName();
        [Service(CreatedBy = "grissom", Comment = "get id")]
        string GetId();

        [Service(CreatedBy = "grissom", Comment = "set id")]
        void SetId();

        [Service(CreatedBy = "grissom", Comment = "set name")]
        Task SetName(string name);
    }
}
