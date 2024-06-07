
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Tavern;

public partial class EditGroupPage : ContentPage
{
    public Group GroupData { get; set; }
    public ObservableCollection<Request> requests { get; set; }

    public ObservableCollection<Tag> tags { get; set; }

    public EditGroupPage(in Group data)
	{
		InitializeComponent();
		GroupData = data;
        checkPrivate.IsChecked = GroupData.IsPrivate;
        //GetRequests();
        Task t = Task.Run(async () => { await PopulateDataFields(); });
        t.Wait();
        Debug.WriteLine("Pushed Edit Group Page");
	}

    private async Task PopulateDataFields()
    {
        var singleton = ProfileSingleton.GetInstance();
        tags = await singleton.GetGroupTags();
        GroupTagsList.ItemsSource = tags;

        foreach (var tag in GroupData.Tags)
        {
            GroupTagsList.SelectedItems.Add(tag);
        }

        entryUsername.Text = GroupData.Name;
        entryBio.Text = GroupData.Bio;
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
        if (val == 0)
        {
            await ShowErrorMessage("Successfully Updated Tags", "Success");
            GroupData.Tags.Clear();
            foreach (var tag in GroupTagsList.SelectedItems)
            {
                if (tag is Tag t)
                {
                    GroupData.Tags.Add(t);
                }
            }
        }
        else
            await ShowErrorMessage("Failed to Update Tags");
    }

    public async void UpdateGroupInfo(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(entryUsername.Text))
        {
            await ShowErrorMessage("Cannot have blank Username");
            return;
        }
        string name = entryUsername.Text.Equals(GroupData.Name) ? null : entryUsername.Text;
        string bio = entryBio.Text.Equals(GroupData.Bio) ? null : entryBio.Text;
        bool? isPrivate = checkPrivate.IsChecked == GroupData.IsPrivate ? null : checkPrivate.IsChecked;


        var retval = await ProfileSingleton.GetInstance().UpdateGroupData(GroupData.GroupId, name, bio, isPrivate);
        switch (retval)
        {
            case -10:
                await ShowErrorMessage("Group does not exist");
                await Navigation.PopToRootAsync();
                break;
            case -3:
                await ShowErrorMessage("Duplicate group name");
                break;
            case -5:
                await ShowErrorMessage("Only the owner can edit the group");
                await Navigation.PopAsync();
                break;
            case -1:
                await ShowErrorMessage("An error had occurred, Try again later");
                break;
            case 0:
                await ShowErrorMessage("Successfully updated group", "Success");
                break;
            default:
                await ShowErrorMessage("A unexpected errror had occurred");
                break;
        }

    }

    public async void KickMembers(object sender, EventArgs e)
    {

    }

    public async void BanMembers(object sender, EventArgs e)
    {

    }

    public async void RetrieveCode(object sender, EventArgs e)
    {
        string code = await ProfileSingleton.GetInstance().GetGroupCode(GroupData.GroupId);
        if (GroupData.GroupCode == null) GroupData.GroupCode = code;
        await DisplayAlert("Private Code", $"Your group code is {code}", "Ok");
    }

    public async Task ShowErrorMessage(string message, string title = "An Error Occurred")
    {
        await DisplayAlert(title, message, "Okay");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Debug.WriteLine("Popped Edit Grou Page");
    }
}