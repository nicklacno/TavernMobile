using System;
using System.Collections.Generic;
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
        public List<string> Members { get; set; }
        public List<string> Tags { get; set; }


        public Group(int id, string name="", string bio="")
        {
            GroupId = id;
            Name = name;
            Bio = bio;
        }
    }
}
