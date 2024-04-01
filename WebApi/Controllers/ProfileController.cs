using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("[controller]")] //all http calls are through /Profile/
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private ILogger<ProfileController> _logger;

        public ProfileController(ILogger<ProfileController> logger)
        {
            _logger = logger;
        }

        /**
         * Get - HttpGet method that returns the profile data
         * @return - json string of profile data
         */
        [HttpGet("{id}")]
        public Profile? Get(int id)
        {
            return Profile.GetProfile(id); //calls singleton
        }
        /**
         * GetFriends - HttpGet method that returns the friends for a given profile id
         * @param id - the id of the user
         * @return - json string of friend usernames
         */
        [HttpGet("{id}/Friends")]
        public string? GetFriends(int id)
        {
            return Profile.GetFriends(id); //calls singleton
        }

        /**
         * GetGroups - HttpGet method that returns the names of the groups that the user is apart of
         * @param id - the id of the user
         * @return - json string of the group names
         */
        [HttpGet("{id}/Groups")]
        public List<Group>? GetGroups(int id)
        {
            return Profile.GetGroups(id);
        }

        /**
         * Request login to the Website. Should Return id number or -1 if failed login
         * @param creds - the username and password for the profile login
         */
        [HttpPost("Login")]
        public int PostLogin([FromBody] Dictionary<string, string> creds)
        {
            return Profile.GetProfileId(creds["username"], creds["password"]);
        }

        /**
         * Requests Registration of user for website. Will return id number or -1 if failed register for a particular reason
         * @param data - the data for the account
         */
        [HttpPost("Register")]
        public int PostRegister([FromBody] Dictionary<string, string> data) 
        {
            return Profile.Register(data);
        }
    }
}
