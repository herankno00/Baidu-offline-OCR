using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using OpenCvSharp;

// sdk使用注意事项，使用sdk前，请参考文档进行授权激活，否则
// sdk初始化可能失败，可根据文档错误码判断。

// ocr c#入口类
namespace WebApplication2
{
    class CSharpOcr
    {
        
        // sdk初始化
        [DllImport("GenerOCRApi.dll", EntryPoint = "init", CharSet = CharSet.Ansi
            , CallingConvention = CallingConvention.Cdecl)]
        public static extern int init(string model_path, int thread_num = 4);
        // ocr 识别
        [DllImport("GenerOCRApi.dll", EntryPoint = "process", CharSet = CharSet.Ansi
               , CallingConvention = CallingConvention.Cdecl)]
        public static extern int process(IntPtr mat, int type, IntPtr out_ptr);
        // sdk销毁
        [DllImport("GenerOCRApi.dll", EntryPoint = "uninit", CharSet = CharSet.Ansi
            , CallingConvention = CallingConvention.Cdecl)]
        public static extern void uninit();

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct OCRResponse
        {
            public int x1;
            public int y1;
            public int x2;
            public int y2;
            public int x3;
            public int y3;
            public int x4;
            public int y4;
            public float left;
            public float top;
            public float width;
            public float height;
            public float score;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4096)]
            public byte[] result;  // 识别结果     
        }

        // 读取单个文件进行ocr识别示例
        public static string image_test(string img_path)
        {
            int max_num = 1000;
            OCRResponse[] resp_info = new OCRResponse[max_num];
            int sizeResp = Marshal.SizeOf(typeof(OCRResponse));
            IntPtr ptT = Marshal.AllocHGlobal(sizeResp * max_num);
            Mat mat = Cv2.ImRead(img_path);
            int type = 0;
            int resCode = process(mat.CvPtr, type, ptT);
            if (resCode < 0)
            {
                Marshal.FreeHGlobal(ptT);
                return "ocr process fail and error code " + resCode;
            }
            string txt = "";
            int array_size = resCode;
            for (int index = 0; index < array_size; index++)
            {

                IntPtr ptr = new IntPtr();
                if (8 == IntPtr.Size)
                {
                    ptr = (IntPtr)(ptT.ToInt64() + sizeResp * index);
                }
                else if (4 == IntPtr.Size)
                {
                    ptr = (IntPtr)(ptT.ToInt32() + sizeResp * index);
                }

                resp_info[index] = (OCRResponse)Marshal.PtrToStructure(ptr, typeof(OCRResponse));
                //Console.WriteLine("x1 is {0}", resp_info[index].x1);
                //Console.WriteLine("y1 is {0}", resp_info[index].y1);
                //Console.WriteLine("x2 is {0}", resp_info[index].x2);
                //Console.WriteLine("y2 is {0}", resp_info[index].y2);

                //Console.WriteLine("x3 is {0}", resp_info[index].x3);
                //Console.WriteLine("y3 is {0}", resp_info[index].y3);
                //Console.WriteLine("x4 is {0}", resp_info[index].x4);
                //Console.WriteLine("y4 is {0}", resp_info[index].y4);          
                byte[] buf = Encoding.Convert(Encoding.Default, Encoding.Unicode, resp_info[index].result, 0, 4096);
                string str = Marshal.PtrToStringAuto(Marshal.UnsafeAddrOfPinnedArrayElement(buf, 0));
                //Console.WriteLine("result:" + str);
                txt += str;
            }
            Marshal.FreeHGlobal(ptT);
            return txt;

        }
    //进行遍历整个文件夹，对所有图片进行ocr识别示例
        public static void folder_test()
        {
            List<string> results = new List<string>();
            // 文件夹，请填入图片所在文件夹的相对路径或绝对路径
            string folder = @"../images";
            DirectoryInfo directory = new DirectoryInfo(folder);
            try
            {
                if (!directory.Exists)
                {
                    // Console.WriteLine("文件夹不存在, 请检查");
                    return;
                }
            }
            catch (Exception e)
            {
                //  Console.WriteLine("文件夹不存在异常");
            }
            // 遍历文件夹下的扩展名为jpg的所有文件
            string[] files = Directory.GetFiles(folder,
                "*.jpg", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                // Console.WriteLine("file is:" + file);
                image_test(file);
            }
        }

        // ocr c#入口方法
        public static string Load(string paths)
        {
            OcrLicense ocrlicense = new OcrLicense();
            int auth = ocrlicense.ocr_auth_from_file();
            string txt;
            if (auth != 0)
            {
                txt = "auth fail and error code is " + auth;
                return txt;
            }
            string model_folder = null;
            int thread_num = 4;  
            int n = init(model_folder, thread_num);
            if (n != 0)
            {
                txt = "sdk init fail and errcode is " + n;
                return txt;
            }
            string img_path = paths;
            txt = CSharpOcr.image_test(img_path);
            uninit();
            return txt;
          
        }
    }
}
