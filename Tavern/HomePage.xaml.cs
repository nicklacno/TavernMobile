namespace Tavern;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
		//AddGroupsToHomePage();

	}

	public async Task AddGroupsToHomePage()
	{
		ProfileSingleton singleton = ProfileSingleton.GetInstance();//temporarily putting 5 for testing
		await singleton.GetGroupsList();
		layoutGroup.Children.Clear();
		foreach (Group group in singleton.Groups)
		{
			GroupCard groupCard = new GroupCard(group);
			foreach (var child in groupCard.Children)
			{
				if (child is Label label)
				{
					label.FontFamily = "Sedan";
				}
			}
			layoutGroup.Children.Add(groupCard);
		}
	}

    private void NavigateToSwipe(object sender, EventArgs e)
    {
		Navigation.PushAsync(new SwipingFunctionality.SwipingPage());
    }


    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        layoutGroup.Children.Clear();
        foreach(Group group in ProfileSingleton.GetInstance().Groups)
		{
			layoutGroup.Children.Add(new GroupCard(group));
		}
    }

	private void NavigateToCreate(object sender, EventArgs e)
	{
		Navigation.PushAsync(new CreateGroupPage());
	}
}