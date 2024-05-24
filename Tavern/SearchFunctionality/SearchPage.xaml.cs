namespace Tavern.SearchFunctionality;

public partial class SearchPage : ContentPage
{
	public SearchPage()
	{
		InitializeComponent();
		Task t = Task.Run(async () => { await SetTagList(); });
		t.Wait();
	}

	private async void Search(object sender, EventArgs e)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		if (!string.IsNullOrEmpty(txtSearch.Text)) data.Add("text", txtSearch.Text);
		int nextId = 1;
		foreach (object o in TagList.SelectedItems)
		{
			if (o is Tag t)
			{
				data.Add("tag" + nextId, t.Id.ToString());
				nextId++;
			}
		}

		var list = await ProfileSingleton.GetInstance().GetSearchResults(data);
		await Navigation.PushAsync(new SearchResultsPage(list));
	}

	private async Task SetTagList()
	{
		TagList.ItemsSource = await ProfileSingleton.GetInstance().GetGroupTags();
	}
}