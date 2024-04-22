using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GroupsController : Controller
    {
        private ILogger<GroupsController> _logger;

        public GroupsController(ILogger<GroupsController> logger)
        {
            _logger = logger;
        }

        /**
         * GetGroup - Calls the GetGroup method in the Group Class
         * @param id - Id for the group
         * @return - The data for the group
         */
        [HttpGet("{id}")]
        public Group? GetGroup(int id)
        {
            return Group.GetGroup(id);
        }

        [HttpPost("EditGroup")]
        public int EditGroup(Dictionary<string, string> data)
        {
            return Group.EditGroup(data);
        }
    }
}
