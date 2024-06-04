using MessageLog = System.Collections.ObjectModel.ObservableCollection<Tavern.MessageByDay>;

namespace Tavern;

public partial class GroupAnnouncementView : ContentView
{
    DateTime LatestRetrieval { get; set; }
    int GroupId { get; set; }
    GroupPage Parent { get; set; }
    MessageLog messageLog { get; set; } = new MessageLog();

    public GroupAnnouncementView(int groupId, GroupPage groupPage)
    {
        InitializeComponent();
        GroupId = groupId;
        Parent = groupPage;
        Task t = Task.Run(async () => await SetBase());
        t.Wait();
    }

    public async void PostAnnounement(object sender, EventArgs e)
    {

    }

    public async Task SetBase()
    {
        ProfileSingleton singleton = ProfileSingleton.GetInstance();
        messageLog = await singleton.GetAnnouncements(GroupId, null);
        if (messageLog == null) messageLog = new MessageLog();
        if (messageLog.Count > 0 && messageLog.First().Count > 0)
        {
            LatestRetrieval = messageLog.First().FirstMessageTime;
        }
        messageBox.ScrollTo(0);
        messageBox.ItemsSource = messageLog;
    }

    public async Task RetrieveNewMessages()
    {
        var log = await ProfileSingleton.GetInstance().GetAnnouncements(GroupId, LatestRetrieval);
        if (log == null || log.Count == 0) return;

        LatestRetrieval = log.First().FirstMessageTime;
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
        messageBox.ScrollTo(0);
    }
}