using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jimu.Client;
using Microsoft.AspNetCore.Mvc;
using IServices;

namespace ApiGateway.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IServiceProxy _serviceProxy;
        public ValuesController(IServiceProxy proxy)
        {
            _serviceProxy = proxy;
        }
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var echoService = _serviceProxy.GetService<IEchoService>();
            var echo = echoService.GetEcho("hah");
            return new string[] { "echo is: " + echo };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
