using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Collections.ObjectModel;

namespace Tavern.SwipingFunctionality
{
    public class SwipingSingleton
    {
        private static SwipingSingleton _instance;
        public static Queue<Group> Groups = new Queue<Group>();
        public delegate void GroupAction();
        public static GroupAction SkipGroup;
        public static GroupAction RequestGroup;
        public Group LikedGroup { get; set; }
        private readonly HttpClient _httpClient = new(); //creates client
        private const string BASE_ADDRESS = "https://n588x7k6-5273.usw2.devtunnels.ms/"; //base address for persistent dev-tunnel for api
        private SwipingSingleton()
        {
            _httpClient.BaseAddress = new Uri(BASE_ADDRESS); 
        }
        public static SwipingSingleton GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SwipingSingleton();
            }
            return _instance;
        }
        public async Task PopulateGroups()
        {
            ProfileSingleton instance = ProfileSingleton.GetInstance();

            string groups = await _httpClient.GetStringAsync($"Groups/{instance.ProfileId}/PopulateGroups");
            
            Groups = ConvertToGroupList(groups);
            return;
        }

        private Queue<Group> ConvertToGroupList(string json)
        {
            try
            {
                Queue<Group> groups = new Queue<Group>();

                if (json != null)
                {
                    JToken data = JToken.Parse(json);
                    foreach (JObject groupData in data.Children())
                    {
                        groups.Enqueue(ConvertToGroup(groupData));
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

        public async Task SwipeRight(Group likedGroup)
        {
            ProfileSingleton singleton = ProfileSingleton.GetInstance();
            var parameters = new Dictionary<string, string>
            {
                { "groupId", likedGroup.GroupId.ToString() },
                { "userId", singleton.ProfileId.ToString() }
            };
            var json = JsonSerializer.Serialize(parameters);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                var response = await _httpClient.PostAsync("Groups/LikeGroup", content); //Throws exception
                int id = JsonSerializer.Deserialize<int>(response.Content.ReadAsStringAsync().Result);
                if (id == 0)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return;
            }
            //Group.JoinRequest(likedGroup.GroupId, singleton.ProfileId);
        }
    }
    

    
}
