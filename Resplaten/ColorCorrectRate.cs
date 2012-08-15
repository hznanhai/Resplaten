using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resplaten
{
    class ColorCorrectRate
    {
        public static string[][] colorinfo= new string[][]{
            new string[]{},
            new string[]{},
            new string[]{"蓝","紫"},
            new string[]{"蓝","紫"},
            new string[]{"橙","黄"},
            new string[]{"橙","黄"},
            new string[]{"红","绿"},
            new string[]{"红","绿"},
            new string[]{"紫","橙"},
            new string[]{"紫","橙"},        //10
            new string[]{"黄","绿","红"},
            new string[]{"黄","绿","红"},
            new string[]{"黄","红","橙"},
            new string[]{"黄","红","橙"},
            new string[]{"黄","蓝","橙"},
            new string[]{"黄","蓝","橙"},
            new string[]{"黄","蓝","紫"},
            new string[]{"黄","橙","紫"},
            new string[]{"红","橙","黄","绿"},
            new string[]{"红","橙","黄","绿"},  //20
            new string[]{"红","橙","黄","紫"},
            new string[]{"红","橙","黄","紫"},
            new string[]{"红","橙","蓝","绿"},
            new string[]{"红","橙","蓝","绿"},
            new string[]{"红","橙","蓝","紫"},
            new string[]{"红","橙","蓝","紫"},
            new string[]{"红","橙","蓝","黄","绿"},
            new string[]{"红","橙","蓝","黄","绿"},
            new string[]{"红","橙","蓝","紫","绿"},
            new string[]{"红","橙","蓝","紫","绿"}, //30
            new string[]{"黄","橙","蓝","紫","绿"},
            new string[]{"黄","橙","蓝","紫","绿"},
            new string[]{"红","黄","蓝","紫","绿"},
            new string[]{"红","黄","蓝","紫","绿"},
            new string[]{"黄","橙","蓝","紫","绿","红"},
            new string[]{"黄","橙","蓝","紫","绿","红"},
            new string[]{},
            new string[]{},
            new string[]{"绿","蓝"},
            new string[]{"绿","蓝"},        //40
            new string[]{"黄","红"},
            new string[]{"黄","红"},
            new string[]{"橙","紫"},
            new string[]{"橙","紫"},
            new string[]{"橙","绿","黄"},
            new string[]{"橙","绿","黄"},
            new string[]{"红","蓝","紫"},
            new string[]{"红","蓝","紫"},
            new string[]{"红","蓝","绿"},
            new string[]{"红","蓝","绿"},   //50
            new string[]{"红","绿","紫"},
            new string[]{"红","绿","紫"},
            new string[]{"红","绿","紫","蓝","黄"},
            new string[]{"红","绿","紫","蓝","黄"},
            new string[]{"橙","绿","黄","红"},
            new string[]{"橙","绿","黄","红"},
            new string[]{"橙","蓝","黄","紫"},
            new string[]{"橙","蓝","黄","紫"},
            new string[]{"红","绿","黄","紫"},
            new string[]{"红","绿","黄","紫"},  //60
            new string[]{"红","绿","黄","蓝"},
            new string[]{"红","绿","黄","蓝"}, 
            new string[]{"橙","绿","黄","蓝","紫"},
            new string[]{"橙","绿","黄","蓝","紫"},
            new string[]{"橙","绿","黄","红","紫"},
            new string[]{"橙","绿","黄","红","紫"},
            new string[]{"橙","绿","黄","蓝","紫"},
            new string[]{"橙","绿","黄","蓝","紫"},
            new string[]{"橙","绿","黄","蓝","紫","红"},
            new string[]{"橙","绿","黄","蓝","紫","红"},    //70
            new string[]{},
            new string[]{},
            new string[]{"蓝","黄"},
            new string[]{"蓝","黄"},
            new string[]{"绿","紫"},
            new string[]{"绿","紫"},
            new string[]{"橙","蓝"},
            new string[]{"橙","蓝"},
            new string[]{"红","黄"},
            new string[]{"红","黄"},    //80
            new string[]{"紫","绿","蓝"},
            new string[]{"紫","绿","蓝"},
            new string[]{"红","绿","橙"},
            new string[]{"红","绿","橙"},
            new string[]{"黄","紫","橙"},
            new string[]{"黄","紫","橙"},
            new string[]{"黄","紫","蓝"},
            new string[]{"黄","紫","蓝"},
            new string[]{"红","紫","绿","橙"},
            new string[]{"红","紫","绿","橙"},      //90
            new string[]{"红","紫","黄","蓝"},
            new string[]{"红","紫","黄","蓝"},
            new string[]{"红","紫","绿","橙"},
            new string[]{"红","紫","绿","橙"},  
            new string[]{"红","紫","黄","绿","蓝"},
            new string[]{"红","紫","黄","绿","蓝"},
            new string[]{"红","橙","黄","紫","蓝"},
            new string[]{"红","橙","黄","紫","蓝"},
            new string[]{"红","绿","黄","紫","蓝"},
            new string[]{"红","绿","黄","紫","蓝"},     //100
            new string[]{"橙","绿","黄","紫","蓝"},
            new string[]{"橙","绿","黄","紫","蓝"},
            new string[]{"橙","绿","黄","紫","蓝","红"},
            new string[]{"橙","绿","黄","紫","蓝","红"},
        };
        public static double getCorrectRate(string picname,string selectColor) {
            double rate = 0.0;
            int selectColorNum = 0, rightColorNum = 0;
            string[] splitName = picname.Split('.');
            int picNum = Convert.ToInt32(splitName[0]);
            Console.WriteLine("picName:"+picname);
            string[] selectColorList = selectColor.Split(',');
            if (picNum<colorinfo.Length){
                int colorNum = colorinfo[picNum].Length;
                foreach (string oneSelectColor in selectColorList)
                {
                    if (oneSelectColor != "")
                    {
                        selectColorNum++;
                        for (int i = 0; i < colorNum; i++)
                        {
                            if (oneSelectColor == colorinfo[picNum][i])
                            {
                                rightColorNum++;
                            }
                        }
                    }
                }
                Console.WriteLine("rightNum:" + rightColorNum);
                Console.WriteLine("selectcolorNum:" + selectColorNum);
                Console.WriteLine("allColorNum:"+colorNum);
                if (colorNum != 0)
                {
                    rate = rightColorNum / Convert.ToDouble(colorNum);
                }
                else {
                    if (selectColorNum == 0)
                    {
                        rate = 1;
                    }
                    else {
                        rate = 0;
                    }
                }
            }

            return rate;
        }
    }
}
