using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tavern
{
    public class Message
    {
        public string Sender { get; set; }
        public string Body { get; set; }
        public string TimeSent { get; set; }  
    }

    public class MessageByDay : ObservableCollection<Message>
    {
        public string DateSent { get; set; }
        public MessageByDay(string DateSent, ObservableCollection<Message> messages) : base(messages)
        {
            this.DateSent = DateSent;
        }
        
    }
}
