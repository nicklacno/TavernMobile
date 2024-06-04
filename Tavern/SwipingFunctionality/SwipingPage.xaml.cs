namespace Tavern.SwipingFunctionality;

public partial class SwipingPage : ContentPage
{
	public SwipingPage()
	{
		InitializeComponent();
		SwipingSingleton.RequestGroup = new SwipingSingleton.GroupAction(ShowNextGroup);
		SwipingSingleton.SkipGroup = new SwipingSingleton.GroupAction(ShowNextGroup);

		PopulateGroupQueue();
	}
    /**
	 * ShowNextGroup - Shows the next group in the queue, shows nothing if ended
	 */
    private void ShowNextGroup()
	{
		if (SwipingSingleton.Groups.Count > 0)
		{
			GridGroup.Children.Clear();
			GridGroup.Children.Add(new SwipingView(SwipingSingleton.Groups.Dequeue()));
		}
		else
		{
			Navigation.PopAsync();
		}	
	}


    /**
	 * PopulateGroupQueue - Helper function that populates the queue
	 */
    private async void PopulateGroupQueue()
    {
		SwipingSingleton swipingSingleton = SwipingSingleton.GetInstance();
        await swipingSingleton.PopulateGroups();  
        ShowNextGroup();
    }


}