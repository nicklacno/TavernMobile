namespace Tavern.SwipingFunctionality;

public partial class SwipingPage : ContentPage
{
	private Queue<Group> groups = new Queue<Group>(10);

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
		if (groups.Count > 0)
		{
			GridGroup.Children.Clear();
			GridGroup.Children.Add(new SwipingView(groups.Dequeue()));
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
		ProfileSingleton singleton = ProfileSingleton.GetInstance();
		groups.Enqueue(await singleton.GetGroup(2));
		groups.Enqueue(await singleton.GetGroup(3));
		groups.Enqueue(await singleton.GetGroup(4));

		ShowNextGroup();
	}
}