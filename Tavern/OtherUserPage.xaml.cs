using Tavern.PrivateMessage;
namespace Tavern;

public partial class OtherUserPage : ContentPage
{
	Profile ProfileData { get; set; }

	PrivateChatView chat { get; set; }
    public bool Updating { get; set; }

    public OtherUserPage(Profile data)
	{
		InitializeComponent();
		ProfileData = data;
		Title = ProfileData.ProfileName;
		Updating = true;
		PopulateData();

		Task.Run(BackgroundUpdate);
	}

	public OtherUserPage(int id)
	{
		InitializeComponent();
		ShowPreview(id);
	}

	private async void ShowPreview(int id)
	{
		ProfileSingleton singleton = ProfileSingleton.GetInstance();
		ProfileData = await singleton.GetProfile(id);

		PopulateData();
		btnRequest.Text = "";
		btnRequest.IsEnabled = false;
	}

	public void PopulateData()
	{
		Name.Text = ProfileData.ProfileName;
		Bio.Text = ProfileData.ProfileBio == null ? "" : ProfileData.ProfileBio;
		tagList.ItemsSource = ProfileData.Tags;
		groupList.ItemsSource = ProfileData.Groups;

		ProfileSingleton singleton = ProfileSingleton.GetInstance();
		bool isFriend = false;
		foreach (var friend in singleton.Friends)
		{
			if (ProfileData.ProfileId == friend.Id)
			{
				isFriend = true;
				break;
			}
		}
		if (!isFriend)
		{
			btnRequest.Clicked += SendFriendRequest;
			return;
		}
		else
		{
			btnRequest.Text = "Remove Friend";
			chat = new PrivateChatView(ProfileData.ProfileId, this);
			PrivateChat.Add(chat);
		}
    }

    private async void SendFriendRequest(object sender, EventArgs e)
    {
		int ret = await ProfileSingleton.GetInstance().SendFriendRequest(ProfileData.ProfileId);
		switch (ret)
		{
			case 0:
				btnRequest.Text = "Request Sent";
				btnRequest.IsEnabled = false;
				await ShowErrorMessage("Successfully Send Friend Request", "Success");
				break;
			case -1:
				await ShowErrorMessage("Error connecting to server");
				break;
		}
	}
	public async Task ShowErrorMessage(string message, string title = "An Error Occurred")
    {
        await DisplayAlert(title, message, "Okay");
    }
    protected override bool OnBackButtonPressed()
    {
		Updating = false;
        return base.OnBackButtonPressed();
    }
    public async Task BackgroundUpdate()
    {
        while (Updating)
        {
            await chat.RetrieveNewMessages();
            //await UpdatePage();

            Thread.Sleep(2000);
        }
    }
}