using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Tavern;

public partial class EditProfilePage : ContentPage
{
	public ObservableCollection<Tag> tagsList { set; get; } = new ObservableCollection<Tag>();
	public EditProfilePage()
	{
		InitializeComponent();
		PopulateEntryFields();
	}

	public async void PopulateEntryFields()
	{
		ProfileSingleton singleton = ProfileSingleton.GetInstance();
		tagsList = await singleton.GetPlayerTags();
		tagList.ItemsSource = tagsList;
		entryUsername.Text = singleton.ProfileName;
		entryBio.Text = singleton.ProfileBio;
	}

	public async void Update(object sender, EventArgs e)
	{
		ProfileSingleton singleton = ProfileSingleton.GetInstance();
        int val = await singleton.EditProfile(entryPassword.Text, entryUsername.Text, entryBio.Text);
		if (val == -1)
		{
			//error handle
			ShowErrorMessage("Failed to connect to server");
		}
		else if (val == -2)
		{
			//error handling
			ShowErrorMessage("Incorrect Password");
		}
		else if (val == -3)
		{ 
			//error handling
			ShowErrorMessage("Duplicate username");
		}
		else
		{
			singleton.ProfileName = entryUsername.Text;
			singleton.ProfileBio = entryBio.Text;
			entryPassword.Text = "";
			//await Navigation.PopAsync();
		}
	}

	public void Logout(object sender, EventArgs e)
	{
		ProfileSingleton.GetInstance().Logout();
	}
    private void ShowErrorMessage(string message)
    {
        var popup = new ErrorPopup(message);
        this.ShowPopup(popup);
    }
}