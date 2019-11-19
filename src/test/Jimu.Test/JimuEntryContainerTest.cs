using Autofac;
using Jimu.Server;
using Jimu.Server.ServiceContainer.Implement;
using System;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Jimu.Test
{

    public class JimuEntryContainerTest
    {

        private readonly ITestOutputHelper output;
        ServiceEntryContainer serviceEntry;
        public JimuEntryContainerTest(ITestOutputHelper output)
        {
            IContainer container = new ContainerBuilder().Build();
            serviceEntry = new ServiceEntryContainer();
            serviceEntry.LoadServices(new System.Collections.Generic.List<Assembly> { typeof(IUser).Assembly });
            this.output = output;
        }

        [Fact]
        public void AddServices_IfXmlComment_Available_ForReturn1()
        {

            var services = serviceEntry.GetServiceEntry();
            this.output.WriteLine(services[0].Descriptor.ReturnDesc);
            Assert.Equal("{\"ReturnType\":\"System.String\",\"ReturnFormat\":null,\"Comment\":\"name\",\"Properties\":[{\"Name\":\"String\",\"Type\":\"System.String\",\"Comment\":null,\"Default\":null,\"Properties\":null}]}", services[0].Descriptor.ReturnDesc);

        }
        [Fact]
        public void AddServices_IfXmlComment_Available_ForReturn2()
        {

            var services = serviceEntry.GetServiceEntry();
            this.output.WriteLine(services[1].Descriptor.ReturnDesc);
            Assert.Equal("{\"ReturnType\":\"Jimu.Test.User\",\"ReturnFormat\":null,\"Comment\":\"user\",\"Properties\":[{\"Name\":\"Id\",\"Type\":\"System.Int32\",\"Comment\":\"用户 id\",\"Default\":null,\"Properties\":null},{\"Name\":\"Name\",\"Type\":\"System.String\",\"Comment\":\"用户 name\",\"Default\":null,\"Properties\":null}]}", services[1].Descriptor.ReturnDesc);

        }



        [Fact]
        public void AddServices_IfXmlComment_Available_ForParameter1()
        {
            var services = serviceEntry.GetServiceEntry();
            this.output.WriteLine(services[0].Descriptor.Parameters);
            Assert.Equal("[{\"Name\":\"id\",\"Type\":\"System.String\",\"Comment\":\"userid\",\"Default\":null,\"Properties\":null}]", services[0].Descriptor.Parameters);
        }

        [Fact]
        public void AddServices_IfXmlComment_Available_ForMethod1()
        {
            var services = serviceEntry.GetServiceEntry();
            this.output.WriteLine(services[0].Descriptor.Comment);
            Assert.Equal("get user name", services[0].Descriptor.Comment);
        }
    }

    /// <summary>
    /// user
    /// </summary>
    [JimuServiceRoute("api/user")]
    public interface IUser : IJimuService
    {
        /// <summary>
        /// get user name
        /// </summary>
        /// <param name="id">userid</param>
        /// <returns>name</returns>
        [JimuService(CreatedBy = "grissom")]
        string GetName(string id);
        /// <summary>
        /// get user 
        /// </summary>
        /// <param name="id">userid</param>
        /// <param name="name">username</param>
        /// <returns>user</returns>
        [JimuService(Comment = "get user", CreatedBy = "grissom")]
        User GetUser(string id, string name);
    }

    public class User : IUser
    {
        public string GetName(string id)
        {
            throw new NotImplementedException();
        }

        public User GetUser(string id, string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 用户 id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用户 name
        /// </summary>
        public string Name { get; set; }
    }
}
