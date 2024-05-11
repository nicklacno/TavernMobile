using CommunityToolkit.Maui.Views;
using System.Diagnostics;

namespace Tavern;

public partial class EditProfilePage : ContentPage
{
	public EditProfilePage()
	{
		InitializeComponent();
		PopulateEntryFields();
        //tagList.ItemsSource = ProfileSingleton.GetInstance().GetPlayerTags().Result;
	}

	public void PopulateEntryFields()
	{
		ProfileSingleton singleton = ProfileSingleton.GetInstance();
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