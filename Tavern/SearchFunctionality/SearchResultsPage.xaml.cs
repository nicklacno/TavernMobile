namespace Tavern.SearchFunctionality;
using GroupList = System.Collections.ObjectModel.ObservableCollection<Tavern.Group>;

public partial class SearchResultsPage : ContentPage
{
    GroupList groupsList = new GroupList();

	public SearchResultsPage()
	{
        InitializeComponent();
	}

	public SearchResultsPage(GroupList list)
	{
		InitializeComponent();
        groupsList = list;
        groupList.ItemsSource = groupsList;
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