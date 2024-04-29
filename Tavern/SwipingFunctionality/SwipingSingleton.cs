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
}
