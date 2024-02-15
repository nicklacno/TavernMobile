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
			ProfileSingleton.GetInstance().loginSuccessful.Invoke(); //if successful, switch base page to main page
		}
    }
}