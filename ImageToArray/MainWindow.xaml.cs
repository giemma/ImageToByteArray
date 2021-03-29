using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
using System.Windows.Shapes;

namespace ImageToArray
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Bitmap SourceImage { get; set; }
        Bitmap DestinationImage { get; set; }

        double OriginalWHRatio { get; set; }
        string ImageFilename { get; set; }
        int DestinationH { get; set; }
        int DestinationW { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            TxtDestinationH.KeyUp += TxtDestinationH_KeyDown;
            TxtDestinationW.KeyUp += TxtDestinationW_KeyDown;
        }


        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == false)
                return;

            ImageFilename = openFileDialog.FileName;

            SourceImage = new Bitmap(ImageFilename);
            DestinationImage = new Bitmap(ImageFilename);

            ImgOriginal.Source = ConvertBitmap(SourceImage);
            ImgDestination.Source = ConvertBitmap(DestinationImage);

            LblOriginalW.Content = SourceImage.Width;
            TxtDestinationW.Text = SourceImage.Width.ToString();
            DestinationW = SourceImage.Width;

            LblOriginalH.Content = SourceImage.Height;
            TxtDestinationH.Text = SourceImage.Height.ToString();
            DestinationH = SourceImage.Height;

            OriginalWHRatio = ((double)SourceImage.Width / (double)SourceImage.Height);
            LblOriginalWHRatio.Content = OriginalWHRatio;
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int y = 0; y < DestinationImage.Height; y++)
            {
                for (int x = 0; x < DestinationImage.Width; x++)
                {
                    var color = DestinationImage.GetPixel(x, y);

                    int b1, b2;

                    b1 = (color.R & 0xF8) | (color.G >> 5);
                    b2 = ((color.G & 0x1C) << 3) | (color.B >> 3);

                    stringBuilder.AppendLine($"0x{b2.ToString("X")},0x{b1.ToString("X")},");
                }
            }

            string header = $"const unsigned char myImage[{DestinationImage.Height * DestinationImage.Width * 2}] = {{";
            string code = stringBuilder.ToString();
            code = code.Substring(0, code.Length - 3);
            string footer = "};";
            TxtCode.Text = $"{header}{Environment.NewLine}{code}{Environment.NewLine}{footer}";
        }


        public BitmapImage ConvertBitmap(System.Drawing.Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }


        private void TxtDestinationW_KeyDown(object sender, KeyEventArgs e)
        {
            int x;
            if (int.TryParse(TxtDestinationW.Text, out x))
            {
                DestinationH = Convert.ToInt32(x / OriginalWHRatio);
                TxtDestinationH.Text = DestinationH.ToString();

                ImgDestination.Source = null;

                if (DestinationH > 0)
                {
                    var DestinationImage = new Bitmap(x, DestinationH);

                    using (var graphics = Graphics.FromImage(DestinationImage))
                        graphics.DrawImage(SourceImage, 0, 0, x, DestinationH);

                    ImgDestination.Source = ConvertBitmap(DestinationImage);
                }
            }
            else
            {
                TxtDestinationH.Text = "--";
            }
        }

        private void TxtDestinationH_KeyDown(object sender, KeyEventArgs e)
        {
            int x;
            if (int.TryParse(TxtDestinationH.Text, out x))
            {
                DestinationW = Convert.ToInt32(x * OriginalWHRatio);
                TxtDestinationW.Text = DestinationW.ToString();

                ImgDestination.Source = null;

                if (DestinationW > 0)
                {
                    var DestinationImage = new Bitmap(DestinationW, x);

                    using (var graphics = Graphics.FromImage(DestinationImage))
                        graphics.DrawImage(SourceImage, 0, 0, DestinationW, x);

                    ImgDestination.Source = ConvertBitmap(DestinationImage);
                }
            }
            else
            {
                TxtDestinationW.Text = "--";
            }
        }




    }
}
