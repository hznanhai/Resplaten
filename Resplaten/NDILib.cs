using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Resplaten
{
    class NDILib
    {
        [DllImport("NDIDLL.dll", EntryPoint = "myAdd", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int myAdd(int a, int b);

        [DllImport("NDIDLL.dll", EntryPoint = "initSystem", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int initSystem();

        [DllImport("NDIDLL.dll", EntryPoint = "initCamera", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int initCamera();

        [DllImport("NDIDLL.dll", EntryPoint = "shutdownSystem", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int shutdownSystem();

        [DllImport("NDIDLL.dll", EntryPoint = "get3D", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int get3D(out Position3d dtPosition3d0,out Position3d dtPosition3d1,out Position3d dtPosition3d2);

        public struct Position3d {
            public float x;
            public float y;
            public float z;
            public Position3d(int x, int y, int z) {
                this.x = x;
                this.y = y;
                this.z = z;
            }
        }
    }
}
