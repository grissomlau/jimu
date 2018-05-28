using System;

namespace Jimu.Core.Protocols.Attributes
{
    /// <summary>
    ///     who has this attribute service will be register as microservice
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ServiceAttribute : ServiceDescriptorAttribute
    {
        public ServiceAttribute()
        {
            IsWaitExecution = true;
        }

        /// <summary>
        ///     sync to invoke
        /// </summary>
        public bool IsWaitExecution { get; set; }

        /// <summary>
        ///     director for this service
        /// </summary>
        public string Director { get; set; }

        /// <summary>
        ///     service name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     service create date
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        ///     before invoke this service, need to be authorization
        /// </summary>
        public bool EnableAuthorization { get; set; }

        /// <summary>
        ///     just for specify roles to invoke this service
        /// </summary>
        public string Roles { get; set; }

        public override void Apply(ServiceDescriptor descriptor)
        {
            descriptor.WaitExecution(IsWaitExecution);
            descriptor.EnableAuthorization(EnableAuthorization);
            if (!string.IsNullOrEmpty(Director))
                descriptor.Director(Director);
            if (!string.IsNullOrEmpty(Date))
                descriptor.Date(Date);
            if (!string.IsNullOrEmpty(Name))
                descriptor.Name(Name);
            if (!string.IsNullOrEmpty(Roles))
                descriptor.Roles(Roles);
        }
    }
}