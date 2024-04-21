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
		int id = await ProfileSingleton.GetInstance().CreateGroup(txtGroupName.Text, txtBio.Text);
		if (id > 0)
		{
			ProfileSingleton singleton = ProfileSingleton.GetInstance();
			singleton.Groups.Add(await singleton.GetGroup(id));
			await Navigation.PopAsync();
		}
		else if (id == -1)
		{
			//error handle
			Debug.WriteLine("Server connection error");
		}
		else if (id == -2)
		{
			//error handle
			Debug.WriteLine("Duplicate Group name");
		}
    }
}