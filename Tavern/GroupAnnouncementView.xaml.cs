using System.Diagnostics;
using MessageLog = System.Collections.ObjectModel.ObservableCollection<Tavern.MessageByDay>;

namespace Tavern;

public partial class GroupAnnouncementView : ContentView
{
    DateTime LatestRetrieval { get; set; }
    int GroupId { get; set; }
    GroupPage Parent { get; set; }
    MessageLog messageLog { get; set; } = new MessageLog();

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
        txtMessage.Text = "";
    }

    public async Task SetBase()
    {
        ProfileSingleton singleton = ProfileSingleton.GetInstance();
        messageLog = await singleton.GetAnnouncements(GroupId, null);
        if (messageLog == null) messageLog = new MessageLog();
        if (messageLog.Count > 0 && messageLog.First().Count > 0)
        {
            LatestRetrieval = messageLog.First().FirstMessageTime.AddSeconds(1);
        }
        messageBox.ScrollTo(0);
        messageBox.ItemsSource = messageLog;
    }

    public async Task RetrieveNewMessages()
    {
        var log = await ProfileSingleton.GetInstance().GetAnnouncements(GroupId, LatestRetrieval);
        if (log == null || log.Count == 0) return;

        LatestRetrieval = log.First().FirstMessageTime.AddSeconds(1);
        if (log.Last().Count > 0 && messageLog.Count > 0 && messageLog.First().DateSent == log.Last().DateSent)
        {
            foreach (var message in log.Last())
            {
                messageLog.First().Prepend(message);
            }
            log.Remove(log.Last());
        }
        foreach (var day in log)
        {
            messageLog.Prepend(day);
        }
        messageBox.ItemsSource = messageLog;
        messageBox.ScrollTo(0);
    }
}