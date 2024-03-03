using Microsoft.Maui.Controls;
using System.Diagnostics;
namespace Tavern.SwipingFunctionality;

public partial class SwipingView : SwipeView
{
	public SwipingView()
	{
		InitializeComponent();
	}

	/**
	 * OnJoinGroup - sends request to join group in database
	 */
	async void OnJoinGroup(object sender, EventArgs e)
	{
		Debug.WriteLine("Join");
	}
	
	/**
	 * OnSkipGroup - sends request to app to move to the next group in the list
	 */
	async void OnSkipGroup(object sender, EventArgs e)
	{
		Debug.WriteLine("Skip");
	}
}