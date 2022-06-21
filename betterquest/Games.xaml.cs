using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
namespace betterquest
{
    class installer
    {
        public static bool MustDelete = true;
        public string InstallAPKOnDevice(string Name, sbyte mode = 1)
        {
            try
            {
                if (mode == 1)
                {
                    var proc = Process.Start("adb", "install " + PreLoad.FileSavePath + @"\" + Name);
                    proc.WaitForExit();
                    proc.Kill();
                    int a = proc.ExitCode;
                    return a.ToString();
                }
                if (mode == 2)
                {
                    var proc = Process.Start("adb", "install " + Name);
                    proc.WaitForExit();
                    int a = proc.ExitCode;
                    return a.ToString();
                }
            }
            catch { }
            return "1";
        }

        public string UnZip(string name)
        {
            string tempfolder = PreLoad.FileSavePath + @"\" + name.Substring(0, name.Length - 5) + "Folder";
            string contentpath = PreLoad.FileSavePath + @"\" + name;
            try
            {
                Directory.CreateDirectory(tempfolder);
                ZipFile.ExtractToDirectory(contentpath, tempfolder);
                string[] searchfile = Directory.GetFiles(tempfolder, "*.apk");
                for (int i = 0; i < searchfile.Length; i++)
                {
                    installer installer = new installer();
                    installer.InstallAPKOnDevice(searchfile[i], 2);
                }
                DirectoryInfo dir = new DirectoryInfo(tempfolder);
                var t = dir.GetDirectories();
                for (int i = 0; i < t.Length; i++)
                {
                    var proc = Process.Start("adb", "push " + t[i] + " /storage/emulated/0/Android/obb");
                    proc.WaitForExit();
                    proc.Kill();
                }
                if (MustDelete == true)
                {
                    Directory.Delete(tempfolder, true);
                    File.Delete(contentpath);
                }
                return "0";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                if (MustDelete == true)
                {
                    Directory.Delete(tempfolder, true);
                    File.Delete(contentpath);
                }
                return "1";
            }
        }
    }

