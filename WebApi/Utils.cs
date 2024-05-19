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
}
