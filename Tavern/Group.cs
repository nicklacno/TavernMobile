using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavern
{
    public class Group
    {
        public int GroupId { get; set; }
        public string? Name { get; set; }
        public string? Bio { get; set; }
        public int OwnerId { get; set; }
        public ObservableCollection<OtherUser> Members { get; set; }
        public ObservableCollection<Tag> Tags { get; set; }
        public bool isPrivate { get; set; }
        public string? GroupCode { get; set; }


        public Group(int id, string name="", string bio="")
        {
            GroupId = id;
            Name = name;
            Bio = bio;
        }
    }
}
