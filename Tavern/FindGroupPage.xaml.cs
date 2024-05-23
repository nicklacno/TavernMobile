using System.Diagnostics;

namespace Tavern;

public partial class FindGroupPage : ContentPage
{
	public FindGroupPage()
	{
		InitializeComponent();
	}

	public async void OnClickSwiping(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new SwipingFunctionality.SwipingPage());
	}

	public async void OnClickSearching(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new SearchFunctionality.SearchPage());
	}

	public async void FindByCode(object sender, EventArgs e)
	{
		string code = await DisplayPromptAsync("Group Code", "Enter The Group Code:", accept: "Search", placeholder: "Code", maxLength: 6);
		Debug.WriteLine(code);

	}
}