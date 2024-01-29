using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private ILogger<ProfileController> _logger;

        public ProfileController(ILogger<ProfileController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{id}")]
        public Profile Get(int id)
        {
            return Profile.GetProfile(id);
        }

        [HttpGet("{id}/Friends")]
        public string GetFriends(int id)
        {
            return Profile.GetFriends(id);
        }
    }
}
