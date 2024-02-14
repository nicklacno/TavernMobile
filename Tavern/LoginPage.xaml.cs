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
		await ProfileSingleton.GetInstance().Login(txtUsername.Text, txtPassword.Text);
    }
}