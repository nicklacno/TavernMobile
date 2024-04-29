
namespace Tavern;

public partial class GroupChatView : ContentView
{
	public GroupChatView()
	{
		InitializeComponent();
	}

    public GroupChatView(int groupId)
    {
		InitializeComponent();
		
		AddMessages(groupId);
    }

    private async Task AddMessages(int groupId)
    {
        List<Dictionary<string, string>> messages = await ProfileSingleton.GetInstance().GetMessages(groupId);
        if (messages != null)
        {
            foreach(var message in messages)
            {
                HorizontalStackLayout stack = new HorizontalStackLayout();
                var lb = new Label();
                DateTime time = Convert.ToDateTime(message["timestamp"]).ToLocalTime();
                lb.Text = time.ToShortTimeString();
                stack.Add(lb);
                lb = new Label();
                lb.Text = " "+message["sender"] + ":";
                stack.Add(lb);
                lb = new Label();
                lb.Text = message["message"];
                stack.Add(lb);

                messageBox.Add(stack);
            }
        }
    }
}