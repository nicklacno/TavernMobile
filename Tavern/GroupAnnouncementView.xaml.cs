using System.Diagnostics;
using MessageLog = System.Collections.ObjectModel.ObservableCollection<Tavern.MessageByDay>;

namespace Tavern;

public partial class GroupAnnouncementView : ContentView
{
    DateTime LatestRetrieval { get; set; }
    int GroupId { get; set; }
    GroupPage Parent { get; set; }
    MessageLog messageLog { get; set; } = new MessageLog();

    int MessageCount { get; set; }

    public GroupAnnouncementView(int groupId, GroupPage groupPage, bool isOwner = false)
    {
        InitializeComponent();
        GroupId = groupId;
        Parent = groupPage;
        if (!isOwner)
        {
            stackAnnounce.Remove(stackPost);
        }
        Task t = Task.Run(async () => await SetBase());
        t.Wait();
    }

    public async void PostAnnounement(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtMessage.Text)) return;
        var ret = await ProfileSingleton.GetInstance().PostAnnouncement(GroupId, txtMessage.Text);

        Debug.WriteLine(ret);
        if (ret != 0)
        {
            await Parent.DisplayAlert("An Error Occurred", "Failed to Post the Announcement", "Okay");
        }
        txtMessage.Text = "";
    }

    public async Task SetBase()
    {
        ProfileSingleton singleton = ProfileSingleton.GetInstance();
        messageLog = await singleton.GetAnnouncements(GroupId, null);
        if (messageLog == null) messageLog = new MessageLog();
        MessageCount = messageLog.Count;
        if (messageLog.Count > 0 && messageLog.First().Count > 0)
        {
            LatestRetrieval = messageLog.Last().LastMessageTime.AddSeconds(1);
            foreach (var message in messageLog)
            {
                MessageCount += message.Count;
            }
        }
        messageBox.ScrollTo(MessageCount);
        messageBox.ItemsSource = messageLog;
    }

    public async Task RetrieveNewMessages()
    {
        var log = await ProfileSingleton.GetInstance().GetAnnouncements(GroupId, LatestRetrieval);
        if (log != null && log.Count > 0)
        {
            if (log.Count > 0 && log.First().DateSent == messageLog.Last().DateSent)
            {
                foreach (var message in log.First())
                {
                    messageLog.Last().Add(message);
                    MessageCount++;
                }
                messageLog.Last().LastMessageTime = log.First().LastMessageTime;
                log.RemoveAt(0);
            }
            foreach (var message in log)
            {
                MessageCount += message.Count;
                messageLog.Add(message);
            }
            LatestRetrieval = messageLog.Last().LastMessageTime.AddSeconds(1);
            messageBox.ScrollTo(MessageCount);
        }
    }
}