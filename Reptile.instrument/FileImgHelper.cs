using System;
using System.IO;
using System.Net;

namespace Reptile.instrument
{
    public static class FileImgHelper
    {
       public static string FileName = string.Empty;
        static FileImgHelper()
        {
            FileName = AppDomain.CurrentDomain.BaseDirectory + "img\\";
            if (!Directory.Exists(FileName))
            {
                Directory.CreateDirectory(FileName);
            }
        }

        /// <summary>
        /// 从图片地址下载图片到本地磁盘
        /// </summary>
        /// <param name="ToLocalPath">图片本地磁盘地址
        /// <param name="Url">图片网址
        /// <returns></returns>
        public static bool DownImage(string Url)
        {
            bool Value = false;
            WebResponse response = null;
            Stream stream = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

                response = request.GetResponse();
                stream = response.GetResponseStream();

                if (!response.ContentType.ToLower().StartsWith("text/"))
                {
                    Value = SaveBinaryFile(response, $"{FileName}\\{DateTime.Now.ToString("yyyyMMddHHmmss")}.jpg");
                }
            }
            catch (Exception err)
            {
                string aa = err.ToString();
            }
            return Value;
        }
        /// <summary>
        /// Save a binary file to disk.
        /// </summary>
        /// <param name="response">The response used to save the file
        // 将二进制文件保存到磁盘
        private static bool SaveBinaryFile(WebResponse response, string FileName)
        {
            bool Value = true;
            byte[] buffer = new byte[1024];

            try
            {
                if (File.Exists(FileName))
                    File.Delete(FileName);
                Stream outStream = System.IO.File.Create(FileName);
                Stream inStream = response.GetResponseStream();

                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0)
                        outStream.Write(buffer, 0, l);
                }
                while (l > 0);

                outStream.Close();
                inStream.Close();
            }
             catch (Exception ex)
            {
                Value = false;
            }
            return Value;
        }

        /// <summary>
        /// 异步下载
        /// </summary>
        /// <param name="url"></param>
        public static void Down(string url)
        {
            string FileName = FileImgHelper.FileName + DateTime.Now.ToString("yyyyMMddHHmmssssssss") + ".jpg";
            try
            {
                if (!File.Exists(FileName))
                {
                    WebClient client = new WebClient();
                    //client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                    client.DownloadFileAsync(new Uri(url), FileName);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
