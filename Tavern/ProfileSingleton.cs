﻿using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using GroupsList = System.Collections.ObjectModel.ObservableCollection<Tavern.Group>;
using JsonSerializer = System.Text.Json.JsonSerializer;
using MemberList = System.Collections.ObjectModel.ObservableCollection<Tavern.OtherUser>;
using MessageLog = System.Collections.ObjectModel.ObservableCollection<Tavern.MessageByDay>;
namespace Tavern
{
    public class ProfileSingleton : Profile
    {
        private static ProfileSingleton _instance;
        public bool isLoggedIn;

        public delegate void BasePageEvent(Page page);
        public BasePageEvent switchMainPage; //login successful delegate


        private readonly HttpClient _httpClient = new(); //creates client
        private const string BASE_ADDRESS = "https://n588x7k6-5273.usw2.devtunnels.ms/"; //base address for persistent dev-tunnel for api

        private ObservableCollection<Tag> ProfileTags = null;
        private ObservableCollection<Tag> GroupTags = null;

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

            if (Preferences.ContainsKey("profileId") && Preferences.Get("profileId", -1) != -1)
            {

                try
                {
                    ProfileId = Preferences.Get("profileId", -1);
                    Task t = Task.Run(async () => { await SetValues(); });
                    t.Wait();
                }
                catch (Exception ex)
                {
                    Preferences.Remove("profileId");
                    isLoggedIn = false;
                }
            }
        }


        public async Task SetValues()
        {
            string profileData = await GetProfileData();
            if (profileData != null)
            {
                JObject profile = JObject.Parse(profileData); //parses the json
                ProfileName = (string)profile["name"]; //gets the name of the profile
                ProfileBio = (string)profile["bio"]; //gets the bio for the profile

                await GetGroupsList();
                await GetTags();
                await GetFriendsList();
                isLoggedIn = true;
            }
            else
            {
                Logout();
            }

        }