    public partial class Games : Page
    {
        public string[][] GamesArray;
        public static string aaaaaaaaaaa;
        public static string aaaaaa;
        public static sbyte imstupid = 1;
        BlurEffect blurEffect = new BlurEffect();
        public Games()
        {
            InitializeComponent();
            GamesLoading();
        }
        async void DownloadEvent(Object sender, EventArgs e)
        {
            Button bts = sender as Button;
            bts.Content = "Скачивание";
            bts.Foreground = (Brush)(new BrushConverter().ConvertFrom("#000000"));
            bts.IsEnabled = false;
            DebugInformation.Text = "";
            string tag = ((sender as Button).Tag).ToString();
            aaaaaa = GamesArray[int.Parse(tag)][3];
            string name = GamesArray[int.Parse(tag)][0].ToString().Substring(1);
            aaaaaaaaaaa = name;
            try
            {
                avtoreblan(sender, e);
            }
            catch
            {
                MessageBox.Show("Sorry something went wrong ");
                bts.Background = (Brush)(new BrushConverter().ConvertFrom("#990000"));
                bts.Content = "Ошибка";
                MessageBox.Show(GamesArray[int.Parse(tag)][3]);
            }
        }
        public void avtoreblan(object sender, EventArgs e)
        {
            Download(Fixed.FixFirstLetter(aaaaaa), PreLoad.FileSavePath, aaaaaaaaaaa, sender, e);
        }
        void Options(object sender, RoutedEventArgs e)
        {
            blurEffect.Radius = 10;
            maingamegrid.Effect = blurEffect;
            options1.Visibility = Visibility.Visible;
            closeallfunc.Visibility = Visibility.Visible;
            directoryapp.Text = PreLoad.FileSavePath;
            ipapp.Text = PreLoad.IPAdressOfServer;
        }
        void optionsiwanttodelete(object sender, RoutedEventArgs e)
        {
            if (installer.MustDelete == true)
            {
                installer.MustDelete = false;
            }
            else
            {
                installer.MustDelete = true;
            }
        }
        void downloadlist(object sender, EventArgs e)
        {
            download1.Visibility = Visibility.Visible;
            closeallfunc.Visibility = Visibility.Visible;
            blurEffect.Radius = 10;
        }
        void exitfromdownload(object sender, EventArgs e)
        {
            blurEffect.Radius = 0;
            download1.Visibility= Visibility.Hidden;
            closeallfunc.Visibility = Visibility.Hidden;
        }
        void exitfromoptions(object sender, EventArgs e)
        {
            options1.Visibility = Visibility.Hidden;
            closeallfunc.Visibility = Visibility.Hidden;
            blurEffect.Radius = 0;
        }
        public void Download(string url, string save_path, string name, object sender, EventArgs e)
        {
            string fl = @"\";
            WebClient wc = new WebClient();
            double i = 0;
            wc.DownloadProgressChanged += (s, e) =>
            {
                i += 15.8;
                if (i >= 1000)
                {
                    DownloadingTextBlock.Text = "Downloading: " + Math.Round((i / 1000), 2) + " mb";
                }
                else
                {
                    DownloadingTextBlock.Text = "Downloading: " + Math.Round(i, 0) + " kb";
                }
            };
                wc.DownloadFileCompleted += (s, e) =>
                {
                    Button bts = sender as Button;
                    DebugInformation.Text = "Your file has been downloaded";
                    DownloadingTextBlock.Text = "";
                    bts.Content = "Установка";
                    string temporalthing = name.Substring(name.Length - 4);
                    if (temporalthing == ".apk")
                    {
                        installer ins = new installer();
                        string res = ins.InstallAPKOnDevice(name);
                        if (res == "0")
                        {
                            bts.Content = "Готов";
                            bts.Background = (Brush)(new BrushConverter().ConvertFrom("#00CC66"));
                            bts.Foreground = (Brush)(new BrushConverter().ConvertFrom("#000000"));
                        }
                        else
                        {
                            bts.Content = "Ошибка";
                            bts.Background = (Brush)(new BrushConverter().ConvertFrom("#990033"));
                            bts.Foreground = (Brush)(new BrushConverter().ConvertFrom("#FFFFFF"));
                            bts.IsEnabled = true;
                        }
                    }
                    else
                    {
                        if (temporalthing == ".zip")
                        {
                            installer ins1 = new installer();
                            string res = ins1.UnZip(name);
                            if (res == "0")
                            {
                                bts.Content = "Готов";
                                bts.Background = (Brush)(new BrushConverter().ConvertFrom("#00CC66"));
                                bts.Foreground = (Brush)(new BrushConverter().ConvertFrom("#000000"));
                            }
                            else
                            {
                                bts.Content = "Ошибка";
                                bts.Background = (Brush)(new BrushConverter().ConvertFrom("#990033"));
                                bts.Foreground = (Brush)(new BrushConverter().ConvertFrom("#FFFFFF"));
                                bts.IsEnabled = true;
                            }
                        }
                        else
                        {
                            bts.Content = "Готов";
                            bts.Background = (Brush)(new BrushConverter().ConvertFrom("#00CC66"));
                            bts.Foreground = (Brush)(new BrushConverter().ConvertFrom("#000000"));
                        }
                    }
                };
            wc.DownloadFileAsync(new Uri(url), save_path + fl + name);
        }
        public void GamesLoading()
        {
            GamesArray = MainWindow.GamesList;
            for (int i = 0; i < GamesArray.Length; i++)
            {
                Grid GameGrid = new Grid();
                GameGrid.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFFF");
                GameGrid.Height = 100;
                GameGrid.Children.Add(
                    new TextBlock
                    {
                        Text = GamesArray[i][0],
                        Margin = new System.Windows.Thickness(157, 8, 3, 80),
                        FontFamily = new FontFamily("Arial Black")
                    }
                    );
                GameGrid.Children.Add(
                    new TextBlock
                    {
                        Text = GamesArray[i][1],
                        TextWrapping = System.Windows.TextWrapping.Wrap,
                        Margin = new System.Windows.Thickness(157, 22, 24, 38),
                        FontFamily = new FontFamily("Bahnschrift SemiBold Condensed"),
                        FontSize = 18
                    }
                    );
                Button button = new Button();
                button.Tag = i.ToString();
                string Test = Convert.ToString(GamesArray[i][0]).Substring(1);
                button.Name = "btn";
                button.Content = "Download";
                if (GamesArray[i][3] != null)
                    button.Background = (Brush)(new BrushConverter().ConvertFrom("#181735"));
                else
                    button.Background = (Brush)(new BrushConverter().ConvertFrom("#808080"));

                button.Foreground = Brushes.White;
                button.Margin = new Thickness(157, 65, 345, 10);
                button.Click += DownloadEvent;
                button.FontFamily = new FontFamily("Arial Black");
                GameGrid.Children.Add(button);

                //картинка с вапросикам)
                Image GameImage0 = new Image();
                GameImage0.Margin = new System.Windows.Thickness(10, 10, 436, 10);
                BlurEffect BlurEffect = new BlurEffect();
                BlurEffect.Radius = 5;
                GameImage0.Effect = BlurEffect;
                BitmapImage myBitmapImage = new BitmapImage();
                myBitmapImage.BeginInit();
                myBitmapImage.DecodePixelWidth = 200;
                GameImage0.Source = myBitmapImage;
                if (GamesArray[i][3] == null)
                {
                    myBitmapImage.UriSource = new Uri("../NotEnoghtImage.png", UriKind.Relative);

                }
                else
                {
                    try
                    {
                        myBitmapImage.UriSource = new Uri(GamesArray[i][2], UriKind.Absolute);
                        BlurEffect.Radius = 0;
                    }
                    catch
                    {
                        myBitmapImage.UriSource = new Uri("../NotEnoghtImage.png", UriKind.Relative);
                    }
                }
                myBitmapImage.EndInit();
                GameGrid.Children.Add(GameImage0);
                //картинка с вапросикам)
                GameListScroller.Children.Add(GameGrid);
            }
        }
        void CheakInformationAboutAPP(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Information());
        }
        void AdminDownloadServer(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://disk.yandex.ee/d/W2leCGblmnub6w");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка внутри вашего пк " + ex);
            }
        }
        void AdministrationModeSave(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://disk.yandex.ee/d/W2leCGblmnub6w");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка внутри вашего пк " + ex);
            }
        }
    }
}
