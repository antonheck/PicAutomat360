using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;

namespace PicAutomat
{

    public sealed partial class ImageView : Page
    {
        public ObservableCollection<ViewItemDb> CurrentPhotos = new ObservableCollection<ViewItemDb>();
        private DB _db = new DB("pic.sqlite");
        private int _currentPhotoIndex = 0;

        public ImageView()
        {
            this.InitializeComponent();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SeriesView), CurrentPhotos[0]);
        }

        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var tmpSelectedItem = (ViewItemDb)e.Parameter;
            await _ShowAllSeriesPhotos(tmpSelectedItem.TimeStamp);

        }

        async private Task _ShowAllSeriesPhotos(string IN_Name)
        {
            CurrentPhotos = await _db.Get(IN_Name);

            await _ShowImage(CurrentPhotos[0].Name);

        }

        private async Task _ShowImage(string IN_FileName)
        {
            string tmpFileName = IN_FileName;
            var tmpFile = await Windows.Storage.KnownFolders.PicturesLibrary.GetFileAsync(tmpFileName);
            var tmpStream = await tmpFile.OpenReadAsync();
            var tmpBitmapImage = new BitmapImage();
            tmpBitmapImage.DecodePixelWidth = 800;
            tmpBitmapImage.SetSource(tmpStream);
            imgViewPhoto.Source = tmpBitmapImage;
        }

        async private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            await _ShowImage(CurrentPhotos[_currentPhotoIndex].Name);

            if (_currentPhotoIndex > 0)
            {
                _currentPhotoIndex--;
            }
        }

        async private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            await _ShowImage(CurrentPhotos[_currentPhotoIndex].Name);

            if (_currentPhotoIndex < CurrentPhotos.Count - 1)
            {
                _currentPhotoIndex++;
            }
        }
    }
}
