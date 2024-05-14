using System.Diagnostics;

namespace Tavern;

public partial class HomePage : ContentPage
{
	GroupPage group;

	public HomePage()
	{
		InitializeComponent();
		//AddGroupsToHomePage();
		groupList.ItemsSource = ProfileSingleton.GetInstance().Groups;

	}


    private void NavigateToSwipe(object sender, EventArgs e)
    {
		Navigation.PushAsync(new SwipingFunctionality.SwipingPage());
    }

	private void NavigateToCreate(object sender, EventArgs e)
	{
		Navigation.PushAsync(new CreateGroupPage());
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
		if (group != null)
		{
			group.Updating = false;
			group = null;
		}
    }

    private void GroupSelected(object sender, SelectionChangedEventArgs e)
    {
		Group selected = (Group)groupList.SelectedItem;
		groupList.SelectedItem = null;
		if (selected != null)
		{
			group = new GroupPage(selected);
			Navigation.PushAsync(group);
		}
    }
}