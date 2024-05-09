using System.Diagnostics;

namespace Tavern;

public partial class HomePage : ContentPage
{
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
			//layoutGroup.Children.Add(new GroupCard(group));
		}
	}

    private void NavigateToSwipe(object sender, EventArgs e)
    {
		Navigation.PushAsync(new SwipingFunctionality.SwipingPage());
    }


  //  protected override void OnNavigatedTo(NavigatedToEventArgs args)
  //  {
  //      base.OnNavigatedTo(args);
  //      //layoutGroup.Children.Clear();
  //      foreach(Group group in ProfileSingleton.GetInstance().Groups)
		//{
		//	//layoutGroup.Children.Add(new GroupCard(group));
		//}
  //  }

	private void NavigateToCreate(object sender, EventArgs e)
	{
		Navigation.PushAsync(new CreateGroupPage());
    }

    private void GroupSelected(object sender, SelectionChangedEventArgs e)
    {
		Group selected = (Group)groupList.SelectedItem;
		groupList.SelectedItem = null;
		if (selected != null)
		{
			Navigation.PushAsync(new GroupPage(selected));
		}
    }
}