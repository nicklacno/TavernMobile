using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tavern
{
    public class ProfileSingleton
    {
        private static ProfileSingleton _instance;
        private bool isLoggedIn;

        public delegate void ErrorMessage(string message);

        public delegate void UpdateProfile();

        public UpdateProfile updateProfile;

        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileBio { get; set; }

        private readonly HttpClient _httpClient = new();
        private const string BASE_ADDRESS = "https://nlk70t0m-5273.usw2.devtunnels.ms";

        private ProfileSingleton(int id)
        {
            ProfileId = id;
            _httpClient.BaseAddress = new Uri(BASE_ADDRESS);
            updateProfile = new UpdateProfile(PushToDatabase);
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
        
        /**
         * GetFriendsList - Calls the Api for the friends list of a given user
         * Return - json of an array of strings will be returned a
         */
        public async Task<string> GetFriendsList()
        {
            if (ProfileId < 0)
                return null;
            return await _httpClient.GetStringAsync($"Profile/{ProfileId}/Friends");
        }
        /**
         * PushToDatabase - temporary function to be called when the delegate is called
         */
        public void PushToDatabase()
        {
            Debug.WriteLine("Pushed?");
        }
        
        /**
         * Login - Attempting Login to the Database
         */
        public async Task<bool> Login(string username, string password)
        {
            var values = new Dictionary<string, string>()
            {
                { "username", username },
                { "password", password }
            };

            var json = JsonSerializer.Serialize(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Profile/Login", content);
            Debug.WriteLine(response.IsSuccessStatusCode);
            return false;
        }
    }
}
