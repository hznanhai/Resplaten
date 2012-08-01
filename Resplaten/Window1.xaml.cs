using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using System.Windows.Threading;

namespace Resplaten
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        private const string imagesPath = @"d:\错觉图片\";
        private const float MAX_NEGATIVE = (float)-3.0E28;   //小于该值，Marker点MISS
        private double screenWidth,screenHeight,imageWidth,imageHeight;
        private FileInfo[] imagesFileInfo = null;
        private ThreeTimesRandom threeRandom;
        private BackgroundWorker backgroundWorker;
        private BackgroundWorker ndiBW;

        private NDILib.Position3d position3D0, position3D1, position3D2;
        private int imageCount = 0;
        private string picName;
        private BitmapImage bitmapImage;
        private StoreData storeData;
        private Stopwatch sw;
        private double showTime, releaseTime, timespan;

        private delegate bool BindPicDispatcherDelegate();
        private delegate void SetUIDelegate();
        private Random rd;

        private bool hasRecordTime = false;
        private bool isSpacePressed = false;
        private bool processStarted = false;
        private bool expriStarted = false;
        private bool ndiToStop = false;

        public Window1()
        {
            InitializeComponent();
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);

            ndiBW = new BackgroundWorker();
            ndiBW.DoWork += new DoWorkEventHandler(ndiBW_DoWork);

            storeData = new StoreData();
            sw = new Stopwatch();
            sw.Start();

            rd = new Random();
        }

        void ndiBW_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true){
                get3DPosition();
                if (position3D0.x>MAX_NEGATIVE && position3D1.x>MAX_NEGATIVE && position3D2.x>MAX_NEGATIVE){
                    timespan = sw.Elapsed.TotalMilliseconds;
                    storeData.addData(position3D0,position3D1,position3D2,picName,timespan);
                }
                if (ndiToStop){
                    break;
                }
            }
        }

        private void initUI() {
            this.guideTextblock.Visibility = Visibility.Hidden;
            this.crossLabel.Visibility = Visibility.Visible;
            backgroundWorker.RunWorkerAsync();
        }

        private  void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            processStarted = true;
            while (!isSpacePressed) {
                //this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new SetUIDelegate(settipLabelVisibility));
                Thread.Sleep(500);
                //this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new SetUIDelegate(settipLabelVisibility));
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new SetUIDelegate(checkSpcaePressed));
            }
            Thread.Sleep(2000);
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new BindPicDispatcherDelegate(bindPic));
            Thread.Sleep(1000);
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,new SetUIDelegate(unbindPic));
            Thread.Sleep(rd.Next(5000,7000));
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new SetUIDelegate(showColorTextBox));
            processStarted = false;
            
        }

        private void checkSpcaePressed() {
            if (Keyboard.IsKeyDown(Key.Space))
            {
                isSpacePressed = true;
            }
            else {
                isSpacePressed = false;
            }

            /*
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ispressed = true;
            }
            else {
                ispressed = false;
            } */
        }
        private void settipLabelVisibility() {
            Console.WriteLine("setVisibility");
            if (tipLabel.Visibility == Visibility.Visible)
            {
                tipLabel.Visibility = Visibility.Hidden;
            }
            else {
                tipLabel.Visibility = Visibility.Visible;
            }
        }

        private void showColorTextBox() {
            colorTextbox.Text = "";
            colorTextbox.Visibility = Visibility.Visible;
            colorTextbox.Focus();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //FullScreen.GoFullscreen(this);
            setCanvasProperty();
            setImageFileInfo();

            initNDISystem();
        }

        private void setCanvasProperty() {
            //获得屏幕的分辨率
            screenWidth = SystemParameters.PrimaryScreenWidth;
            screenHeight = SystemParameters.PrimaryScreenHeight;

            imageWidth = 500;
            imageHeight = 500;

            //设置图片的宽度高度
            this.centerImage.SetValue(Image.WidthProperty, imageWidth);
            this.centerImage.SetValue(Image.HeightProperty, imageHeight);
            
            //设置图片的位置
            this.centerImage.SetValue(Canvas.LeftProperty, (screenWidth-imageWidth) * 1 / 2);
            this.centerImage.SetValue(Canvas.TopProperty, (screenHeight - imageHeight) * 1 / 2);

            //设置指导语位置
            this.guideTextblock.SetValue(Canvas.LeftProperty, (screenWidth - guideTextblock.Width) * 1 / 2);
            this.guideTextblock.SetValue(Canvas.TopProperty, (screenHeight - guideTextblock.Height) * 1 / 2);
            this.guideTextblock.Visibility = Visibility.Visible;

            //设置十字位置
            this.crossLabel.SetValue(Canvas.LeftProperty, (screenWidth - crossLabel.Width) * 1 / 2);
            this.crossLabel.SetValue(Canvas.TopProperty, (screenHeight - crossLabel.Height) * 1 / 2);

            //设置按住空格键提示语位置
            this.tipLabel.SetValue(Canvas.LeftProperty, (screenWidth - crossLabel.Width) * 1 / 2);
            this.tipLabel.SetValue(Canvas.TopProperty, 0.0);

            //设置输入框的位置
            this.colorTextbox.SetValue(Canvas.LeftProperty, (screenWidth - colorTextbox.Width) * 1 / 2);
            this.colorTextbox.SetValue(Canvas.TopProperty, (screenHeight - colorTextbox.Height) * 1 / 2);

        }

        private void setImageFileInfo()
        {
            if (!Directory.Exists(imagesPath))
            {
                MessageBox.Show("没有找到" + imagesPath + "这个目录！");
            }
            else
            {
                DirectoryInfo directory = new DirectoryInfo(imagesPath);
                imagesFileInfo = directory.GetFiles("*.jpg");
                if (imagesFileInfo != null)
                {
                    threeRandom = new ThreeTimesRandom(imagesFileInfo.Length);
                }
                else
                {
                    MessageBox.Show("没有在 " + imagesPath + " 文件夹下找到 .jpg 文件！");
                }
            }
        }

        private void initNDISystem()
        {
            if (NDILib.initSystem() == 1)
            {
                MessageBox.Show("系统初始化错误！");
                return;
            }
            if (NDILib.initCamera() == 1)
            {
                MessageBox.Show("镜头初始化错误");
                return;
            }
        }

        private void shutdownDNISystem()
        {
            if (NDILib.shutdownSystem() == 1)
            {
                MessageBox.Show("关闭系统出错！");
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !expriStarted) {
                this.guideTextblock.Visibility = Visibility.Hidden;
                this.crossLabel.Visibility = Visibility.Visible;
                expriStarted = true;
                backgroundWorker.RunWorkerAsync(); 
            }
            else if (e.Key==Key.Enter && !processStarted){
                this.colorTextbox.Visibility = Visibility.Hidden;
                recordData();
                this.crossLabel.Visibility = Visibility.Visible;
                reset();
                backgroundWorker.RunWorkerAsync();
            }
            else if (e.Key==Key.Escape){
                FullScreen.ExitFullscreen(this);
                storeData.inputDataCompleted = true;
                Application.Current.Shutdown();
            }
        }

        private void get3DPosition()
        {
            if (NDILib.get3D(out position3D0,out position3D1,out position3D2) == 1)
            {
                //MessageBox.Show("获取Marker点数据出错");
                return;
            }
        }


        private bool bindPic() {
            if (threeRandom.getLeftNumCount() > 0)
            {
                imageCount = threeRandom.getRandomNum();
                picName = imagesFileInfo[imageCount].Name;
                this.crossLabel.Visibility = Visibility.Hidden;
                bitmapImage = new BitmapImage(new Uri(imagesFileInfo[imageCount].FullName, UriKind.Absolute));
                centerImage.Source = bitmapImage;
                centerImage.Visibility = Visibility.Visible;
                showTime = sw.Elapsed.TotalMilliseconds;
                return true;
            }
            else
            {
                MessageBox.Show("没有更多图片了。");
                storeData.inputDataCompleted = true;
                return false;
            }
        }

        private void unbindPic() {
            this.centerImage.Visibility = Visibility.Hidden;
        }

        private void recordData() { 
                
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key==Key.Space){
                if (!hasRecordTime)
                {
                    releaseTime = sw.Elapsed.TotalMilliseconds;
                    hasRecordTime = true;
                }
            }
        }

        private void reset() {
            hasRecordTime = false;
            isSpacePressed = false;
        }
    }
}
