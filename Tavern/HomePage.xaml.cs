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

	public async Task AddGroupsToHomePage()
	{
		ProfileSingleton singleton = ProfileSingleton.GetInstance();//temporarily putting 5 for testing
		await singleton.GetGroupsList();
		//layoutGroup.Children.Clear();
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
			//layoutGroup.Children.Add(new GroupCard(group))
		}
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
		group.Updating = false;
		group = null;
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