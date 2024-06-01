namespace Tavern;

public partial class OtherUserPage : ContentPage
{
	Profile ProfileData { get; set; }

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
    }

    private void SendFriendRequest(object sender, EventArgs e)
    {
		
    }
}