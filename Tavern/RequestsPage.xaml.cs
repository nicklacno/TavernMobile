using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;

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
        base.OnNavigatedTo(args);
        Task t = Task.Run(async () => { await UpdateRequests(); });
        t.Wait();
    }

    private async void UserSelected(object sender, SelectionChangedEventArgs e)
    {
		object other = requestStack.SelectedItem;
		requestStack.SelectedItem = null;

		if (other is Request r)
		{
			string result = await DisplayActionSheet($"What would you like to do with {r.UserName}?", "Cancel", null, "Accept", "Reject");
			//Debug.WriteLine(result);
			if (result.Equals("Cancel")) return;

			bool isAccepted = result.Equals("Accept");
			

			var singleton = ProfileSingleton.GetInstance();
			int ret;

			if (r.RequestId == -1)
			{
				//Process Friend Request
				ret = await singleton.ModifyFriendRequest(r.UserId, isAccepted);
			}
			else
			{
				if (isAccepted) ret = await singleton.AcceptMember(r);
				else ret = await singleton.RejectMember(r);
			}
			if (ret != 0)
			{
				ShowErrorMessage("Failed to Process Request");
				return;
			}
			foreach (var rbg in requests)
			{
				if (rbg.Remove(r))
				{
					if (rbg.Count == 0) requests.Remove(rbg);
					break;
				}
			}
			ShowErrorMessage("Successfully Processed Request");
		}
    }

    public void ShowErrorMessage(string message)
    {
        var popup = new ErrorPopup(message);
        this.ShowPopup(popup);
    }
}