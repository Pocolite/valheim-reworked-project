using System;
using System.IO;
using System.Net;
using Loader;
using Loader.Properties;

namespace Loader
{
    class LoaderV
    {
        static void Main()
        {
            string baseUrl = http://localhost; //Изменён
            string url = http://localhost/vp.txt; //Изменён
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
                    fileContent = client.DownloadString(url);
                }

                // Разделение строки на отдельные пути
                string[] lines = fileContent.Split('\n');

                foreach (string line in lines)
                {
                    try
                    {
                        // Удаление символа перевода строки и пробелов
                        string cleanLine = line.Trim();

                        // Игнорирование пустых строк и строк, не содержащих точку с запятой
                        if (string.IsNullOrEmpty(cleanLine) || !cleanLine.Contains(";"))
                            continue;

                        // Разделение строки на путь и хеш
                        string[] parts = cleanLine.Split(';');
                        string path = parts[0];

                        // Формирование полного пути файла
                        string filePath = Path.Combine(outputFolderPath, path);

                        // Проверка, существует ли файл уже в папке
                        if (File.Exists(filePath))
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"Файл уже загружен: {path}");
                            Console.ResetColor();
                            continue;
                        }

                        // Создание папок, если они не существуют
                        string directoryPath = Path.GetDirectoryName(filePath);
                        Directory.CreateDirectory(directoryPath);

                        // Формирование полной ссылки
                        string fullUrl = baseUrl + path;

                        // Получение размера файла
                        long fileSize;
                        WebRequest request = WebRequest.Create(fullUrl);
                        using (WebResponse response = request.GetResponse())
                        {
                            fileSize = response.ContentLength;
                        }
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($"Размер файла: {GetSizeString(fileSize)} >");
                        Console.ResetColor();

                        // Загрузка файла по ссылке
                        using (WebClient client = new WebClient())
                        {
                            client.DownloadProgressChanged += (sender, e) =>
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"Загрузка файла: {e.ProgressPercentage}% ({GetSizeString(e.BytesReceived)} / {GetSizeString(e.TotalBytesToReceive)})");
                                Console.ResetColor();
                            };

                            client.DownloadFile(fullUrl, filePath);

                            // Логирование действия
                            string logMessage = $"Загружен файл: {path}";
                            logFile.WriteLine(logMessage);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(logMessage);
                            Console.ResetColor();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Ошибка при загрузке файла: {ex.Message}");
                        logFile.WriteLine($"Ошибка при загрузке файла: {ex.Message}");
                        Console.ResetColor();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка при загрузке списка файлов: {ex.Message}");
                logFile.WriteLine($"Ошибка при загрузке списка файлов: {ex.Message}");
                Console.ResetColor();
            }
            finally
            {
                // Закрытие файла для логов
                logFile.Close();

                Console.WriteLine("Загрузка файлов завершена.");
            }
        }

        // Метод для форматирования размера файла в читаемый вид (Кб, Мб, Гб)
        static string GetSizeString(long size)
        {
            string[] sizes = { "Б", "Кб", "Мб", "Гб" };
            double len = size;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
