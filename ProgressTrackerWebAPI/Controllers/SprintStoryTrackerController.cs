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
    public class SprintStoryTrackerController : ControllerBase
    {
        // GET: api/<SprintStoryTrackerController>
        [HttpGet]
        public List<SprintStoryTrackerModel> Get()
        {
            return new ProgressTrackerBusiness.SprintStoryTrackerBusiness().GetAllSprintStoryTracker();
        }

        // GET api/<SprintStoryTrackerController>/5
        [HttpGet("{id}")]
        public SprintStoryTrackerModel Get(int id)
        {
            return new ProgressTrackerBusiness.SprintStoryTrackerBusiness().GetSprintStoryTrackerById(id);
        }

        // POST api/<SprintStoryTrackerController>
        [HttpPost]
        public void Post([FromBody] SprintStoryTrackerModel value)
        {
            new ProgressTrackerBusiness.SprintStoryTrackerBusiness().SaveSprintStoryTracker(value);
        }

        // PUT api/<SprintStoryTrackerController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] SprintStoryTrackerModel value)
        {
            value.SprintStoryTrackerId = id;
            new ProgressTrackerBusiness.SprintStoryTrackerBusiness().SaveSprintStoryTracker(value);

        }

        // DELETE api/<SprintStoryTrackerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
