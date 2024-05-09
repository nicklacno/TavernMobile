using CommunityToolkit.Maui.Views;

namespace Tavern;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}
	
	/** 
	 * Main method that calls the singleton to get data from the api
	 */
    private async void AttemptLogin(object sender, EventArgs e)
    {
		bool success = await ProfileSingleton.GetInstance().Login(txtUsername.Text, txtPassword.Text); //calls singleton to login using text boxes
		if (success) 
		{
			ProfileSingleton.GetInstance().switchMainPage(new NavigationPage(new TabbedMainPage())); //if successful, switch base page to main page
		}
        else
        {
			var popup = new ErrorPopup("Failed to Login To Tavern");
			this.ShowPopup(popup);
        }
    }

	private async void Register(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new RegisterPage());
	}
}