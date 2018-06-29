using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Jimu;

namespace Simple.ConHttpServer.Controllers
{
    [Route("api/[controller]")]
    [ServiceRoute("api/values")]
    public class ValuesController : Controller, IServiceKey
    {
        // GET api/values
        [HttpGet]
        [Service(Director = "grissom", Name = "get")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [Service(Director = "grissom", Name = "get by id")]
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
