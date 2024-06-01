using System.Diagnostics;
using MessageLog = System.Collections.ObjectModel.ObservableCollection<Tavern.MessageByDay>;
namespace Tavern.PrivateMessage;

public partial class PrivateMessagePage : ContentPage
{ 
	private OtherUser Other { get; set; }
	private DateTime? LastUpdate { get; set; } = null;
	private bool Updating { get; set; } = true;

	private MessageLog log =  new MessageLog();

	object _lock = new object();

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
		var singleton = ProfileSingleton.GetInstance();
		if (!string.IsNullOrEmpty(txtBody.Text))
		{
			int ret = await singleton.SendPrivateMessage(Other.Id, txtBody.Text);
			if (ret != 0)
			{
				ShowErrorMessage("Failed to Send Message");
			}
		}
		txtBody.Text = "";
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
    public async Task ShowErrorMessage(string message)
    {
        await DisplayAlert("An Error Occurred", message, "Okay");
    }
}