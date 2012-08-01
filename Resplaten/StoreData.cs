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
        private const double keySpeed = 0.1;

        private  ApplicationClass excelApp = null;
        private Workbooks workbooks = null;
        private Workbook workbook = null;
        private Worksheet worksheet = null;
        private int dataRowNum = 2;

        private BackgroundWorker backgroundWorker;
        private ExcelData[] allDataList;
        private int allDataLength;
        public bool storeCompleted = false;
        public bool inputDataCompleted = false;

        public struct ExcelData {
            public string picName;
            public bool direct;
            public double time;
            public float x;
            public float y;
            public float z;
            public double m_range;
            public double m_speed;
            public double m_acce;
            public ExcelData(string picName,bool direct,double time, float x, float y, float z, double range, double speed, double acce) {
                this.picName = picName;
                this.direct = direct;
                this.time = time;
                this.x = x;
                this.y = y;
                this.z = z;
                this.m_range = range;
                this.m_speed = speed;
                this.m_acce = acce;
            }
        }

        public StoreData(){
            initExcel();
            allDataList = new ExcelData[65535];
            allDataLength = 0;
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            backgroundWorker.RunWorkerAsync();
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.storeCompleted = true;
        }

        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        private void initExcel() {
            try { 
                excelApp = new ApplicationClass();
                workbooks = excelApp.Workbooks;

                workbook = workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                worksheet =(Worksheet) workbook.Worksheets[1];

                worksheet.Cells[1, 1] = "Frame";
                worksheet.Cells[1, 2] = "Picture";
                worksheet.Cells[1, 3] = "Direction";
                worksheet.Cells[1, 4] = "Time";
                worksheet.Cells[1, 5] = "X";
                worksheet.Cells[1, 6] = "Y";
                worksheet.Cells[1, 7] = "Z";
                worksheet.Cells[1, 8] = "Range";
                worksheet.Cells[1, 9] = "Speed";
                worksheet.Cells[1, 10] = "Acceleration";

            }
            catch(Exception exp){
                throw(exp);
            }
        }

        public void addData(NDILib.Position3d position3D0,NDILib.Position3d position3D1,NDILib.Position3d position3D2,string picName,double timespan) { 
            
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
