using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProgressTrackerBusiness;
using ProgressTrackerModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProgressTrackerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // GET: api/<UserController>
        [HttpGet]
        public List<UserModel> Get()
        {
            return new UserBusiness().GetAllUser();
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public UserModel Get(int id)
        {
            return new UserBusiness().GetUserById(id);
        }

        // POST api/<UserController>
        [HttpPost]
        public void Post([FromBody] UserModel value)
        {
            new UserBusiness().SaveUser(value);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] UserModel value)
        {
            value.UserId = id;
            new UserBusiness().SaveUser(value);

        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
