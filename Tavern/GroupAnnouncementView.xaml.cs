using MessageLog = System.Collections.ObjectModel.ObservableCollection<Tavern.MessageByDay>;

namespace Tavern;

public partial class GroupAnnouncementView : ContentView
{
	DateTime LatestRetrieval { get; set; }
	int GroupId { get; set; }
	GroupPage Parent { get; set; }
	MessageLog messageLog {  get; set; } = new MessageLog();

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
		
	}
}