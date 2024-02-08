using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Tavern;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
		TryGetData();
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

			}
			Name.Text = singleton.ProfileName;
			Bio.Text = singleton.ProfileBio;
		}
		catch (Exception ex)
		{
			await Navigation.PushAsync(new ErrorPage(ex.Message));
		}
	}
}