using System.Collections.ObjectModel;

namespace Tavern;

public partial class RequestsPage : ContentPage
{
	ObservableCollection<RequestByGroup> requests;
	public RequestsPage()
	{
		InitializeComponent();

		Task t = Task.Run(async () => { await UpdateRequests(); });
		t.Wait();
	}

	public async Task UpdateRequests()
	{
		requests = await ProfileSingleton.GetInstance().GetAllRequests();
		requestStack.ItemsSource = requests;
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
		base.OnNavigatedTo(args);
        Task t = Task.Run(async () => { await UpdateRequests(); });
        t.Wait();
    }
}