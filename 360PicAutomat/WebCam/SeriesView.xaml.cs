using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media.Imaging;

namespace PicAutomat
{
    public sealed partial class SeriesView : Page
    {
        public ObservableCollection<ViewItem> CurrentPhotos = new ObservableCollection<ViewItem>();
        private DB _db = new DB("pic.sqlite");
        private ObservableCollection<ViewItemDb> _uniquePictures = new ObservableCollection<ViewItemDb>();

        public SeriesView()
        {
            this.InitializeComponent();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GalleryPage), null);
        }

        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var tmpSelectedItem = (ViewItemDb)e.Parameter;
            await _ShowAllSeriesPhotos(tmpSelectedItem.TimeStamp);

        }

        async private Task _ShowAllSeriesPhotos(string IN_Name)
        {
            _uniquePictures = await _db.Get(IN_Name);
            foreach (var tmpPicture in _uniquePictures)
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
            Frame.Navigate(typeof(ImageView), _uniquePictures[0]);
        }
    }
}
