using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;
using DirectShowLib;
using DirectShowLib.DES;
using Image = System.Drawing.Image;

namespace BCRK
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private bool isScreenshotChose;
        private bool isVideoChose;
        private string screenshotFile;
        private string videoFile;

        private double count;
        private double time;

        private MediaElement me;

        private void ChooseScreenshot_OnClick(object sender, RoutedEventArgs e)
        {
            string file = PickFile();
            if (!string.IsNullOrEmpty(file))
            {
                isScreenshotChose = true;
                screenshotFile = file;
                ScreenshotBlock.Text = screenshotFile;
                Image image = Image.FromFile(file);
                count = Math.Floor((double) (image.Height / int.Parse(DanmakuHeightBox.Text)));
                DanmakuCountBlock.Text = count.ToString();
                CheckStatus();
            }
        }

        private void ChooseVideo_OnClick(object sender, RoutedEventArgs e)
        {
            string file = PickFile();
            if (!string.IsNullOrEmpty(file))
            {
                isVideoChose = true;
                videoFile = file;
                VideoBlock.Text = videoFile;
                time = GetMediaTimeLen(file);
                VideoTimeBlock.Text = time.ToString();

                TransTimeBlock.Text = TimeSpan.FromMilliseconds(time / count).ToString("G");
                CheckStatus();
            }
        }

        private async Task<string> GetData()
        {
                Uri uri = new Uri("/DATA", UriKind.Relative);
                StreamResourceInfo info = Application.GetResourceStream(uri);
                TextReader tr = new StreamReader(info.Stream);
                return await tr.ReadToEndAsync();
        }

        private bool CheckStatus()
        {

            try
            {
            if (!(isVideoChose && isScreenshotChose && !string.IsNullOrEmpty(
                      DanmakuHeightBox.Text) && !string.IsNullOrEmpty(AnimationTimeBox.Text)))
            {
                GenerateButton.IsEnabled = false;
                return false;
            }

            ScreenshotBlock.Text = screenshotFile;
            VideoBlock.Text = videoFile;
            DanmakuCountBlock.Text = count.ToString();

            VideoTimeBlock.Text = time.ToString();

            TransTimeBlock.Text = (time / count).ToString();
            }
            catch (Exception)
            {
                return false;
            }

            GenerateButton.IsEnabled = true;
            return true;
        }

        private string PickFile()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();


            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                
                return openFileDialog.FileName;

            }

            return null;
        }

        private async void GenerateButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!CheckStatus()) return;
            StringBuilder sb = new StringBuilder();
            sb.Append($"danmakuCount = {count};\n");
            sb.Append($"animationTime = {double.Parse(AnimationTimeBox.Text)};\n");
            sb.Append(await GetData());
            ResultBox.Text = sb.ToString();
        }

        public static double GetMediaTimeLen(string path)
        {
            var mediaDet = (IMediaDet)new MediaDet();
            DsError.ThrowExceptionForHR(mediaDet.put_Filename(path));
            double mediaLength;//这个是视频长度，单位秒
            mediaDet.get_StreamLength(out mediaLength);
            return mediaLength;
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            CheckStatus();
        }
    }
}
