using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls.Shapes;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.Json;

namespace Tavern;

public partial class ProfilePage : ContentPage
{
	/**
	 * ProfilePage - constructor for the personal profile page
	 */
	public ProfilePage()
	{
		InitializeComponent();
		UpdateProfile();

		ProfileSingleton.GetInstance().updateProfile += UpdateProfile; //adds to delegate
		groupList.ItemsSource = ProfileSingleton.GetInstance().Groups;
	}
	/**
	 * TryGetData - Attempts to get the data for the profile from the database using the singleton
	 * !!! Depricated Method
	 */
	public async void TryGetData()
	{
		ProfileSingleton singleton = ProfileSingleton.GetInstance(); //gets singleton
		try
		{
			string json = await singleton.GetProfileData(5);
			if (json != null)
			{
				JObject profile = JObject.Parse(json); //parses the json
				singleton.ProfileName = (string)profile["name"]; //gets the name of the profile
				singleton.ProfileBio = (string)profile["bio"]; //gets the bio for the profile
				
				UpdateProfile(); //calls the update profile page
			}
			else //throws exception for catch statement
			{
				throw new Exception("Data not found");
			}
		}
		catch (Exception ex) 
		{
			this.ShowPopup(new ErrorPopup(ex.Message, true));
		}
	}
	/**
	 * EditProfile - Helper method that pushes the EditProfilePage onto the Navigation Page stack
	 */
	public async void EditProfile(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new EditProfilePage()); //push onto stack
	}

	/**
	 * UpdateProfile - Method that updates changes to the profile
	 */
	public async void UpdateProfile()
	{
        ProfileSingleton singleton = ProfileSingleton.GetInstance();
        Name.Text = singleton.ProfileName;//sets profile name
        Bio.Text = singleton.ProfileBio;//sets profile bio

		//Need to add updating friends and groups !!!
    }
}