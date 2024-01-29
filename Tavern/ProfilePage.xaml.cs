using System.Diagnostics;

namespace Tavern;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
		GetProfile();
	}

	public async void GetProfile()
	{
		string json = await ProfileSingleton.GetInstance().GetProfileData(1);
		Debug.WriteLine("Cardboard");
		Test.Text = json;
	}
}