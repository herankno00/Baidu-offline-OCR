using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace WebApplication2
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LicenseLocalInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string license_key;   // 用户设定的license key
        public int algorithm_id;  // SDK使用的算法ID
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string package_name;         // 本地包名
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string md5;                  // 本地包MD5签名
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string device_id;            // 本地设备ID
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string finger_version;       // 获取设备指纹库版本号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string license_sdk_version;  // 鉴权库版本号
    }

    class OcrLicense
    {
        // 通过授权 (传授权文件所在文件夹路径)
        [DllImport("GenerOCRApi.dll", EntryPoint = "auth_from_folder", CharSet = CharSet.Ansi
             , CallingConvention = CallingConvention.Cdecl)]
        public static extern int auth_from_folder(string license_folder, bool is_remote = true);
        // 通过授权 (传授权文件所在路径和授权文件key)
        [DllImport("GenerOCRApi.dll", EntryPoint = "auth_from_file", CharSet = CharSet.Ansi
             , CallingConvention = CallingConvention.Cdecl)]
        public static extern int auth_from_file(string license_key, string license_path, bool is_remote = true);
        // 获取本地信息
        [DllImport("GenerOCRApi.dll", EntryPoint = "get_local_info", CharSet = CharSet.Ansi
             , CallingConvention = CallingConvention.Cdecl)]
        public static extern int get_local_info(IntPtr out_ptr);

        // 获取本地信息示例
        public void get_local_info_demo()
        {
            LicenseLocalInfo loc_info = new LicenseLocalInfo();
            int sizeInfo = Marshal.SizeOf(typeof(LicenseLocalInfo));
            IntPtr ptT = Marshal.AllocHGlobal(sizeInfo);
            int res = get_local_info(ptT);
            if (res == 0)
            {
                IntPtr ptr = new IntPtr();
                if (8 == IntPtr.Size)
                {
                    ptr = (IntPtr)(ptT.ToInt64());
                }
                else if (4 == IntPtr.Size)
                {
                    ptr = (IntPtr)(ptT.ToInt32());
                }
                loc_info = (LicenseLocalInfo)Marshal.PtrToStructure(ptr, typeof(LicenseLocalInfo));
                // Console.WriteLine("license key is {0}", loc_info.license_key);               
                // Console.WriteLine("algorithm_id is {0}", loc_info.algorithm_id);
                // Console.WriteLine("package_name is {0}", loc_info.package_name);
                // Console.WriteLine("md5 is {0}", loc_info.md5);
                // Console.WriteLine("device_id is {0}", loc_info.device_id);
                // Console.WriteLine("finger_version is {0}", loc_info.finger_version);
                // Console.WriteLine("license_sdk_version is {0}", loc_info.license_sdk_version);
            }
        }

        // 通过授权示例(传授权文件夹，方式一）
        public int ocr_auth_from_folder()
        {
            // license_folder为激活文件的文件夹路径
            // 传空为采用默认路径，若想定置化路径，请填写全局路径如：d:\\ocr （models模型文件夹目录放置后为d:\\ocr\\license）
            // 亦可在激活文件默认生成的路径，激活文件生成的方法请参考文档
            string license_folder = null;
            int res = auth_from_folder(license_folder);
            return res;
        }

        // 通过授权示例(传授权文件夹，方式二）
        public int ocr_auth_from_file()
        {
            // license_pah为激活文件license.ini的文件相对路径或绝对路径
            // key传授权文件license.key的文件内容
            string license_key = "../license/license.key";
            string key = System.IO.File.ReadAllText(license_key);
            string license_path = "../license/license.ini";
            int res = auth_from_file(key, license_path);
            return res;
        }
    }
}
