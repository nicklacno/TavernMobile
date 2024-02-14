namespace Tavern
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new NavigationPage(new MainPage());
            LoginToPage();
        }

        public void LoginToPage()
        {
            MainPage = new LoginPage();
        }
    }
}
