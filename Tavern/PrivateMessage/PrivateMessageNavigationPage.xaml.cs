using System.Collections.ObjectModel;

namespace Tavern.PrivateMessage;

public partial class PrivateMessageNavigationPage : ContentPage
{
	ObservableCollection<OtherUser> friends = new ObservableCollection<OtherUser>();

	public PrivateMessageNavigationPage()
	{
		InitializeComponent();
	}
}