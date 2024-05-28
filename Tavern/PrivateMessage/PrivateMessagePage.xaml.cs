using System.Diagnostics;
using MessageLog = System.Collections.ObjectModel.ObservableCollection<Tavern.MessageByDay>;
namespace Tavern.PrivateMessage;

public partial class PrivateMessagePage : ContentPage
{ 
	private OtherUser Other { get; set; }
	private DateTime? LastUpdate { get; set; } = null;
	private bool Updating { get; set; } = true;

	private MessageLog log =  new MessageLog();

	public PrivateMessagePage(OtherUser other)
	{
		InitializeComponent();
		Title = other.Name;
		messageBox.ItemsSource = log;
		Other = other;
		Task.Run(GetMessages);
	}

	public async Task GetNewMessages()
	{
		var singleton = ProfileSingleton.GetInstance();
		var newMessages = await singleton.GetPrivateChat(Other.Id, LastUpdate);
		LastUpdate = DateTime.UtcNow;

		if (newMessages == null || newMessages.Count == 0) return;

		if (log.Count != 0 && newMessages.First().DateSent.Equals(log.Last().DateSent))
		{
			foreach (var message in newMessages.First())
			{
				log.Last().Add(message);
			}
			newMessages.RemoveAt(0);
		}
		foreach (var message in newMessages)
		{
			log.Add(message);
		}
	}

	public async void SendMessage(object sender, EventArgs e)
	{
		
	}

	private async Task GetMessages()
	{
		while (Updating)
		{
			await GetNewMessages();
			Thread.Sleep(2000);
		}
	}

    protected override bool OnBackButtonPressed()
    {
		Updating = false;
        return base.OnBackButtonPressed();
    }
}