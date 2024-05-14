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
        [HttpPost("LikeGroup")]
        public int LikeGroup(Dictionary<string, string> data)
        {
            string groupID = data["groupId"];
            string userID = data["userId"];
            return Group.JoinRequest(groupID, userID);
        }
        [HttpGet("{UserID}/PopulateGroups")]
        public List<Group> PopulateGroups(int UserID)
        {
            return Group.GetRandomGroupsForUser(UserID);
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
    }
}
