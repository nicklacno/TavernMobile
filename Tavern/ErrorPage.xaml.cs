namespace Tavern;

public partial class ErrorPage : ContentPage
{
	public ErrorPage(string errorMsg = "")
	{
		InitializeComponent();
		ErrorMsg.Text = errorMsg;
	}
}