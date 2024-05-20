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

	}
}