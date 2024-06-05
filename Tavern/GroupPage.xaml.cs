using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Tavern;

public partial class GroupPage : ContentPage
{
	public Group GroupData { get; set; }
	GroupChatView ChatView { get; set; }
	GroupAnnouncementView Announcements { get; set; }
	public bool Updating { get; set; }
	
	Task UpdateTask { get; set; }

	public bool IsAnnouncmentsShown { get; set; }

	public GroupPage(int id)
	{
		InitializeComponent();
		UpdatePage(id);
	}

	public GroupPage(Group data)
	{
		InitializeComponent();
		GroupData = data;
		UpdatePage();
		Updating = true;
		IsAnnouncmentsShown = true;
		
		UpdateTask = Task.Run(BackgroundUpdate);
		Debug.WriteLine(data.GroupId);
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

			if (GroupData.Tags != null && GroupData.Tags.Count > 0)
				layoutTags.ItemsSource = GroupData.Tags;

			layoutMembers.ItemsSource = GroupData.Members;

			bool isInGroup = false;
			bool isOwner = false;
			foreach (OtherUser m in GroupData.Members)
			{
				if (m.Id == ProfileSingleton.GetInstance().ProfileId)
				{
					isInGroup = true;
					isOwner = GroupData.OwnerId == ProfileSingleton.GetInstance().ProfileId;
					break;
				}
			}
			if (!isInGroup)
			{
				ModifyButton.Text = "Request to Join";
				ModifyButton.Clicked += SendRequest;
				layoutMembers.SelectionMode = SelectionMode.None;
				return;
			}
			else if (isOwner)
			{
				ModifyButton.Text = "Edit Group";
				ModifyButton.Clicked += PushEditGroupPage;
			}
			else
			{
				ModifyButton.Text = "Leave Group";
				ModifyButton.Clicked += LeaveGroup;
			}
			Announcements = new GroupAnnouncementView(GroupData.GroupId, this, isOwner);
			ChatView = new GroupChatView(GroupData.GroupId, this);
			GroupChat.Add(Announcements);
			chatViewBtn.Clicked += ToChat;
            chatViewBtn.Text = "View Group Chat";
        }
    }

    private async void LeaveGroup(object sender, EventArgs e)
    {
		bool r = await DisplayAlert("Leave Group", $"Are you sure you want to leave {GroupData.Name}?", "Yes", "No");
		if (r)
		{
			int ret = await ProfileSingleton.GetInstance().LeaveGroup(GroupData.GroupId);
			if (ret == 0)
			{
                ModifyButton.Text = "Request to Join";
                ModifyButton.Clicked += SendRequest;
                layoutMembers.SelectionMode = SelectionMode.None;
				GroupChat.Remove(Announcements);
				GroupChat.Remove(ChatView);
				ChatView = null;
				Announcements = null;
				chatViewBtn.IsVisible = false;
            }
		}
    }

    //  protected override void OnNavigatedTo(NavigatedToEventArgs args)
    //  {
    //      base.OnNavigatedTo(args);
    //NavigationUpdate();
    //  }

    private async Task NavigationUpdate()
	{
		GroupData = await ProfileSingleton.GetInstance().GetGroup(GroupData.GroupId);
		await UpdatePage();
	}

    private async void PushEditGroupPage(object sender, EventArgs e)
    {
		Group data = GroupData;
		if (data == null) return;
		await Navigation.PushAsync(new EditGroupPage(in data));
    }

    public async Task ShowErrorMessage(string message)
    {
        await DisplayAlert("An Error Occurred", message, "Okay");
    }

	private async void SendRequest(object sender, EventArgs e)
	{
		await SwipingFunctionality.SwipingSingleton.GetInstance().SwipeRight(GroupData);
		ModifyButton.Text = "Request Sent";
		ModifyButton.Clicked -= SendRequest;
	}

	public async Task BackgroundUpdate()
	{
		while (Updating)
		{
			if (IsAnnouncmentsShown) await Announcements.RetrieveNewMessages();
			else await ChatView.RetrieveNewMessages();

			Thread.Sleep(2000);
		}
		Debug.WriteLine("Updating Stopped");
	}

    protected override bool OnBackButtonPressed()
    {
		Updating = false;
		UpdateTask?.Wait();
        return base.OnBackButtonPressed();
    }

    private async void SelectMember(object sender, SelectionChangedEventArgs e)
    {
		ProfileSingleton singleton = ProfileSingleton.GetInstance();


		if (layoutMembers.SelectedItem is OtherUser o && o.Id != singleton.ProfileId)
		{
			Profile data = await singleton.GetProfile(o.Id);
			if (data != null)
			{
				await Navigation.PushAsync(new OtherUserPage(data));
			}
		}
		layoutMembers.SelectedItem = null;
    }




	private void ToChat(object sender, EventArgs e)
	{
		GroupChat.Remove(Announcements);
		GroupChat.Add(ChatView);
		chatViewBtn.Text = "View Announcements";
		chatViewBtn.Clicked -= ToChat;
		chatViewBtn.Clicked += ToAnnouncements;
		IsAnnouncmentsShown = false;
	}

	private void ToAnnouncements(object sender, EventArgs e)
	{
        GroupChat.Remove(ChatView);
        GroupChat.Add(Announcements);
        chatViewBtn.Text = "View Group Chat";
        chatViewBtn.Clicked -= ToAnnouncements;
        chatViewBtn.Clicked += ToChat;
		IsAnnouncmentsShown = true;
    }
}