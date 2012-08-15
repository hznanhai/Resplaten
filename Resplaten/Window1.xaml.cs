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
            ndiBW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ndiBW_RunWorkerCompleted);

            storeData = new StoreData();
            sw = new Stopwatch();
            sw.Start();

            rd = new Random();
        }

        void ndiBW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            storeData.calculData();
        }

        void ndiBW_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true){
                get3DPosition();
                Console.WriteLine("x,y,z:"+position3D0.x+"  "+position3D0.y+"  "+position3D0.z);
                Console.WriteLine("x,y,z:" + position3D1.x + "  " + position3D1.y + "  " + position3D1.z);
                Console.WriteLine("x,y,z:" + position3D2.x + "  " + position3D2.y + "  " + position3D2.z);
                //if (position3D0.x>MAX_NEGATIVE && position3D1.x>MAX_NEGATIVE && position3D2.x>MAX_NEGATIVE){
                    timespan = sw.Elapsed.TotalMilliseconds;
                    storeData.addData(position3D0,position3D1,position3D2,picName,timespan);
                //}
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
            ndiToStop = false;
            ndiBW.RunWorkerAsync();
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new BindPicDispatcherDelegate(bindPic));
            //showTime = sw.Elapsed.TotalMilliseconds;            
            Thread.Sleep(1000);
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background,new SetUIDelegate(unbindPic));
            ndiToStop = true;
            Thread.Sleep(rd.Next(5000,7000));
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new SetUIDelegate(showColorList));
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
            if (tipLabel.Visibility == Visibility.Visible)
            {
                tipLabel.Visibility = Visibility.Hidden;
            }
            else {
                tipLabel.Visibility = Visibility.Visible;
            }
        }

        private void showColorList() {
            initListBox();
            this.allStackPanel.Visibility = Visibility.Visible;
            this.selectStackPanel.Visibility = Visibility.Visible;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FullScreen.GoFullscreen(this);
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

            //设置颜色选框的位置
            initListBox();
            this.allStackPanel.SetValue(Canvas.LeftProperty, screenWidth *(1- 0.618) - allStackPanel.Width * 1 / 2);
            this.allStackPanel.SetValue(Canvas.TopProperty, screenHeight * (1 - 0.618) - allStackPanel.Height * 1 / 2);
            this.selectStackPanel.SetValue(Canvas.LeftProperty, screenWidth * 0.618 - selectStackPanel.Width * 1 / 2);
            this.selectStackPanel.SetValue(Canvas.TopProperty, screenHeight * (1 - 0.618) - selectStackPanel.Height * 1 / 2);
        }

        private void initListBox() {
            allListbox.Items.Clear();
            selectListbox.Items.Clear();

            ListBoxItem listBoxItem = new ListBoxItem();
            listBoxItem.Content = "红";
            listBoxItem.FontSize = 30;
            listBoxItem.Height = 60;
            allListbox.Items.Add(listBoxItem);

            listBoxItem = new ListBoxItem();
            listBoxItem.Content = "橙";
            listBoxItem.FontSize = 30;
            listBoxItem.Height = 60;
            allListbox.Items.Add(listBoxItem);

            listBoxItem = new ListBoxItem();
            listBoxItem.Content = "黄";
            listBoxItem.FontSize = 30;
            listBoxItem.Height = 60;
            allListbox.Items.Add(listBoxItem);

            listBoxItem = new ListBoxItem();
            listBoxItem.Content = "绿";
            listBoxItem.FontSize = 30;
            listBoxItem.Height = 60;
            allListbox.Items.Add(listBoxItem);

            listBoxItem = new ListBoxItem();
            listBoxItem.Content = "蓝";
            listBoxItem.FontSize = 30;
            listBoxItem.Height = 60;
            allListbox.Items.Add(listBoxItem);

            listBoxItem = new ListBoxItem();
            listBoxItem.Content = "紫";
            listBoxItem.FontSize = 30;
            listBoxItem.Height = 60;
            allListbox.Items.Add(listBoxItem);

            listBoxItem = new ListBoxItem();
            listBoxItem.Content = "黑";
            listBoxItem.FontSize = 30;
            listBoxItem.Height = 60;
            allListbox.Items.Add(listBoxItem);
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
                this.allStackPanel.Visibility = Visibility.Hidden;
                this.selectStackPanel.Visibility = Visibility.Hidden;
                recordData();
                this.crossLabel.Visibility = Visibility.Visible;
                reset();
                backgroundWorker.RunWorkerAsync();
            }
            else if (e.Key==Key.Escape){
                FullScreen.ExitFullscreen(this);
                storeData.inputDataCompleted = true;
                storeData.completed();
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
                storeData.addPicName(picName);
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
            string choseColor = "";
            foreach (ListBoxItem item in selectListbox.Items) {
                choseColor += item.Content+",";
            }
            storeData.addSelectColor(choseColor);
            storeData.addtoExcel();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            double uptime = sw.Elapsed.TotalMilliseconds;
            if (e.Key==Key.Space){
                if (!hasRecordTime)
                {
                    releaseTime = uptime;
                    Console.WriteLine("showTime:"+showTime);
                    Console.WriteLine("releaseTime:"+releaseTime);
                    storeData.addReactionTime(releaseTime - showTime);
                    hasRecordTime = true;
                }
            }
        }

        private void reset() {
            hasRecordTime = false;
            isSpacePressed = false;
        }

        private void allListbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem selectItem = (ListBoxItem)allListbox.SelectedItem;
            allListbox.Items.Remove(allListbox.SelectedItem);
            selectListbox.Items.Add(selectItem);
        }

        private void selectListbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem selectItem = (ListBoxItem)selectListbox.SelectedItem;
            selectListbox.Items.Remove(selectListbox.SelectedItem);
            allListbox.Items.Add(selectItem);
        }
    }
}
