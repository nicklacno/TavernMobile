using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavern
{
    public class ProfileSingleton
    {
        private static ProfileSingleton _instance;
        private bool isLoggedIn;

        public delegate void ErrorMessage(string message);

        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileBio { get; set; }

        private readonly HttpClient _httpClient = new();
        private const string BASE_ADDRESS = "https://nlk70t0m-5273.usw2.devtunnels.ms";

        private ProfileSingleton(int id)
        {
            ProfileId = id;
        }

        public static ProfileSingleton GetInstance(int id = -1)
        { 
            if (_instance == null)
            {
                _instance = new ProfileSingleton(id);
            }
            return _instance;
        }

        public async Task<string> GetProfileData(int id = -1)
        {
            if (id > 0)
                ProfileId = id;

            if (ProfileId < 0)
                return null;
            return await _httpClient.GetStringAsync($"{BASE_ADDRESS}/Profile/{ProfileId}");
        }
        public async Task<string> GetFriendsList()
        {
            if (ProfileId < 0)
                return null;
            return await _httpClient.GetStringAsync($"{BASE_ADDRESS}/Profile/{ProfileId}/Friends");
        }
    }
}
