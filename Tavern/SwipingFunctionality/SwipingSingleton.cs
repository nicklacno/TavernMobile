using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavern.SwipingFunctionality
{
    public static class SwipingSingleton
    {
        public static Queue<Group> Groups = new Queue<Group>();
        public delegate void GroupAction();

        public static GroupAction SkipGroup;
        public static GroupAction RequestGroup;

    }
}
