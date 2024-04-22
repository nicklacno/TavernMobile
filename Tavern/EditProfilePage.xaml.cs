using System.Diagnostics;

namespace Tavern;

public partial class EditProfilePage : ContentPage
{
	public EditProfilePage()
	{
		InitializeComponent();
		PopulateEntryFields();
	}

	public void PopulateEntryFields()
	{
		ProfileSingleton singleton = ProfileSingleton.GetInstance();
		entryUsername.Text = singleton.ProfileName;
		entryBio.Text = singleton.ProfileBio;
	}

	public async void Update(object sender, EventArgs e)
	{
         int val = await ProfileSingleton.GetInstance().EditProfile(entryPassword.Text, entryUsername.Text, entryBio.Text);
		if (val == -1)
		{
			//error handle
			Debug.WriteLine("Failed to connect to server");
		}
		else if (val == -2)
		{
			//error handling
			Debug.WriteLine("Incorrect Password");
		}
		else if (val == -3)
		{ 
			//error handling
			Debug.WriteLine("Duplicate username");
		}
		else
		{
			ProfileSingleton.GetInstance().updateProfile.Invoke();
			await Navigation.PopAsync();
		}
	}
}