using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Tavern;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
		TryGetData();
		//TempData();

		ProfileSingleton.GetInstance().updateProfile += UpdateProfile;
	}

	public async void GetProfile()
	{
		string json = await ProfileSingleton.GetInstance().GetProfileData(1);
		if (json != null)
		{
			JObject profile = JObject.Parse(json);
			Name.Text = (string)profile["name"];
			Bio.Text = (string)profile["bio"];
		}
		
	}

	public async void TryGetData()
	{
		ProfileSingleton singleton = ProfileSingleton.GetInstance();
		try
		{
			string json = await singleton.GetProfileData(5);
			if (json != null)
			{
				JObject profile = JObject.Parse(json);
				singleton.ProfileName = (string)profile["name"];
				singleton.ProfileBio = (string)profile["bio"];
				UpdateProfile();
			}
			else
			{
				throw new Exception("Data not found");
			}
		}
		catch (Exception ex)
		{
			//await singleton.ShowError(ex.Message);
		}
	}

	public void TempData()
	{
		ProfileSingleton singleton = ProfileSingleton.GetInstance();
		singleton.ProfileName = "Admin";
		singleton.ProfileBio = "This is a test profile text\nChangable";

		UpdateProfile();
	}

	public async void EditProfile(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new EditProfilePage());
	}

	public void UpdateProfile()
	{
        ProfileSingleton singleton = ProfileSingleton.GetInstance();
        Name.Text = singleton.ProfileName;
        Bio.Text = singleton.ProfileBio;
    }
}