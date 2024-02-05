using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Tavern;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
		//GetProfile();
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
}