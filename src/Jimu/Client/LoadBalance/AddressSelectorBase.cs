using System;
using System.Linq;
using System.Threading.Tasks;

namespace Jimu.Client.LoadBalance
{
    public abstract class AddressSelectorBase : IAddressSelector
    {
        Task<JimuAddress> IAddressSelector.GetAddressAsyn(JimuServiceRoute serviceRoute)
        {
            if (serviceRoute == null)
                throw new ArgumentNullException(nameof(serviceRoute));
            if (!serviceRoute.Address.Any(x => x.IsHealth))
                throw new ArgumentException($"{serviceRoute.ServiceDescriptor.Id},not available server");
            return GetAddressAsync(serviceRoute);
        }

        public abstract Task<JimuAddress> GetAddressAsync(JimuServiceRoute serviceRoute);
    }
}