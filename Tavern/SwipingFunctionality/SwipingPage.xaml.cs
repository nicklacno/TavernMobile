namespace Tavern.SwipingFunctionality;

public partial class SwipingPage : ContentPage
{
	public SwipingPage()
	{
		InitializeComponent();
		GridGroup.Children.Add(new SwipingView(5));
	}
}