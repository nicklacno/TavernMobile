namespace Tavern
{
    public partial class App : Application
    {
        /**
         * App - constructor the application, backbone of the app
         */
        public App()
        {
            InitializeComponent(); //Initializes the xaml elements

            ProfileSingleton singleton = ProfileSingleton.GetInstance(); //gets singleton
            if (singleton.isLoggedIn) //checks login, storage will hold temporary data
                MainPage = new NavigationPage(new MainPage());//sets page to MainPage (need to change!!!), navigation page base allows stacking
            else
            {
                MainPage = new LoginPage(); //Sets to login page if not currently logged in
                singleton.loginSuccessful = new ProfileSingleton.LoginSuccessful(MoveToMainPage); //sets delegate for when successful login
            }
                
        }
        /**
         * MoveToMainPage - Gets called via delegate to switch the page the user is viewing
         */
        public void MoveToMainPage()
        {
            MainPage = new NavigationPage(new MainPage()); //same as in constructor
        }
    }
}
