using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resplaten
{
    class ThreeTimesRandom
    {
        private int num;
        private int leftCount;
        private int[] randomList = null;
        private Random random=null;
        public ThreeTimesRandom(int num) {
            this.num = num;
            this.leftCount = num * 3;
            randomList=new int[leftCount+1];
            for (int i = 0; i < num ; i++) {
                randomList[i] = i;
                randomList[i + num] = i;
                randomList[i + num * 2] = i;
            }
            random = new Random();
        }

        public int getLeftNumCount(){
            return leftCount;
        }

        public int getRandomNum() {
            int index = random.Next(0, leftCount);

            //随机到的数与数组最后一个数交换
            int randomNum = randomList[index];
            randomList[index] = randomList[leftCount - 1];
            randomList[leftCount - 1] = randomNum;

            leftCount--;

            return randomNum;
        }
    }
}
