using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Tavern;

public partial class RequestsPage : ContentPage
{
	ObservableCollection<RequestByGroup> groupRequests = new ObservableCollection<RequestByGroup>();

	ObservableCollection<Request> friendRequests = new ObservableCollection<Request>();

	public RequestsPage()
	{
		InitializeComponent();
		BindingContext = this;
		//myFriendRequestsStack.ItemsSource = myFriendRequests;
		//myGroupRequestsStack.ItemsSource= myGroupRequests;
		requestStack.ItemsSource = groupRequests;
		friendRequestStack.ItemsSource = friendRequests;
		
		Task t = Task.Run(async () => { await UpdateRequests(); });
		t.Wait();
	}

	public async Task UpdateRequests()
	{
		var singleton = ProfileSingleton.GetInstance();

		groupRequests.Clear();
		friendRequests.Clear();
		var incomingRequests = await singleton.GetAllRequests();
		if (incomingRequests.Count > 0)
		{
			if (incomingRequests.First().GroupId == -1)
			{
				foreach (var request in incomingRequests.First())
				{
					friendRequests.Add(request);
				}
				incomingRequests.RemoveAt(0);
			}
			foreach (var incoming in incomingRequests)
			{
				groupRequests.Add(incoming);
			}
		}
		Debug.WriteLine(groupRequests.Count);

	}

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
		await UpdateRequests();
    }

    private async void UserSelected(object sender, SelectionChangedEventArgs e)
    {
		object other = requestStack.SelectedItem;
		requestStack.SelectedItem = null;

		if (other is Request r)
		{
			string result = await DisplayActionSheet($"What would you like to do with {r.UserName}?", "Cancel", null, "Accept", "Reject", 
				"View Profile");
			Debug.WriteLine(result);
			if (result.Equals("Cancel")) return;
			else if (result.Equals("View Profile"))
			{
				var otherPage = new OtherUserPage(r.UserId);
				await Navigation.PushAsync(otherPage);
				return;
			}

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
				await ShowErrorMessage("Failed to Process Request");
				return;
			}
			await singleton.SetValues();
			await UpdateRequests();

			await ShowErrorMessage("Successfully Processed Request", "Success");
		}
    }

    public async Task ShowErrorMessage(string message, string title = "An Error Occurred")
    {
        await DisplayAlert(title, message, "Okay");
    }
}