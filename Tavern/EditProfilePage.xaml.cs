using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Tavern;

public partial class EditProfilePage : ContentPage
{
	public ObservableCollection<Tag> tagsList { set; get; } = new ObservableCollection<Tag>();

	private ProfilePage ElementUnder {  get; set; }
	
    public EditProfilePage(ProfilePage element)
	{
		InitializeComponent();
		ElementUnder = element;
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

		var pfps = singleton.GetAllPFPs();
		profilePics.ItemsSource = pfps;
		
		foreach (var pfp in pfps)
		{
			if (pfp.ImageId == singleton.ImageID)
			{
				profilePics.SelectedItem = pfp;
				break;
			}
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

		await ElementUnder.UpdateProfile();
	}

	public void Logout(object sender, EventArgs e)
	{
		ProfileSingleton.GetInstance().Logout();
	}
    private async Task ShowErrorMessage(string message, string title = "An Error Occurred")
    {
        await DisplayAlert(title, message, "Okay");
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

		if (val == 0)
		{
			await ShowErrorMessage("Successfully Updated Tags", "Success");
		}
        else
        {
			await ShowErrorMessage("There was an Error Updating Tags");
        }

        await ElementUnder.UpdateProfile();
	}

	public async void UpdatePFP(object sender, EventArgs e)
	{
		var singleton = ProfileSingleton.GetInstance();
		if (profilePics.SelectedItem == null) return;
		
		if (profilePics.SelectedItem is PFP p)
		{
			if (p.ImageId == singleton.ImageID) return;
			int ret = await singleton.UpdateProfilePic(p.ImageId);
			if (ret == 0)
			{
				singleton.ImageID = p.ImageId;
				await ShowErrorMessage("Successfully Updated Profile Avatar", "Success");
				await ElementUnder.UpdateProfile();
			}
			else
			{
				await ShowErrorMessage("There was an Error Updating your Profile Avatar");
			}
		}
	}
}