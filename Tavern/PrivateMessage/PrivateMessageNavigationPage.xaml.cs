using System.Collections.ObjectModel;

namespace Tavern.PrivateMessage;

public partial class PrivateMessageNavigationPage : ContentPage
{
	ObservableCollection<OtherUser> friends = new ObservableCollection<OtherUser>();

	public PrivateMessageNavigationPage()
	{
		InitializeComponent();
		friendsList.ItemsSource = friends;
		foreach (var friend in ProfileSingleton.GetInstance().Friends)
		{
			friends.Add(friend);
		}
	}

    private void ChatSelected(object sender, SelectionChangedEventArgs e)
    {
		if (friendsList.SelectedItem is OtherUser o)
		{
			
		}
		friendsList.SelectedItems.Clear();
    }
}