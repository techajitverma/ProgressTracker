using Microsoft.AspNetCore.Mvc;
using ProgressTrackerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProgressTrackerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoryInfoController : ControllerBase
    {
        // GET: api/<StoryInfoController>
        [HttpGet]
        public List<StoryInfoModel> Get()
        {
            return new ProgressTrackerBusiness.StoryInfoBusiness().GetAllStoryInfo();
        }

        // GET api/<StoryInfoController>/5
        [HttpGet("{id}")]
        public StoryInfoModel Get(int id)
        {
            return new ProgressTrackerBusiness.StoryInfoBusiness().GetStoryInfoById(id);
        }

        // POST api/<StoryInfoController>
        [HttpPost]
        public void Post([FromBody] StoryInfoModel value)
        {
            new ProgressTrackerBusiness.StoryInfoBusiness().SaveStoryInfo(value);
        }

        // PUT api/<StoryInfoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] StoryInfoModel value)
        {
            value.StoryId = id;
            new ProgressTrackerBusiness.StoryInfoBusiness().SaveStoryInfo(value);

        }

        // DELETE api/<SprintStoryTrackerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
