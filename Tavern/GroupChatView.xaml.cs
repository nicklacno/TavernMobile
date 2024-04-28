
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
    }
}