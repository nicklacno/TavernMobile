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
        public bool isLoggedIn;

        public delegate void ErrorMessage(string message); //object not created, need to fix!!!
        public delegate void UpdateProfile(); 
        public delegate void LoginSuccessful();

        public UpdateProfile updateProfile; //update profile delegate
        public LoginSuccessful loginSuccessful; //login successful delegate

        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileBio { get; set; }

        public List<string> Friends { get; set; }
        public List<string> Groups {  get; set; }
        public List<string> BlockedUsers { get; set; }

        private readonly HttpClient _httpClient = new(); //creates client
        private const string BASE_ADDRESS = "https://nlk70t0m-5273.usw2.devtunnels.ms"; //base address for persistent dev-tunnel for api

        /**
         * ProfileSingleton - private constructor to make the singleton
         * @param id - the id of the profile
         */
        private ProfileSingleton(int id)
        {
            ProfileId = id;//sets the profile id
            _httpClient.BaseAddress = new Uri(BASE_ADDRESS); //sets the base address of the httpclient
            updateProfile = new UpdateProfile(PushToDatabase); //initalizes the delegate object for updateProfile
            isLoggedIn = true; //sets the isLoggedIn to false, will change when retaining data
        }

        /**
         * GetInstance() - Returns the singleton, creates it if needed
         * @param id - the id for the profile
         * @return - returns the singleton object
         */
        public static ProfileSingleton GetInstance(int id = -1)
        { 
            if (_instance == null) //if null, create singleton
            {
                _instance = new ProfileSingleton(id);
            }
            return _instance;
        }

        /**
         * GetProfileData - returns a json string for the data of the given profile
         * @param id - id of the profile, if not entered, uses personal profile id
         * @return - returns profile json, else returns null
         */
        public async Task<string> GetProfileData(int id = -1)
        {
            if (id > 0) // sets the profile if valid id
                ProfileId = id;

            if (ProfileId < 0) //returns null if Profile id was not set
                return null;
            return await _httpClient.GetStringAsync($"Profile/{ProfileId}"); //calls for the profile id
        }
        
        /**
         * GetFriendsList - Calls the Api for the friends list of a given user
         * @return - json of an array of strings
         */
        public async Task<string> GetFriendsList()
        {
            if (ProfileId < 0) // retest !!!
                return null;
            return await _httpClient.GetStringAsync($"Profile/{ProfileId}/Friends");
        }
        /**
         * PushToDatabase - temporary function to be called when the delegate is called, will be changed to post or put call
         */
        public void PushToDatabase()
        {
            Debug.WriteLine("Pushed?");
        }
        
        /**
         * Login - Attempting Login to the Database
         * @param username - username of the account
         * @param password - password of the account
         */
        public async Task<bool> Login(string username, string password)
        {
            var values = new Dictionary<string, string>() //creates dictionary that will be serialized
            {
                { "username", username },
                { "password", password }
            };

            var json = JsonSerializer.Serialize(values); //serializes the dictionary into a json string
            var content = new StringContent(json, Encoding.UTF8, "application/json"); // encodes the dictionary into an application/json

            var response = await _httpClient.PostAsync("Profile/Login", content); //gets the response message
            int id = JsonSerializer.Deserialize<int>(response.Content.ReadAsStringAsync().Result); //Deserializes the response to an int and sets a variable

            if (id >= 0) // greater than 0 is a valid id
            {
                ProfileId = id; //sets the id for the singleton
                isLoggedIn = true; //sets the bool for logged in, later used for the remember me
            }
            return isLoggedIn; //returns true if updated, else false
        }

        /**
         * GetGroupsList - Returns a list of group names using the stored id
         * @return - List of group names
         */
        public async Task<string> GetGroupsList()
        {
            if (ProfileId < 0)
                return null;
            return await _httpClient.GetStringAsync($"Profile/{ProfileId}/Groups");
        }
    }
}
