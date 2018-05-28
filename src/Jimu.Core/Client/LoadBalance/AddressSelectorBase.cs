using System;
using System.Linq;
using System.Threading.Tasks;
using Jimu.Core.Protocols;

namespace Jimu.Core.Client.LoadBalance
{
    public abstract class AddressSelectorBase : IAddressSelector
    {
        Task<Address> IAddressSelector.GetAddressAsyn(ServiceRoute serviceRoute)
        {
            if (serviceRoute == null)
                throw new ArgumentNullException(nameof(serviceRoute));
            if (!serviceRoute.Address.Any(x => x.IsHealth))
                throw new ArgumentException($"{serviceRoute.ServiceDescriptor.Id},not available server");
            return GetAddressAsync(serviceRoute);
        }

        public abstract Task<Address> GetAddressAsync(ServiceRoute serviceRoute);
    }
}