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
		tagsList = await singleton.GetProfileTags();
		tagList.ItemsSource = tagsList;
		entryUsername.Text = singleton.ProfileName;
		entryBio.Text = singleton.ProfileBio;

		foreach (Tag tag in singleton.Tags)
		{
			tagList.SelectedItems.Add(tag);
		}
	}

	public async void Update(object sender, EventArgs e)
	{
		ProfileSingleton singleton = ProfileSingleton.GetInstance();
        int val = await singleton.EditProfile(entryPassword.Text, entryUsername.Text, entryBio.Text);
		if (val == -1)
		{
			//error handle
			await ShowErrorMessage("Failed to connect to server");
		}
		else if (val == -2)
		{
			//error handling
			await ShowErrorMessage("Incorrect Password");
		}
		else if (val == -3)
		{ 
			//error handling
			await ShowErrorMessage("Duplicate username");
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
    private async Task ShowErrorMessage(string message)
    {
        await DisplayAlert("An Error Occurred", message, "Okay");
    }

	public async void UpdateTags(object sender, EventArgs e)
	{
		var singleton = ProfileSingleton.GetInstance();
		Dictionary<int, bool> updatedStatus = new Dictionary<int, bool>();
		foreach (var tag in tagsList)
		{
			updatedStatus.Add(tag.Id, tagList.SelectedItems.Contains(tag));
		}
		//tagList.SelectedItems.Clear();
		int val = await singleton.UpdateProfile(updatedStatus);

		await ShowErrorMessage(val == 0 ? "Successfully Updated Tags" : "Failed to Update Tags");
	}
}