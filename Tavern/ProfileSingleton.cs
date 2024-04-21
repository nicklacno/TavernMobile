using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Tavern
{
    public class ProfileSingleton
    {
        private static ProfileSingleton _instance;
        public bool isLoggedIn;

        public delegate void LoginSuccessful();
        public delegate void UpdateProfile();

        public UpdateProfile updateProfile;
        public LoginSuccessful loginSuccessful; //login successful delegate

        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileBio { get; set; }

        public List<string> Friends { get; set; }
        public List<Group> Groups { get; set; }
        public List<string> BlockedUsers { get; set; }

        private readonly HttpClient _httpClient = new(); //creates client
        private const string BASE_ADDRESS = "https://n588x7k6-5273.usw2.devtunnels.ms"; //base address for persistent dev-tunnel for api

        /**
         * ProfileSingleton - private constructor to make the singleton
         * @param id - the id of the profile
         */
        private ProfileSingleton(int id)
        {
            ProfileId = id;//sets the profile id
            _httpClient.BaseAddress = new Uri(BASE_ADDRESS); //sets the base address of the httpclient

            //set to true for tabbed page, false for login
            isLoggedIn = false; //sets the isLoggedIn to false, will change when retaining data
            updateProfile = new UpdateProfile(InvokedUpdate);
        }

        private async Task SetValues()
        {
            string profileData = await this.GetProfileData();
            if (profileData != null)
            {
                JObject profile = JObject.Parse(profileData); //parses the json
                ProfileName = (string)profile["name"]; //gets the name of the profile
                ProfileBio = (string)profile["bio"]; //gets the bio for the profile

                await this.GetGroupsList();
            }


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
        
        public void InvokedUpdate()
        {
            Debug.WriteLine("Invoked Update");
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

            try
            {
                var response = await _httpClient.PostAsync("Profile/Login", content); //gets the response message
                int id = JsonSerializer.Deserialize<int>(response.Content.ReadAsStringAsync().Result); //Deserializes the response to an int and sets a variable

                if (id >= 0) // greater than 0 is a valid id
                {
                    ProfileId = id; //sets the id for the singleton
                    isLoggedIn = true; //sets the bool for logged in, later used for the remember me
                    await SetValues();
                }
                return isLoggedIn; //returns true if updated, else false
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }

        }

        /**
         * GetGroupsList - Returns a list of group names using the stored id
         * @return - List of group names
         */
        public async Task<List<Group>> GetGroupsList()
        {
            if (ProfileId < 0)
                return null;

            ConvertToGroupList(await _httpClient.GetStringAsync($"Profile/{ProfileId}/Groups"));
            return Groups;
        }


        /**
         * ConvertToGroupList - takes a json and converts it into a list of Group Objects
         * @param json - the json that needs to be parsed
         */
        private void ConvertToGroupList(string json)
        {
            try
            {
                List<Group> groups = new List<Group>();

                if (json != null)
                {
                    JToken data = JToken.Parse(json);
                    foreach (JObject groupData in data.Children())
                    {
                        groups.Add(ConvertToGroup(groupData));
                    }
                }


                Groups = groups;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Groups = null;
            }
        }

        /**
         * GetGroup - returns a group object from the given id
         * @param id - id for the given group
         * @return - a group object with the information
         */
        public async Task<Group> GetGroup(int id)
        {
            try
            {
                Group group = new Group(id);
                string json = await _httpClient.GetStringAsync($"Groups/{id}");
                if (json != null)
                {
                    JObject data = JObject.Parse(json);

                    group = ConvertToGroup(data);
                }
                return group;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        /**
         * ConvertToGroup - takes a JObject and converts it into group object
         * @param data - the JObject that stores the data
         * @return - the group object
         */
        private Group ConvertToGroup(JObject data)
        {
            int id = (int)data["groupId"];
            Group group = new Group(id);

            group.Name = (string)data["name"];
            group.Bio = (string)data["bio"];
            group.OwnerId = (int)data["ownerId"];
            group.Members = data["members"].Values<string>().ToList();
            group.Tags = data["tags"].Values<string>().ToList();

            return group;
        }

        public async Task<int> Register(string username, string password, string email, string city, string state)
        {
            Dictionary<string, string> value = new Dictionary<string, string>()
            {
                {"username", username},
                {"password", password},
                {"email", email },
                {"city", city},
                {"state", state}
            };

            var json = JsonSerializer.Serialize(value); //serializes the dictionary into a json string
            var content = new StringContent(json, Encoding.UTF8, "application/json"); // encodes the dictionary into an application/json

            try
            {
                var response = await _httpClient.PostAsync("Profile/Register", content); //gets the response message
                int id = JsonSerializer.Deserialize<int>(response.Content.ReadAsStringAsync().Result); //Deserializes the response to an int and sets a variable

                if (id > 0) // greater than 0 is a valid id
                {
                    ProfileId = id; //sets the id for the singleton
                    isLoggedIn = true; //sets the bool for logged in, later used for the remember me
                    await SetValues();
                }
                return id; //returns id if valid
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }

        public async Task<int> EditProfile(string password, string newUsername, string newBio)
        {
            string name = newUsername.Equals(ProfileName) ? null : newUsername;
            string bio = newBio.Equals(ProfileBio) ? null : newBio;

            Dictionary<string, string> values = new Dictionary<string, string>()
            {
                {"profileId", ProfileId.ToString() },
                {"password", password },
                {"newUsername", name },
                {"newBio", bio}
            };

            var json = JsonSerializer.Serialize(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var responses = await _httpClient.PostAsync("Profile/EditProfile", content);
                int code = JsonSerializer.Deserialize<int>(responses.Content.ReadAsStringAsync().Result);

                await GetProfileData();
                
                return code;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }
    }
}
