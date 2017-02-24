using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using System.Collections.ObjectModel;


namespace PicAutomat
{

    public sealed partial class GalleryPage : Page
    {
        public ObservableCollection<ViewItem> CurrentPhotos = new ObservableCollection<ViewItem>();
        private DB _db = new DB("pic.sqlite");

        public GalleryPage()
        {
            this.InitializeComponent();
            _ShowAllSeries();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), null);
        }

        async private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            await _ShowAllSeries();
        }

        async private Task _ShowAllSeries()
        {
            var tmpUniquePictures = await _db.GetUniqueSeries();
            foreach (var tmpPicture in tmpUniquePictures)
            {
                await _ShowImage(tmpPicture.Name, tmpPicture.TimeStamp);
            }
        }
        private async Task _ShowImage(string IN_FileName, string IN_TimeStamp)
        {
            string tmpFileName = IN_FileName;
            var tmpFile = await Windows.Storage.KnownFolders.PicturesLibrary.GetFileAsync(tmpFileName);
            var tmpStream = await tmpFile.OpenReadAsync();
            var tmpBitmapImage = new BitmapImage();
            int tmpCurrentPhotosListPhotoWidth = 200;
            tmpBitmapImage.DecodePixelWidth = tmpCurrentPhotosListPhotoWidth;
            tmpBitmapImage.SetSource(tmpStream);
            var tmpCurrentPhoto = new ViewItem();
            tmpCurrentPhoto.CurrentPhoto = tmpBitmapImage;
            tmpCurrentPhoto.Name = IN_TimeStamp;
            CurrentPhotos.Add(tmpCurrentPhoto);
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var tmpSelectedSeries = (ViewItem)e.ClickedItem;
            var tmpViewItemDb = new ViewItemDb();
            tmpViewItemDb.TimeStamp = tmpSelectedSeries.Name;
            Frame.Navigate(typeof(SeriesView), tmpViewItemDb);
        }
    }
}
