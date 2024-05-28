using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Tavern.PrivateMessage;

public partial class PrivateMessageNavigationPage : ContentPage
{
	ObservableCollection<OtherUser> friends = new ObservableCollection<OtherUser>();
	PrivateMessagePage messagePage = null;

	public PrivateMessageNavigationPage()
	{
		InitializeComponent();
		friendsList.ItemsSource = friends;
		foreach (var friend in ProfileSingleton.GetInstance().Friends)
		{
			friends.Add(friend);
		}
	}

	private async void ChatSelected(object sender, SelectionChangedEventArgs e)
	{
		object selected = friendsList.SelectedItem;
		friendsList.SelectedItem = null;
		if (selected is OtherUser o)
		{
			messagePage = new PrivateMessagePage(o);
			await Navigation.PushAsync(messagePage);
		}
	}
}