using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
        private const string BASE_ADDRESS = "https://cxbg938k-7111.usw2.devtunnels.ms/"; //base address for persistent dev-tunnel for api
        public static SwipingSingleton GetInstance()
        {
            _instance = new SwipingSingleton();
            return _instance;
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
                var response = await _httpClient.PostAsync("Groups/LikeGroup", content);
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
