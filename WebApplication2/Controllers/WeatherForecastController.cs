using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            
            var rng = new Random();
            string str = CSharpOcr.Load(@"File\249411a.png");
            return str;
        }
       // [HttpPost]
       // public string Post()
       // {
        //    var rng = new Random();
       //     string str = CSharpOcr.Load("");
      //      return str;
       // }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="files">文件流</param>
        /// <returns></returns>
        [HttpPost]
        public string FileSave(List<IFormFile> files)
        {
            if (files.Count < 1)
            {
                return  "空文件" ;
            }
            //返回的文件地址
            List<string> filenames = new List<string>();
            DateTime now = DateTime.Now;
            //文件存储路径
            string filePath = "";
            //获取当前web目录
            var webRootPath = "File/";
            if (!Directory.Exists(webRootPath + filePath))
            {
                Directory.CreateDirectory(webRootPath + filePath);
            }
            try
            {
                string str= webRootPath+ new Random().Next(10000, 9999999) + "a.png";
                foreach (var item in files)
                {
                    if (item != null)
                    {
                        #region  图片文件的条件判断
                        //文件后缀
                        string fileExtension = Path.GetExtension(item.FileName);

                        //判断后缀是否是图片
                        const string fileFilt = ".gif|.jpg|.jpeg|.png";
                        if (fileExtension == null)
                        {
                            break;
                            //return Error("上传的文件没有后缀");
                        }
                        if (fileFilt.IndexOf(fileExtension.ToLower(), StringComparison.Ordinal) <= -1)
                        {
                            break;
                            //return Error("请上传jpg、png、gif格式的图片");
                        }

                        //判断文件大小
                        long length = item.Length;
                        if (length > 1024 * 1024 * 3) //2M
                        {
                            break;
                            //return Error("上传的文件不能大于2M");
                        }

                        #endregion

                        string strDateTime = DateTime.Now.ToString("yyMMddhhmmssfff"); //取得时间字符串
                        string strRan = Convert.ToString(new Random().Next(100, 999)); //生成三位随机数
                        string saveName = strDateTime + strRan + fileExtension;
                        //插入图片数据
                        using (FileStream fs = System.IO.File.Create(str))
                        {
                            item.CopyTo(fs);
                            fs.Flush();
                        }
                        filenames.Add(filePath + saveName);
                    }
                }
                string strs = CSharpOcr.Load(str);
                return  strs ;
            }
            catch (Exception ex)
            {
                //这边增加日志，记录错误的原因
                //ex.ToString();
                return ex.Message+ "上传失败" ;
            }
        }
    }
}
