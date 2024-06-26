﻿using Tavern.SwipingFunctionality;

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

            MainPage = new NavigationPage(new LoginPage());
            ProfileSingleton singleton = ProfileSingleton.GetInstance(); //gets singleton
            if (singleton.isLoggedIn) //checks login, storage will hold temporary data
            {
                ChangeMainPage(new NavigationPage(new TabbedMainPage()));
            }
            else
            {
                //MainPage = new NavigationPage(new LoginPage()); //Sets to login page if not currently logged in
                singleton.switchMainPage = new ProfileSingleton.BasePageEvent(ChangeMainPage); //sets delegate for when successful login
            }
        }
        /**
         * MoveToMainPage - Gets called via delegate to switch the page the user is viewing
         */
        public void ChangeMainPage(Page newPage)
        {
            MainPage = newPage; //same as in constructor
        }
    }
}
