using System.Collections.ObjectModel;

namespace Tavern
{
    public class Message
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Body { get; set; }
        public string TimeSent { get; set; }
    }

    public class MessageByDay : ObservableCollection<Message>
    {
        public string DateSent { get; set; }
        public DateTime LastMessageTime {  get; set; }
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

    public class OtherUser
    {
        public int Id { set; get; }
        public string Name { set; get; }
    }

    public class Request
    {
        public int RequestId { get; set; }
        public int GroupId { get; set; }
        public int UserId { get; set; }
        public required string UserName { get; set; }
    }

    public class RequestByGroup : ObservableCollection<Request>
    {
        public int GroupId { set; get; }
        public string GroupName { set; get; }

        public RequestByGroup(int GroupId, string GroupName, ObservableCollection<Request> requests) : base(requests)
        {
            this.GroupId = GroupId;
            this.GroupName = GroupName;
        }
    }

}
