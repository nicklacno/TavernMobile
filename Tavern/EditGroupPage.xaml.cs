
using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;

namespace Tavern;

public partial class EditGroupPage : ContentPage
{
    public Group GroupData { get; set; }
    public ObservableCollection<Request> requests { get; set; }

    public ObservableCollection<Tag> tags { get; set; }

    public EditGroupPage(Group data)
	{
		InitializeComponent();
		GroupData = data;
        //GetRequests();
        PopulateDataFields();
	}

    //private async Task GetRequests()
    //{
    //    var singleton = ProfileSingleton.GetInstance();
    //    requests = await singleton.GetGroupRequests(Id);
    //    requestStack.ItemsSource = requests;
    //}

    private async void PopulateDataFields()
    {
        var singleton = ProfileSingleton.GetInstance();
        tags = await singleton.GetGroupTags();
        GroupTagsList.ItemsSource = tags;

        foreach (var tag in GroupData.Tags)
        {
            GroupTagsList.SelectedItems.Add(tag);
        }
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
        var newList = new ObservableCollection<Request>(requests);
        if (status > 0)
        {
            var errored = requestStack.SelectedItems.ElementAt(status - 1);
            foreach (var user in requestStack.SelectedItems)
            {
                if (user == errored) return;
                else
                {
                    newList.Remove((Request)user);
                }
            }
            requestStack.SelectedItems.Clear();
            requests = newList;
            requestStack.ItemsSource = requests;
        }
        else
        {
            foreach (var user in requestStack.SelectedItems)
            {
                newList.Remove((Request)user);
            }
            requestStack.SelectedItems.Clear();
            requests = newList;
            requestStack.ItemsSource = requests;
        }
    }

    public async void UpdateTagsList(object sender, EventArgs e)
    {
        Dictionary<int, bool> updatedValues = new Dictionary<int, bool>();
        foreach (var tag in GroupTagsList.SelectedItems)
        {
            if (tag is Tag t)
            {
                updatedValues[t.Id] = true;
            }
        }

        int val = await ProfileSingleton.GetInstance().UpdateGroupTags(GroupData,  updatedValues);
        ShowErrorMessage(val == 0 ? "Successfully Updated Tags" : "Failed to Update Tags");
    }

    public async void UpdateGroupInfo(object sender, EventArgs e)
    {

    }

    public async void KickMembers(object sender, EventArgs e)
    {

    }

    public async void BanMembers(object sender, EventArgs e)
    {

    }


    public void ShowErrorMessage(string message)
    {
        var popup = new ErrorPopup(message);
        this.ShowPopup(popup);
    }
}