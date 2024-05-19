

using CommunityToolkit.Maui.Views;

namespace Tavern;

public partial class GroupPage : ContentPage
{
	public Group GroupData { get; set; }
	GroupChatView ChatView { get; set; }
	public bool Updating { get; set; }

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
		Updating = true;

		ChatView = new GroupChatView(GroupData.GroupId, this);
		GroupChat.Add(ChatView);
		Task.Run(BackgroundUpdate);
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

			layoutTags.ItemsSource = GroupData.Tags;

			layoutMembers.ItemsSource = GroupData.Members;

			if (GroupData.OwnerId == ProfileSingleton.GetInstance().ProfileId)
			{
				ModifyButton.Text = "Edit Group";
				ModifyButton.Clicked += PushEditGroupPage;
			}
			else
			{
				ModifyButton.Text = "Leave Group";
			}
		}
    }

    private async void PushEditGroupPage(object sender, EventArgs e)
    {
		await Navigation.PushAsync(new EditGroupPage(GroupData));
    }

    public void ShowErrorMessage(string message)
    {
        var popup = new ErrorPopup(message);
        this.ShowPopup(popup);
    }

	public async Task BackgroundUpdate()
	{
		while (Updating)
		{
			await ChatView.RetrieveNewMessages();
			//await UpdatePage();

			Thread.Sleep(2000);
		}
	}
}