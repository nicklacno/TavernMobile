using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Tavern.PrivateMessage;

public partial class PrivateMessageNavigationPage : ContentPage
{
	ObservableCollection<OtherUser> friends = new ObservableCollection<OtherUser>();

	public PrivateMessageNavigationPage()
	{
		InitializeComponent();
		friendsList.ItemsSource = ProfileSingleton.GetInstance().Friends;
    }

  //  protected override void OnNavigatedTo(NavigatedToEventArgs args)
  //  {
  //      base.OnNavigatedTo(args);
		//friends.Clear();
		//foreach (var friend in ProfileSingleton.GetInstance().Friends)
		//{
		//	friends.Add(friend);
		//}
  //  }


    private async void ChatSelected(object sender, SelectionChangedEventArgs e)
	{
		object selected = friendsList.SelectedItem;
		friendsList.SelectedItem = null;
		if (selected is OtherUser o)
		{
			Profile profile = await ProfileSingleton.GetInstance().GetProfile(o.Id);
			if (profile == null)
			{
				if (!friends.Remove(o))
				{
					foreach (var friend in friends)
					{
						if (friend.Id == o.Id)
						{
							friends.Remove(friend);
							return;
						}
					}
				}
				return;
			}
			await Navigation.PushAsync(new OtherUserPage(profile));
		}
	}
}