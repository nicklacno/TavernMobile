
using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;

namespace Tavern;

public partial class EditGroupPage : ContentPage
{
    public int Id { get; set; }
    public ObservableCollection<Request> requests { get; set; }

    public EditGroupPage(int id)
	{
		InitializeComponent();
		Id = id;
        GetRequests();
	}

    private async Task GetRequests()
    {
        var singleton = ProfileSingleton.GetInstance();
        requests = await singleton.GetGroupRequests(Id);
        requestStack.ItemsSource = requests;
    }

    public async void AcceptUsers(object sender, EventArgs e)
    {
        if (requestStack.SelectedItems.Count < 0) return;
        int result = await ProfileSingleton.GetInstance().AcceptMembers(requestStack.SelectedItems);
        if (result == -1) ShowErrorMessage("An Error has occurred, Try again later");
        else ProcessStatus(result);
    }

    public async void RejectUsers(object sender, EventArgs e)
    {
        if (requestStack.SelectedItems.Count < 0) return;
        int result = await ProfileSingleton.GetInstance().RejectMembers(requestStack.SelectedItems);
        if (result == -1) ShowErrorMessage("An error has occurred, Try again later");
        else ProcessStatus(result);
    }

    private void ProcessStatus(int status)
    {
        if (status > 0)
        {
            var errored = requestStack.SelectedItems.ElementAt(status - 1);
            foreach (var user in requestStack.SelectedItems)
            {
                if (user == errored) return;
                else
                {
                    requestStack.SelectedItems.Remove(user);
                    requests.Remove((Request)user);
                }
            }
            requestStack.ItemsSource = requests;
        }
        else
        {
            foreach (var user in requestStack.SelectedItems)
            {
                requestStack.SelectedItems.Remove(user);
                requests.Remove((Request)user);
            }
            requestStack.ItemsSource = requests;
        }
    }

    public void ShowErrorMessage(string message)
    {
        var popup = new ErrorPopup(message);
        this.ShowPopup(popup);
    }
}