namespace Tavern.PrivateMessage;

public partial class PrivateMessagePage : ContentPage
{
	private int ChatID { get; set; }
	private DateTime? lastUpdate { get; set; } = null;
	private bool Updating { get; set; } = true;
	public PrivateMessagePage(OtherUser other)
	{
		InitializeComponent();
		var singleton = ProfileSingleton.GetInstance();
	}

	public async Task GetNewMessages()
	{

	}

	public async void SendMessage(object sender, EventArgs e)
	{
		
	}
}