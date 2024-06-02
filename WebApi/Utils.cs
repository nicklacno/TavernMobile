namespace WebApi
{
    public class Request
    {
        public int RequestId { get; set; }
        public int GroupId { get; set; }
        public int ProfileId { get; set; }
        public string? ProfileName { get; set; }
    }

    public class Tag
    {
        public required int TagId { get; set; }
        public required string TagName { get; set; }
    }

    public class OpenRequest
    { 
        public int RequestId { get; set;}
        public int OtherID {  get; set; }
        public string OtherName { get; set; }
        public int ProfileId { get; set;}
    }

    //The data structure for the minimal data that may be returned to the application
    public class MiniInfo
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class Message
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string Body { get; set; }
    }

}
