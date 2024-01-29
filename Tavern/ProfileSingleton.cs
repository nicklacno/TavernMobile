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

        public int ProfileId { get; set; }

        private readonly HttpClient _httpClient = new();
        private const string BASE_ADDRESS = "https://fz14z60p-5273.usw2.devtunnels.ms";

        private ProfileSingleton()
        {
            ProfileId = -1;
        }

        public static ProfileSingleton GetInstance()
        { 
            if (_instance == null)
            {
                _instance = new ProfileSingleton();
            }
            return _instance;
        }

        public async Task<string> GetProfileData(int id = -1)
        {
            if (id > 0)
                ProfileId = id;
            return await _httpClient.GetStringAsync($"{BASE_ADDRESS}/Profile/{ProfileId}");
        }
    }
}
