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

namespace SAPP.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CapturePage : ContentPage
    {
        private const int MaxColumns = 3;

        private double _rowHeight = 0;
        private int _currentRow = 0;
        private int _currentColumn = -1;


        public CapturePage()
        {
            InitializeComponent();
            Device.BeginInvokeOnMainThread(async () => await InitialisePermissions());
            var addPhotoButton = new Button()
            {
                Text = "Capture Photo",
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BorderColor = Color.FromHex("#F0F0F0"),
                BorderWidth = 1,
                BackgroundColor = Color.FromHex("#F9F9F9"),
                TextColor = Color.Black,
                FontAttributes = FontAttributes.Bold,
                IsVisible=false
            };
          
            btn.Clicked += async (object sender, EventArgs e) => await AddPhoto();

           ImageGridContainer.Children.Add(addPhotoButton);

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(10);

                _rowHeight = addPhotoButton.Width;
                ImageGridContainer.RowDefinitions[0].Height = 130;

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
                _= await CrossPermissions.Current.RequestPermissionAsync<CameraPermission>();
                _= await CrossPermissions.Current.RequestPermissionAsync<StoragePermission>();
            }
        }
    }
}