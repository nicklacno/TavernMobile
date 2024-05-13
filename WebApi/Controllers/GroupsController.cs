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

        [HttpPost("{id}/Chat")]
        public List<Dictionary<string, string>> GetGroupChat(int id, Dictionary<string, string> data)
        {
            return Group.Chat(id, data);
        }

        [HttpPost("{id}/SendMessage")]
        public int PostMessageGroupChat(int id, Dictionary<string, string> data)
        {
            return Group.SendMessage(id, data);
        }

        [HttpGet("Tags")]
        public List<Tag> GetTags()
        {
            return Group.GetTags();
        }

        [HttpPost("AddTag")]
        public int PostAddTag(Dictionary<string, int> data)
        {
            return Group.AddTag(data);
        }

        [HttpPost("RemoveTag")]
        public int PostRemoveTag(Dictionary<string, int> data)
        {
            return Group.RemoveTag(data);
        }

        [HttpGet("{id}/Requests")]
        public List<Request> GetRequests(int id)
        {
            return Group.Requests(id);
        }
    }
}
