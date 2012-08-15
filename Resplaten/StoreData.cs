using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.ComponentModel;
using System.Collections;
using System.Threading;

namespace Resplaten
{
    class StoreData
    {
        private const string excelFileDir = "d:\\ResplatenData\\"; //excel文件夹目录
        private const float MAX_NEGATIVE = (float)-3.0E28;   //小于该值，Marker点MISS
        private const double keySpeed = 0.1;

        private  ApplicationClass excelApp = null;
        private Workbooks workbooks = null;
        private Workbook workbook = null;
        private Worksheet worksheet = null;
        private int dataRowNum = 2;

        private BackgroundWorker backgroundWorker;
        private CollectData[] allDataList;
        private int allDataLength;
        public bool calculCompleted = false;
        public bool inputDataCompleted = false;

        public double maxSpeed=0, maxGrasp=0;
        public string pictureName, selectColor;
        public double reactionTime;

        NDILib.Position3d position3D0, position3D1, position3D2;

        public struct CollectData {
            public string picName;
            public double timespan;
            public NDILib.Position3d position0, position1, position2;
            public CollectData(string picName,double timespan,NDILib.Position3d position0,NDILib.Position3d position1,NDILib.Position3d position2) {
                this.picName = picName;
                this.timespan = timespan;
                this.position0 = position0;
                this.position1 = position1;
                this.position2 = position2;
            }
        }

        public StoreData(){
            initExcel();
            allDataList = new CollectData[65535];
            allDataLength = 0;
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.calculCompleted = true;
            allDataLength = 0;
        }

        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            maxSpeed = 0;
            maxGrasp = 0;
            NDILib.Position3d oldposition = allDataList[0].position2;
            double oldtime = allDataList[0].timespan;
            double oldspeed = 0;
            for (int i = 0; i < allDataLength;i++ ) { 
                position3D0=allDataList[i].position0;
                position3D1=allDataList[i].position1;
                position3D2=allDataList[i].position2;

                if (position3D0.x > MAX_NEGATIVE && position3D1.x > MAX_NEGATIVE) {
                    double distance = Math.Sqrt((position3D0.x - position3D1.x) * (position3D0.x - position3D1.x) +
                                                (position3D0.y - position3D1.y) * (position3D0.y - position3D1.y) +
                                                (position3D0.z - position3D1.z) * (position3D0.z - position3D1.z));

                    Console.WriteLine("distance:" + distance);
                    if (distance > maxGrasp)
                    {
                        maxGrasp = distance;
                    }                    
                }

                if (position3D2.x>MAX_NEGATIVE){
                    double time = allDataList[i].timespan - oldtime;
                    if (time != 0)
                    {
                        double range = Math.Sqrt((position3D2.x - oldposition.x) * (position3D2.x - oldposition.x) +
                                                 (position3D2.y - oldposition.y) * (position3D2.y - oldposition.y) +
                                                 (position3D2.z - oldposition.z) * (position3D2.z - oldposition.z));
                        double speed = range / time;
                        double acce = 1000 * (speed - oldspeed) / time;
                        Console.WriteLine("speed:" + speed);
                        Console.WriteLine("acce: " + acce);
                        if (acce < 15 && acce > -15)
                        {
                            if (speed > maxSpeed)
                            {
                                maxSpeed = speed;
                            }
                            oldposition = position3D2;
                            oldtime = allDataList[i].timespan;
                            oldspeed = speed;
                        }
                    }
                }
            }
        }

        private void initExcel() {
            try { 
                excelApp = new ApplicationClass();
                workbooks = excelApp.Workbooks;

                workbook = workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                worksheet =(Worksheet) workbook.Worksheets[1];

                worksheet.Cells[1, 1] = "No";
                worksheet.Cells[1, 2] = "Picture";
                worksheet.Cells[1, 3] = "Time";
                worksheet.Cells[1, 4] = "CorrectRate";
                worksheet.Cells[1, 5] = "MaxGrasp";
                worksheet.Cells[1, 6] = "MaxSpeed";
                worksheet.Cells[1, 7] = "DetailData";
            }
            catch(Exception exp){
                throw(exp);
            }
        }

        public void addData(NDILib.Position3d position3D0,NDILib.Position3d position3D1,NDILib.Position3d position3D2,string picName,double timespan) {
            allDataList[allDataLength] = new CollectData(picName,timespan,position3D0,position3D1,position3D2);
            allDataLength++;
        }

        public void calculData() {
            backgroundWorker.RunWorkerAsync();
        }

        public void addSelectColor(string color) {
            this.selectColor = color;
        }

        public void addReactionTime(double timespan) {
            this.reactionTime = timespan;
        }
        
        public void addPicName(string picName){
            this.pictureName = picName;
        }

        public void addtoExcel() {
            worksheet.Cells[dataRowNum, 1] = dataRowNum - 1;
            worksheet.Cells[dataRowNum, 2] = pictureName;
            worksheet.Cells[dataRowNum, 3] = reactionTime;
            worksheet.Cells[dataRowNum, 4] = ColorCorrectRate.getCorrectRate(pictureName,selectColor);
            worksheet.Cells[dataRowNum, 5] = maxGrasp;
            worksheet.Cells[dataRowNum, 6] = maxSpeed;
            worksheet.Cells[dataRowNum, 7] = selectColor;
            dataRowNum++;
        }

        public void completed() {
            try { 
                if (!Directory.Exists(excelFileDir)){
                    Directory.CreateDirectory(excelFileDir);
                }
                string subDir = excelFileDir + DateTime.Now.ToString("yyyy_MM_dd")+"\\";
                if (!Directory.Exists(subDir)){
                    Directory.CreateDirectory(subDir);
                }
                string nowTime = DateTime.Now.ToString("HH_mm_ss");

                string excelFilePath = subDir + "ALLData_" + nowTime + ".xls";
                workbook.SaveAs(excelFilePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                ReleaseCOM(worksheet);
                ReleaseCOM(workbook);
                ReleaseCOM(workbooks);
                excelApp.Quit();
                ReleaseCOM(excelApp);
            }
            catch(Exception exp){
                throw(exp);
            }
        }

        private void ReleaseCOM(object pObj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pObj);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                pObj = null;
            }
        }
    }
}
