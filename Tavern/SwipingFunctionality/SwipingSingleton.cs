using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavern.SwipingFunctionality
{
    public class SwipingSingleton
    {
        public static Queue<Group> Groups = new Queue<Group>();
        public delegate void GroupAction();
        public static GroupAction SkipGroup;
        public static GroupAction RequestGroup;
        public Group LikedGroup { get; set; }
        private readonly HttpClient _httpClient = new(); //creates client
        private const string BASE_ADDRESS = "https://cxbg938k-7111.usw2.devtunnels.ms/"; //base address for persistent dev-tunnel for api
    }
    

    private async Task LikeGroup(Group likedGroup)
    {
        ProfileSingleton singleton = ProfileSingleton.GetInstance();

        using (var client = new HttpClient())
        {
            var parameters = new Dictionary<string, string>
            {
                { "groupId", likedGroup.GroupId.ToString() },
                { "userId", singleton.ProfileId.ToString() }
            };
            var content = new FormUrlEncodedContent(parameters);
            var response = await client.PostAsync(connectionString, content);
            if (response.IsSuccessStatusCode)
            {
                ShowNextGroup();
            }
            else
            {
                var popup = new ErrorPopup("You suck dick");
                this.ShowPopup(popup);
            }
        }
        //Group.JoinRequest(likedGroup.GroupId, singleton.ProfileId);

        // Show the next group after liking
    }
}
