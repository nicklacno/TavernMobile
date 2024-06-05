using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemberList = System.Collections.ObjectModel.ObservableCollection<Tavern.OtherUser>;
using GroupsList = System.Collections.ObjectModel.ObservableCollection<Tavern.Group>;

namespace Tavern
{
    public class Profile
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public string ProfileBio { get; set; }

        public MemberList Friends { get; set; }
        public GroupsList Groups { get; set; }
        public ObservableCollection<Tag> Tags { get; set; } = new ObservableCollection<Tag>();

        public List<string> BlockedUsers { get; set; }
        public int ImageID { get; set; }
    }
}
