using System;
using SaveEarth.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SaveEarth
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new CapturePage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
