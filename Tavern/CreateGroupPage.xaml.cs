using System.Diagnostics;

namespace Tavern;

public partial class CreateGroupPage : ContentPage
{
	public CreateGroupPage()
	{
		InitializeComponent();
	}

	private async void CreateGroup(object sender, EventArgs e)
	{
		int id = await ProfileSingleton.GetInstance().CreateGroup(txtGroupName.Text, txtBio.Text, checkPrivate.IsChecked);
		if (id > 0)
		{
			ProfileSingleton singleton = ProfileSingleton.GetInstance();
			singleton.Groups.Add(await singleton.GetGroup(id));
			await Navigation.PopAsync();
		}
		else if (id == -1)
		{
			//error handle
			//ShowErrorMessage("Server connection error");
			await DisplayAlert("An Error Occurred", "There was an error connecting to the Server", "Okay");
		}
		else if (id == -2)
		{
			//error handle
			await DisplayAlert("Duplicate Group Name Error", "You entered a group name for a group that already exists", "Okay");
		}
    }
}