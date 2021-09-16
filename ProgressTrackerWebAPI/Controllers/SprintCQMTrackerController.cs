using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProgressTrackerModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProgressTrackerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SprintCQMTrackerController : ControllerBase
    {
        // GET: api/<SprintCQMTrackerController>
        [HttpGet]
        public IEnumerable<SprintCQMTrackerModel> Get()
        {
            return new List<SprintCQMTrackerModel>();
        }

        // GET api/<SprintTrackerController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<SprintCQMTrackerController>
        [HttpPost]
        public void Post([FromBody] SprintCQMTrackerModel sprintTracker)
        {
            new ProgressTrackerBusiness.SprintCQMTrackerBusiness().SaveSprintTracker(sprintTracker);
        }

        // PUT api/<SprintCQMTrackerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SprintCQMTrackerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
