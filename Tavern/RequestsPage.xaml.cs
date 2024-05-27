using System.Collections.ObjectModel;

namespace Tavern;

public partial class RequestsPage : ContentPage
{
	ObservableCollection<RequestByGroup> requests = new ObservableCollection<RequestByGroup>();

	ObservableCollection<OtherUser> myFriendRequests = new ObservableCollection<OtherUser>();

	ObservableCollection<Request> myGroupRequests = new ObservableCollection<Request>();
	public RequestsPage()
	{
		InitializeComponent();

		myFriendRequestsStack.ItemsSource = myFriendRequests;
		myGroupRequestsStack.ItemsSource= myGroupRequests;
		requestStack.ItemsSource = requests;
		
		Task t = Task.Run(async () => { await UpdateRequests(); });
		t.Wait();
	}

	public async Task UpdateRequests()
	{
		var singleton = ProfileSingleton.GetInstance();

		myFriendRequests.Clear();
		var outgoingFriends = await singleton.MyFriendRequests();
		if (outgoingFriends.Count > 0)
		{
			foreach (var friend in outgoingFriends)
			{
				myFriendRequests.Add(friend);
			}
		}
		
		myGroupRequests.Clear();
		var outgoingGroups = await singleton.MyGroupRequests();
		if (outgoingGroups.Count > 0)
		{
			foreach(var group in outgoingGroups)
			{
				myGroupRequests.Add(group);
			}
		}

		requests.Clear();
		var incomingRequests = await singleton.GetAllRequests();
		if (incomingRequests.Count > 0)
		{
			foreach( var incoming in incomingRequests)
			{
				requests.Add(incoming);
			}
		}
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        Task t = Task.Run(async () => { await UpdateRequests(); });
        t.Wait();
        base.OnNavigatedTo(args);
    }
}