using System.Threading.Tasks;
using Jimu;

namespace Simple.IServices
{
    [JimuServiceRoute("api/user")]
    public interface IUserService : IJimuService
    {
        [JimuServiceAttribute(CreatedBy = "grissom", CreatedDate = "20180530", Comment = "get name")]
        Task<string> GetName();
        [JimuServiceAttribute(CreatedBy = "grissom", Comment = "get id")]
        string GetId();

        [JimuServiceAttribute(CreatedBy = "grissom", Comment = "set id")]
        void SetId();

        [JimuServiceAttribute(CreatedBy = "grissom", Comment = "set name")]
        Task SetName(string name);
    }
}
