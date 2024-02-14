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
        public Profile Get(int id)
        {
            return Profile.GetProfile(id); //calls singleton
        }
        /**
         * GetFriends - HttpGet method that returns the friends for a given profile id
         * @return - json string of friend usernames
         */
        [HttpGet("{id}/Friends")]
        public string GetFriends(int id)
        {
            return Profile.GetFriends(id); //calls singleton
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
    }
}
