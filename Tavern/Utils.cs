using System.Collections.ObjectModel;

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
    public class Tag
    {
        public int Id { set; get; }
        public string Name { set; get; }
    }

    public class Request
    {
        public int RequestId { get; set; }
        public int GroupId { get; set; }
        public int ProfileId { get; set; }
        public string? ProfileName { get; set; }
    }
}
