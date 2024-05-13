
namespace Tavern;

public partial class EditGroupPage : ContentPage
{
    public int Id { get; set; }

    public EditGroupPage(int id)
	{
		InitializeComponent();
		Id = id;
        GetRequests();
	}

    private async Task GetRequests()
    {
        var singleton = ProfileSingleton.GetInstance();
        requestStack.ItemsSource = await singleton.GetGroupRequests(Id);
    }
}