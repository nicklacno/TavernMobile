

using CommunityToolkit.Maui.Views;

namespace Tavern;

public partial class GroupPage : ContentPage
{
	public Group GroupData { get; set; }
	GroupChatView ChatView { get; set; }

	public GroupPage(int id)
	{
		InitializeComponent();
		UpdatePage(id).RunSynchronously();
	}

	public GroupPage(Group data)
	{
		InitializeComponent();
		GroupData = data;
		UpdatePage();

		ChatView = new GroupChatView(GroupData.GroupId, this);
		GroupChat.Add(ChatView);
	}

	/**
	 * UpdatePage - Helper function to put all the data onto the screen
	 * @param group - the data being shown to the user
	 */
    private async Task UpdatePage(int id = -1)
    {
		if (id > 0)
			GroupData = await ProfileSingleton.GetInstance().GetGroup(id);

        if (GroupData != null)
		{
			Title = GroupData.Name;
			lbGroupName.Text = GroupData.Name;
			lbGroupBio.Text = GroupData.Bio;

			Label lb;

			foreach (Tag tag in GroupData.Tags) 
			{
				lb = new Label();
				lb.Text = tag.Name;
				layoutTags.Add(lb);
			}

			lb = new Label();
			lb.FontSize = 20;
			lb.Text = "* " + GroupData.Members[0] + " *";
			layoutMembers.Add(lb);

			for (int i = 1; i < GroupData.Members.Count; i++)
			{
				lb = new Label();
				lb.FontSize = 18;
				lb.Text = GroupData.Members[i];
				layoutMembers.Add(lb);
			}

			ShowEditGroup();
		}
    }

	/**
	 * Show Exit Button - Shows the edit group button if it is the ownner of the group
	 */
	private void ShowEditGroup()
	{
		int id = ProfileSingleton.GetInstance().ProfileId;
		if (id > 0 && id == GroupData.OwnerId)
		{
			Button btn = new Button();
			btn.Text = "Edit Group";
			btn.Clicked += PushEditGroupPage;

			stackInfo.Insert(4, btn);
		}
	}

    private async void PushEditGroupPage(object sender, EventArgs e)
    {
		await Navigation.PushAsync(new EditGroupPage(GroupData.GroupId));
    }
    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
		ChatView.isRetrievingMessages = false;
    }
    public void ShowErrorMessage(string message)
    {
        var popup = new ErrorPopup(message);
        this.ShowPopup(popup);
    }
}