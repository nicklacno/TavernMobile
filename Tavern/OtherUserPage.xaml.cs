using Tavern.PrivateMessage;
namespace Tavern;

public partial class OtherUserPage : ContentPage
{
	Profile ProfileData { get; set; }

	PrivateChatView chat { get; set; }

	public OtherUserPage(Profile data)
	{
		InitializeComponent();
		ProfileData = data;
		PopulateData();
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
}