        private async Task GetTags()
        {
            if (ProfileId < 0)
                return;
            try
            {
                string response = await _httpClient.GetStringAsync($"Profile/{ProfileId}/Tags");
                var curr = ConvertToTagList(response);

                await GetProfileTags();
                Tags.Clear();

                Dictionary<int, string> dict = new Dictionary<int, string>();
                foreach (var tag in curr)
                {
                    dict.Add(tag.Id, tag.Name);
                }

                foreach (var tag in ProfileTags)
                {
                    if (dict.ContainsKey(tag.Id) && dict[tag.Id].Equals(tag.Name))
                    {
                        Tags.Add(tag);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Tags.Clear();
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
        public async Task<MemberList> GetFriendsList()
        {
            if (ProfileId < 0) // retest !!!
                return null;
            try
            {
                string json = await _httpClient.GetStringAsync($"Profile/{ProfileId}/Friends");
                JToken token = JToken.Parse(json);
                Friends = ConvertToMembers(token);

                return Friends;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
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
        public async Task<GroupsList> GetGroupsList()
        {
            if (ProfileId < 0)
                return null;

            Groups = await ConvertToGroupList(await _httpClient.GetStringAsync($"Profile/{ProfileId}/AllGroups"));
            return Groups;
        }


        /**
         * ConvertToGroupList - takes a json and converts it into a list of Group Objects
         * @param json - the json that needs to be parsed
         */
        private async Task<GroupsList> ConvertToGroupList(string json)
        {
            try
            {
                GroupsList groups = new GroupsList();

                if (json != null)
                {
                    JToken data = JToken.Parse(json);
                    foreach (JObject groupData in data.Children())
                    {
                        groups.Add(await ConvertToGroup(groupData));
                    }
                }


                return groups;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
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
                if (!string.IsNullOrEmpty(json))
                {
                    JObject data = JObject.Parse(json);

                    group = await ConvertToGroup(data);
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
        private async Task<Group> ConvertToGroup(JObject data)
        {
            int id = (int)data["groupId"];
            Group group = new Group(id);

            group.Name = (string)data["name"];
            group.Bio = (string)data["bio"];
            group.OwnerId = (int)data["ownerId"];
            group.Members = ConvertToMembers(data["members"]);
            group.Tags = await GetGroupTags(id);
            group.IsPrivate = (bool)data["isPrivate"];
            group.GroupCode = group.OwnerId == ProfileId ? (string)data["groupCode"] : null;

            return group;
        }

        private MemberList ConvertToMembers(JToken json)
        {
            MemberList list = new MemberList();
            foreach (JObject member in json.Children())
            {
                list.Add(new OtherUser { Id = (int)member["id"], Name = (string)member["name"] });
            }
            return list;
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

                return code;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }

        public async Task<int> CreateGroup(string groupName, string groupBio, bool isPrivate)
        {
            Dictionary<string, string> values = new Dictionary<string, string>()
            {
                { "name", groupName },
                { "ownerId", ProfileId.ToString() },
                { "bio", groupBio },
                { "isPrivate", isPrivate.ToString() }
            };

            var json = JsonSerializer.Serialize(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("Profile/CreateGroup", content);
                int code = JsonSerializer.Deserialize<int>(response.Content.ReadAsStringAsync().Result);

                return code;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }

        public async Task<MessageLog> GetMessages(int groupId, DateTime? timestamp = null)
        {
            Dictionary<string, string> value = new Dictionary<string, string>();
            value["timestamp"] = timestamp == null ? null : timestamp.ToString();

            var json = JsonSerializer.Serialize(value);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"Groups/{groupId}/Chat", content);
                string messages = response.Content.ReadAsStringAsync().Result;

                return ConvertToMessageList(messages);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        private MessageLog ConvertToMessageList(string messages)
        {
            MessageLog messageList = new MessageLog();

            MessageByDay? messageByDay = null;

            if (!string.IsNullOrEmpty(messages))
            {
                JToken data = JToken.Parse(messages);
                foreach (JObject messageData in data.Children())
                {
                    var mData = messageData.ToObject<Dictionary<string, string>>();
                    DateTime timestamp = Convert.ToDateTime(mData["timestamp"]).ToLocalTime();

                    if (messageByDay == null)
                    {
                        messageByDay = new MessageByDay(timestamp.ToLongDateString(), new ObservableCollection<Message>());
                        messageByDay.FirstMessageTime = timestamp.ToUniversalTime();
                    }
                    else if (!messageByDay.DateSent.Equals(timestamp.ToLongDateString()))
                    {
                        messageList.Add(messageByDay);
                        messageByDay = new MessageByDay(timestamp.ToLongDateString(), new ObservableCollection<Message>());
                        messageByDay.FirstMessageTime = timestamp.ToUniversalTime();
                    }

                    messageByDay.Add(new Message()
                    {
                        Id = Convert.ToInt32(mData["id"]),
                        Sender = mData["sender"],
                        Body = mData["message"],
                        TimeSent = timestamp.ToShortTimeString()
                    });
                    messageByDay.LastMessageTime = timestamp.ToUniversalTime();
                }
                if (messageByDay != null) messageList.Add(messageByDay);
            }
            return messageList;
        }

        public async Task<int> SendMessage(int groupId, string message)
        {
            //DateTime now = DateTime.UtcNow;

            Dictionary<string, string> values = new Dictionary<string, string>()
            {
                { "senderId", ProfileId.ToString() },
                { "message", message }
            };

            var json = JsonSerializer.Serialize(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"Groups/{groupId}/SendMessage", content);
                int status = Convert.ToInt32(response.Content.ReadAsStringAsync().Result);
                return status;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Debug.WriteLine("Message Failed to Send");
                return -1;
            }
        }

        public async Task<int> DeleteGroup(int groupId)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            ProfileId = -1;
            ProfileName = "";
            ProfileBio = "";
            Groups?.Clear();
            Groups = null;
            Friends = null;
            Tags.Clear();
            isLoggedIn = false;
            switchMainPage.Invoke(new NavigationPage(new LoginPage()));
            Preferences.Remove("profileId");
        }

        public async Task<ObservableCollection<Tag>> GetProfileTags()
        {
            if (ProfileTags != null) return ProfileTags;

            try
            {
                var response = await _httpClient.GetAsync("Profile/Tags");
                string list = response.Content.ReadAsStringAsync().Result;

                ProfileTags = ConvertToTagList(list);
                return ProfileTags;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        private ObservableCollection<Tag> ConvertToTagList(string list)
        {
            ObservableCollection<Tag> tags = new ObservableCollection<Tag>();
            if (!string.IsNullOrEmpty(list))
            {
                JToken data = JToken.Parse(list);
                foreach (JObject tag in data.Children())
                {
                    string name = (string)tag["tagName"];
                    int id = (int)tag["tagId"];

                    tags.Add(new Tag { Name = name, Id = id });
                }
            }
            return tags;
        }

        public async Task<ObservableCollection<Tag>> GetGroupTags(int id = -1)
        {
            if (GroupTags == null) await GetAllGroupTags();

            if (id == -1) return GroupTags;
            try
            {
                var response = await _httpClient.GetStringAsync($"Groups/{id}/Tags");
                if (response == null) return null;

                var curr = ConvertToTagList(response);
                ObservableCollection<Tag> tags = new ObservableCollection<Tag>();

                Dictionary<int, string> dict = new Dictionary<int, string>();
                foreach (var tag in curr)
                {
                    dict.Add(tag.Id, tag.Name);
                }

                foreach (var tag in GroupTags)
                {
                    if (dict.ContainsKey(tag.Id) && dict[tag.Id].Equals(tag.Name))
                    {
                        tags.Add(tag);
                    }
                }

                return tags;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        private async Task<ObservableCollection<Tag>> GetAllGroupTags()
        {
            if (GroupTags != null) return GroupTags;
            try
            {
                var response = await _httpClient.GetAsync("Groups/Tags");
                string list = response.Content.ReadAsStringAsync().Result;

                GroupTags = ConvertToTagList(list);
                return GroupTags;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public bool CanAccessGroup(int groupId)
        {
            if (Groups == null) return false;
            foreach (var group in Groups)
            {
                if (group.GroupId == groupId)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<int> UpdateProfile(Dictionary<int, bool> tagUpdate)
        {
            var allTags = await GetProfileTags();
            try
            {
                foreach (var tag in allTags)
                {
                    Dictionary<string, int> values = new Dictionary<string, int>()
                    {
                        { "userId", ProfileId },
                        { "tagId", tag.Id }
                    };
                    var json = JsonSerializer.Serialize(values);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    if (tagUpdate.ContainsKey(tag.Id) && tagUpdate[tag.Id])
                    {
                        var response = await _httpClient.PostAsync($"Profile/AddTag", content);
                        int status = Convert.ToInt32(response.Content.ReadAsStringAsync().Result);
                        if (status == -10) return -10;
                        else if (status == -9) return -9;
                    }
                    else
                    {
                        var response = await _httpClient.PostAsync($"Profile/RemoveTag", content);
                        int status = Convert.ToInt32(response.Content.ReadAsStringAsync().Result);
                        if (status == -10) return -10;
                        else if (status == -9) return -9;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }

        }

        public async Task<ObservableCollection<Request>> GetGroupRequests(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Groups/{id}/Requests");
                string stuff = response.Content.ReadAsStringAsync().Result;

                return ConvertToGroupRequests(stuff);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }
        private ObservableCollection<Request> ConvertToGroupRequests(string json)
        {
            ObservableCollection<Request> requests = new ObservableCollection<Request>();
            if (!string.IsNullOrEmpty(json))
            {
                JToken data = JToken.Parse(json);
                foreach (JObject req in data)
                {
                    requests.Add(new Request
                    {
                        RequestId = (int)req["requestId"],
                        GroupId = (int)req["groupId"],
                        UserId = (int)req["profileId"],
                        UserName = (string)req["profileName"]
                    });
                }
            }
            return requests;
        }

        public async Task<int> AcceptMember(Request r)
        {
            Dictionary<string, int> values = new Dictionary<string, int>()
                    {
                        { "requestId", r.RequestId },
                        { "isAccepted", 1 }
                    };

            var json = JsonSerializer.Serialize(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("Groups/ModifyRequest", content);
                int status = Convert.ToInt32(response.Content.ReadAsStringAsync().Result);

                return status;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }

        public async Task<int> RejectMember(Request r)
        {
            Dictionary<string, int> values = new Dictionary<string, int>()
                    {
                        { "requestId", r.RequestId },
                        { "isAccepted", 0 }
                    };

            var json = JsonSerializer.Serialize(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("Groups/ModifyRequest", content);
                int status = Convert.ToInt32(response.Content.ReadAsStringAsync().Result);

                return status;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }

        public async Task<int> UpdateGroupTags(Group groupData, Dictionary<int, bool> updatedValues)
        {
            if (groupData == null) return -1;
            Group g = null;
            foreach (var group in Groups)
            {
                if (group.GroupId == groupData.GroupId)
                {
                    g = group;
                    break;
                }
            }

            if (g == null) return -1;

            g.Tags.Clear();
            foreach (var tag in GroupTags)
            {
                Dictionary<string, int> values = new Dictionary<string, int>
                {
                    { "groupId", g.GroupId },
                    { "tagId", tag.Id }
                };
                var json = JsonSerializer.Serialize(values);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (updatedValues.ContainsKey(tag.Id) && updatedValues[tag.Id])
                {
                    g.Tags.Add(tag);
                    response = await _httpClient.PostAsync("Groups/AddTag", content);
                }
                else
                {
                    response = await _httpClient.PostAsync("Groups/RemoveTag", content);
                }
                var retVal = Convert.ToInt32(response.Content.ReadAsStringAsync().Result);
                if (retVal != 0) return -2;
            }
            return 0;
        }


        //null means no change
        public async Task<int> UpdateGroupData(int id, string? newGroupname, string? newBio, bool? isPrivate)
        {
            if (newGroupname == null && newBio == null && isPrivate == null) return 0;

            Dictionary<string, string> values = new Dictionary<string, string>
            {
                { "newName", newGroupname },
                { "newBio", newBio },
                { "ownerId", ProfileId.ToString() },
                { "groupId", id.ToString() },
                { "isPrivate", isPrivate.ToString()}
            };

            try
            {
                var json = JsonSerializer.Serialize(values);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Groups/EditGroup", content);
                var retVal = Convert.ToInt32(response.Content.ReadAsStringAsync().Result);

                if (retVal == 0)
                {
                    foreach (Group g in Groups)
                    {
                        if (g.GroupId == id)
                        {
                            if (newGroupname != null) g.Name = newGroupname;
                            if (newBio != null) g.Bio = newBio;
                            if (isPrivate != null) g.IsPrivate = (bool)isPrivate;
                        }
                    }
                }

                return retVal;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }

        }

        public async Task<GroupsList> GetSearchResults(Dictionary<string, string> values)
        {
            var json = JsonSerializer.Serialize(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("Groups/SearchGroups", content);
                string ret = response.Content.ReadAsStringAsync().Result;
                return await ConvertToGroupList(ret);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public async Task<ObservableCollection<RequestByGroup>> GetAllRequests()
        {
            try
            {
                ObservableCollection<RequestByGroup> requests = new ObservableCollection<RequestByGroup>();
                var friend = await GetFriendRequests();
                if (friend.Count > 0)
                {
                    requests.Add(friend);
                }
                var response = await _httpClient.GetStringAsync($"Profile/{ProfileId}/OwnedGroups");
                JToken data = JToken.Parse(response);
                foreach (var group in data.Children())
                {
                    var reqs = await GetGroupRequests((int)group["id"]);
                    if (reqs.Count > 0)
                    {
                        requests.Add(new RequestByGroup((int)group["id"], (string)group["name"], reqs));
                    }
                }
                return requests;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        private async Task<RequestByGroup> GetFriendRequests()
        {
            var reqs = new ObservableCollection<Request>();
            try
            {
                var response = await _httpClient.GetStringAsync($"Profile/{ProfileId}/OpenPrivateRequests");
                JToken data = JToken.Parse(response);
                foreach (JObject obj in data.Children())
                {
                    Request r = new Request
                    {
                        GroupId = -1,
                        UserId = (int)obj["id"],
                        UserName = (string)obj["name"],
                        RequestId = -1
                    };
                    reqs.Add(r);
                }
                return new RequestByGroup(-1, "Friend Requests", reqs);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public async Task<MemberList> MyFriendRequests()
        {
            try
            {
                var m = new MemberList();

                var response = await _httpClient.GetStringAsync($"Profile/{ProfileId}/OpenFriendRequests");
                JToken data = JToken.Parse(response);
                foreach (JObject obj in data.Children())
                {
                    m.Add(new OtherUser { Id = (int)obj["id"], Name = (string)obj["name"] });
                }
                return m;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }


        //uses request class but username represents the group name
        public async Task<ObservableCollection<Request>> MyGroupRequests()
        {
            try
            {
                var m = new ObservableCollection<Request>();

                var response = await _httpClient.GetStringAsync($"Profile/{ProfileId}/OpenGroupRequests");
                JToken data = JToken.Parse(response);
                foreach (JObject obj in data.Children())
                {
                    m.Add(new Request
                    {
                        GroupId = (int)obj["otherID"],
                        UserName = (string)obj["otherName"],
                        RequestId = (int)obj["requestId"],
                        UserId = ProfileId
                    });
                }
                return m;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public async Task<MessageLog> GetPrivateChat(int otherId, DateTime? lastRetrieval)
        {
            int chatId = await GetChatId(otherId);
            if (chatId < 0) return null;

            Dictionary<string, string> values = new Dictionary<string, string>();
            values["timestamp"] = lastRetrieval == null ? null : lastRetrieval.ToString();

            var json = JsonSerializer.Serialize(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"Profile/PrivateChat/{chatId}", content);
                string messages = response.Content.ReadAsStringAsync().Result;

                return ConvertToMessageList(messages);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        private async Task<int> GetChatId(int otherId)
        {
            Dictionary<string, int> values = new Dictionary<string, int>()
            {
                { "userId", ProfileId },
                { "otherId", otherId }
            };

            var json = JsonSerializer.Serialize(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("Profile/PrivateChat/GetChatID", content);
                var chatId = Convert.ToInt32(response.Content.ReadAsStringAsync().Result);

                return chatId;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }

        public async Task<string> GetGroupCode(int groupId)
        {
            try
            {
                return await _httpClient.GetStringAsync($"Groups/{groupId}/Code");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public async Task<int> SendPrivateMessage(int otherId, string message)
        {
            Dictionary<string, string> values = new Dictionary<string, string>()
            {
                { "senderId", ProfileId.ToString() },
                { "recieverId", otherId.ToString() },
                { "message", message },
                { "timestamp", DateTime.UtcNow.ToString()}
            };

            var json = JsonSerializer.Serialize(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("Profile/SendPrivateMessage", content);
                return Convert.ToInt32(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }

        public async Task<int> SendFriendRequest(int otherId)
        {
            Dictionary<string, int> values = new Dictionary<string, int>()
            {
                { "userId", ProfileId },
                { "otherId", otherId }
            };

            var json = JsonSerializer.Serialize(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var response = await _httpClient.PostAsync("Profile/SendFriendRequest", content);
                var ret = Convert.ToInt32(response.Content?.ReadAsStringAsync().Result);
                return ret;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }

        public async Task<int> ModifyFriendRequest(int otherId, bool isAccepted)
        {
            Dictionary<string, int> values = new Dictionary<string, int>()
            {
                { "requestorID", otherId },
                { "userID", ProfileId },
                { "isAccepted", isAccepted ? 1 : 0 }
            };

            var json = JsonSerializer.Serialize(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var response = await _httpClient.PostAsync("Profile/ModifyFriendRequest", content);
                return Convert.ToInt32(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }

        public async Task<Group> GetGroupFromCode(string code)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"Groups/Code={code}");
                return await ConvertToGroup(JObject.Parse(response));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        //other profile data
        public async Task<Profile> GetProfile(int id)
        {
            Profile profile = new Profile { ProfileId = id };
            try
            {
                string json = await _httpClient.GetStringAsync($"Profile/{id}");
                var prof = JObject.Parse(json);
                profile.ProfileName = (string)prof["name"];
                profile.ProfileBio = (string)prof["bio"];

                profile.Groups = await ConvertToGroupList(await _httpClient.GetStringAsync($"Profile/{id}/Groups"));
                profile.Tags = ConvertToTagList(await _httpClient.GetStringAsync($"Profile/{id}/Tags"));
                var token = JToken.Parse(await _httpClient.GetStringAsync($"Profile/{id}/Friends"));
                profile.Friends = ConvertToMembers(token);

                return profile;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public async Task<int> RemoveFriend(int otherId)
        {
            Dictionary<string, int> values = new Dictionary<string, int>()
            { { "userId", ProfileId },
              { "otherId", otherId}
            };

            var json = JsonSerializer.Serialize(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("Profile/RemoveFriend", content);
                int ret = Convert.ToInt32(response.Content.ReadAsStringAsync().Result);

                return ret;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }

        public async Task<MessageLog> GetAnnouncements(int groupId, DateTime? latestRetrieval)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            values["timestamp"] = latestRetrieval == null ? null : latestRetrieval.ToString();

            var json = JsonSerializer.Serialize(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"Groups/{groupId}/Announcements", content);
                return ConvertToMessageList(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public async Task<int> PostAnnouncement(int groupId, string message)
        {
            Dictionary<string, string> values = new Dictionary<string, string>()
            {
                { "senderId", ProfileId.ToString() },
                { "message", message }
            };

            var json = JsonSerializer.Serialize(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var response = await _httpClient.PostAsync($"Groups/{groupId}/PostAnnouncement", content);
                var ret = Convert.ToInt32(response.Content.ReadAsStringAsync().Result);
                return ret;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }

        public async Task<int> KickMember(int groupId, int otherId)
        {
            Dictionary<string, int> values = new Dictionary<string, int>()
            {
                { "userId", ProfileId },
                { "otherId", otherId }
            };

            var json = JsonSerializer.Serialize(values);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var response = await _httpClient.PostAsync($"Groups/{groupId}/KickMember", content);
                return Convert.ToInt32(response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return -1;
            }
        }
    }
}