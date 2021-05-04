using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;
using Xamarin.Essentials;
using System.Diagnostics;
using System.IO;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace SaveEarth.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CapturePage : ContentPage
    {
        private const int MaxColumns = 3;

        private double _rowHeight = 0;
        private int _currentRow = 0;
        private int _currentColumn = -1;

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //Get All photos  
            var personList = await App.SQLiteDb.GetItemsAsync();
            if (personList.Count> 0)
            {
                lstPersons.ItemsSource = personList;
            }
        }

        public CapturePage()
        {
            InitializeComponent();
            Device.BeginInvokeOnMainThread(async () => await InitialisePermissions());
            var addPhotoButton = new ImageButton()
            {
                //Text = "Capture Photo",
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                //BorderColor = Color.FromHex("#F0F0F0"),
                BorderWidth = 1,
                //BackgroundColor = Color.FromHex("#F9F9F9"),
                //TextColor = Color.Black,
                //FontAttributes = FontAttributes.Bold,
                IsVisible = false
            };

            //btn.Clicked += async (object sender, EventArgs e) => await LoadImages();

            btn.Clicked += async (object sender, EventArgs e) => await AddPhoto();

            ImageGridContainer.Children.Add(addPhotoButton);

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(10);

                _rowHeight = addPhotoButton.Width;
                ImageGridContainer.RowDefinitions[0].Height = 130;

                //await LoadImages();

                await ImageGridContainer.FadeTo(1);

            });
        }

        async Task AddPhoto()
        {
            MediaFile file = null;

            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", "You need to fix the problem of camera availability", "OK");
                return;
            }
            var imageSource = await DisplayActionSheet("Image Source", "Cancel", null, new string[] { "Camera", "Photo Gallery" });
            var photoName = Guid.NewGuid().ToString() + ".jpg";
            switch (imageSource)
            {
                case "Camera":
                    file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = photoName
                    });
                    break;
                case "Photo Gallery":
                    file = await CrossMedia.Current.PickPhotoAsync();
                    break;
                default:
                    break;
            }

            if (file == null)
                return;

            // We have the photo, now add it to the grid.
            _currentColumn++;
            if (_currentColumn > MaxColumns - 1)
            {
                _currentColumn = 0;
                _currentRow++;
                ImageGridContainer.RowDefinitions.Add(ImageGridContainer.RowDefinitions[0]);
            }
            var newImage = new Image()
            {

                Source = ImageSource.FromFile(file.Path),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Aspect = Aspect.AspectFill,
                Scale = 0
            };
            AddToDB(file);
            ImageGridContainer.Children.Add(newImage, _currentColumn, _currentRow);
            await Task.Delay(250);
            await newImage.ScaleTo(1, 250, Easing.SpringOut);
        }

        async Task InitialisePermissions()
        {
            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<CameraPermission>();
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();
            if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            {
                _ = await CrossPermissions.Current.RequestPermissionAsync<CameraPermission>();
                _ = await CrossPermissions.Current.RequestPermissionAsync<StoragePermission>();
            }
        }


        private async void AddToDB(MediaFile file)
        {
            if (file != null)
            {
                var tt = file.GetStream();
                var item = new MediaItemToUpload()
                {
                    itemData = GetImageBytes(tt),
                    mediaLocation = file.Path
                }; 


                //Add New image to db
                await App.SQLiteDb.SaveItemAsync(item);
                //txtName.Text = string.Empty;
                await DisplayAlert("Success", "image added Successfully", "OK");
                //Get All Persons  
                var imgList = await App.SQLiteDb.GetItemsAsync();


                
                //if (imgList != null)
                //{
                byte[] b = imgList[0].itemData;
                Stream ms = new MemoryStream(b);
                    //lstPersons.Source = ImageSource.FromStream(() => ms);
                //}


                //var stream1 = new MemoryStream(viewModel.SImageBase64);
                //IncidentImageData.Source = ImageSource.FromStream(() => stream1);

                var newImage = new Image()
                {


                    Source = ImageSource.FromFile(imgList[0].mediaLocation),
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Aspect = Aspect.AspectFill,
                    Scale = 0
                };


                ImageGridContainer.Children.Add(newImage, _currentColumn, _currentRow);
                await Task.Delay(250);
                await newImage.ScaleTo(1, 250, Easing.SpringOut);

                //if (personList != null)
                //{
                //    lstPersons.ItemsSource = personList;
                //}
            }
            else
            {
                await DisplayAlert("Required", "Please Enter name!", "OK");
            }

        }


        //private void getdata()
        //{
        //    var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "sqlite1.db3");
        //    using (var con = new SQLiteConnection(path))
        //    {
        //        var image = con.Query<MediaItemToUpload>("SELECT content FROM Image ;").FirstOrDefault();

        //        if (image != null)
        //        {
        //            byte[] b = image.itemData;
        //            Stream ms = new MemoryStream(b);
        //            image1.Source = ImageSource.FromStream(() => ms);
        //        }

        //    }
        //}

        private byte[] GetImageBytes(Stream stream)
        {
            byte[] ImageBytes;
            using (var memoryStream = new System.IO.MemoryStream())
            {
                stream.CopyTo(memoryStream);
                ImageBytes = memoryStream.ToArray();
            }
            return ImageBytes;
        }

        public Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        private async void BtnRead_Clicked(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(""))
            {
                //Get Person  
                var person = await App.SQLiteDb.GetItemAsync(Convert.ToInt32("123"));
                if (person != null)
                {
                    //txtName.Text = person.Name;  
                    //await DisplayAlert("Success", "Person Name: " + person.Name, "OK");
                }
            }
            else
            {
                await DisplayAlert("Required", "Please Enter PersonID", "OK");
            }

        }
    }

}