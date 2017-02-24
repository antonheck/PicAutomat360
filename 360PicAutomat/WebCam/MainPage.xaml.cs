
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace PicAutomat
{
    public sealed partial class MainPage : Page
    {
        #region Private
        private MediaCapture _mediaCapture;
        private StorageFile _photoFile;
        private readonly string _PHOTO_FILE_NAME = "photo.jpg";
        private bool _isPreviewing;
        private bool _stop=false;
        private Motor _motor = new Motor();
        private DB _db = new DB("pic.sqlite");
        public ObservableCollection<ViewItem> CurrentPhotos = new ObservableCollection<ViewItem>();
        #endregion
        public MainPage( )
        {
            this.InitializeComponent();
        }
        #region Camera
        private async Task _initWebCam()
        {
            try
            {
                if (_mediaCapture == null)
                {
                    tbxStatus.Text = "Initializing camera to capture.";
                    _mediaCapture = new MediaCapture();
                    await _mediaCapture.InitializeAsync();
                    tbxStatus.Text = "Camera successfully initialized!";

                    previewElement.Source = _mediaCapture;
                    await _mediaCapture.StartPreviewAsync();

                    _isPreviewing = true;
                    tbxStatus.Text = "Camera preview succeeded";
                }
            }
            catch (Exception ex)
            {
                tbxStatus.Text = "Unable to initialize camera" + ex.Message;
            }
        }

        async Task<string> _TakePhoto()
        {
            if (_mediaCapture == null)
            {
                await _initWebCam();
            }
            _photoFile = await KnownFolders.PicturesLibrary.CreateFileAsync(_PHOTO_FILE_NAME, CreationCollisionOption.GenerateUniqueName);
            ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();

            await _mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, _photoFile);
            tbxStatus.Text = "Take Photo succeeded: " + _photoFile.Path;

            return _photoFile.Name;
        }

        private async Task _ShowImage(string IN_FileName)
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
            tmpCurrentPhoto.Name = IN_FileName;
            CurrentPhotos.Add(tmpCurrentPhoto);
        }

        private async Task _CapturePhotoSeries(int IN_Photos,int IN_Steps)
        {
            string tmpFileName;
            _stop = false;
            _ShowStopBtn();
            var tmpTimeStamp = DateTime.Now.ToString();
            int tmpPulseWidth= 50;
            ProgressBar.Maximum = IN_Photos;
            for (int i = 0; i <IN_Photos; i++)
            {
                tmpFileName = await _TakePhoto();
                await _ShowImage(tmpFileName);
                await _db.Write(tmpFileName, tmpTimeStamp);
                if (_stop == true)
                {
                    break;
                }
                await _motor.Run(IN_Steps, tmpPulseWidth);
                ProgressBar.Value = i+1;
            }
            _ShowStartBtn();
        }

        private async void _Stop()
        {
            if (_mediaCapture != null)
            {
                if (_isPreviewing)
                {
                    await _mediaCapture.StopPreviewAsync();
                    _isPreviewing = false;
                }
                _mediaCapture.Dispose();
                _mediaCapture = null;
            }
        }
        #endregion
        #region GUI
        private void _ShowStopBtn()
        {
            btnStopPhotoSeries.Visibility = Visibility.Visible;
            imgStop.Visibility = Visibility.Visible;
            btnStartPhotoSeries.Visibility = Visibility.Collapsed;
            imgStart.Visibility = Visibility.Collapsed;
            imgGallery.Visibility = Visibility.Collapsed;
        }

        private void _ShowStartBtn()
        {
            btnStopPhotoSeries.Visibility = Visibility.Collapsed;
            imgStop.Visibility = Visibility.Collapsed;
            btnStartPhotoSeries.Visibility = Visibility.Visible;
            imgStart.Visibility = Visibility.Visible;
            imgGallery.Visibility = Visibility.Visible;
        }

        async private void btnStartPhotoSeries_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar.Value = 0;
            var tmpPictures = Convert.ToInt32(txtPictures.Text);
            var tmpTurn = Convert.ToInt32(txtPicturesTurn.Text);

            await _CapturePhotoSeries(tmpPictures,tmpTurn);
          
        }

        async private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            if (_mediaCapture == null)
                await _initWebCam();
        }

        private void btnStopPhotoSeries_Click(object sender, RoutedEventArgs e)
        {
            _stop = true;
            ProgressBar.Value = 0;
        }

        private void btnGallery_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GalleryPage),null);
        }
        #endregion
    }
}
