using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SaveEarth.Views;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace SaveEarth
{
    public partial class App : Application
    {
        static SQLiteHelper db;
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

        public static SQLiteHelper SQLiteDb
        {
            get
            {
                if (db == null)
                {
                    db = new SQLiteHelper(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "XamarinSQLite.db3"));
                }
                return db;
            }
        }
    }
  
}
