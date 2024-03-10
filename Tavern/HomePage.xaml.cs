namespace Tavern;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
		ProfileSingleton.GetInstance(5).GetGroupsList();
	}
}