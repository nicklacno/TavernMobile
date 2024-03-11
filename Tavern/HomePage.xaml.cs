namespace Tavern;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
		AddGroupsToHomePage();
	}

	public async Task AddGroupsToHomePage()
	{
		ProfileSingleton singleton = ProfileSingleton.GetInstance();//temporarily putting 5 for testing
		await singleton.GetGroupsList();
		foreach (Group group in singleton.Groups)
		{
			layoutGroup.Children.Add(new GroupCard(group));
		}
	}

    private void NavigateToSwipe(object sender, EventArgs e)
    {
		Navigation.PushAsync(new SwipingFunctionality.SwipingPage());
    }
}