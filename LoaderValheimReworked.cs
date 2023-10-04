using LoaderValheimReworked;
using LoaderValheimReworked.Properties;
using Microsoft.Win32;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoaderValheimReworked
{

    internal class LoaderValheimReworked
    {


        static void Main()
        {
           
            string baseUrl = "http://localhost/";
            string url = "http://localhost/vl_files.list";
            string outputFolderPath = "LoadPlugins";
            string logFilePath = "log.txt";
            // Создание новой папки для сохранения файлов
            Directory.CreateDirectory(outputFolderPath);
   
            // Создание файла для логов
            StreamWriter logFile = new StreamWriter(logFilePath);

            try
            {
                // Загрузка строки из URL
                string fileContent;
                using (WebClient client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    fileContent = client.DownloadString(url);
                }
//Oops
