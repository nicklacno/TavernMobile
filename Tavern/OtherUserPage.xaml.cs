namespace Tavern;

public partial class OtherUserPage : ContentPage
{
	int ProfileId { get; set; }

	public OtherUserPage(int id)
	{
		InitializeComponent();

		Task t = Task.Run(PopulateData);
		t.Wait();
	}

	public async Task PopulateData()
	{
		
	}

    private void SendFriendRequest(object sender, EventArgs e)
    {

    }
}