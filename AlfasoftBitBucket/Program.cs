using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AlfasoftBitBucket
{
    class Program
    {
        static void Main(string[] args)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //Getting last time the app runned
            var lastRunTime = config.AppSettings.Settings["lastRunTime"].Value;
            DateTime currentTime = DateTime.Now;
            string path = string.Empty;
            bool validPath = false;


            if (String.IsNullOrEmpty(lastRunTime) || (currentTime - DateTime.Parse(lastRunTime)).TotalSeconds > 60)
            {
                while (!validPath)
                {
                    Console.WriteLine("Please, insert a valid path to the users file:");
                    path = Console.ReadLine();

                    validPath = IsPathValid(path);

                    if (!validPath)
                        Console.WriteLine("The path is invalid");
                    else
                        Console.WriteLine("Starting to process the file...");
                }


                string[] usersFile = ReturnUsers(path);

                if (usersFile.Length == 0)
                {
                    Console.WriteLine("No users were found in the file!");
                    CloseApp(config);
                }


                Console.WriteLine("\n -------------------------------------------------------------------------------------------- \n");

                foreach (string user in usersFile)
                {
                    bool last = usersFile.Last() == user;
                    GetUserData(user);

                    if (!last)
                        Thread.Sleep(5000);
                    else
                        CloseApp(config);
                }
            }

            else
            {
                Console.WriteLine("Last time run was less than 60 seconds, please try again later");
                CloseApp(config, false);
            }

        }

        private static bool IsPathValid(string path)
        {
            //Checking if the file is a txt
            if (!path.Contains(".txt"))
                return false;
            //Else, check if the path exists
            return File.Exists(path);
        }

        private static string[] ReturnUsers(string path)
        {
            return File.ReadAllLines(path);
        }

        private static void CloseApp(Configuration config, bool rewrite = true)
        {
            //Rewriting last time runned
            if (rewrite)
            {
                config.AppSettings.Settings["lastRunTime"].Value = DateTime.Now.ToString();
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            
            for (int i = 5; i > 0; i--)
            {
                Console.WriteLine("Closing the app in {0} seconds...", i);
                Thread.Sleep(1000);
            }

            Environment.Exit(0);
        }

        private static async void GetUserData(string user)
        {
            string baseUrl = "https://api.bitbucket.org/2.0/users/" + user;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    Console.WriteLine("User {0}", user);
                    Console.WriteLine("Requesting {0}", baseUrl);

                    HttpResponseMessage response = await client.GetAsync(baseUrl);

                    string data = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response: {0}", data);

                    Console.WriteLine("\n -------------------------------------------------------------------------------------------- \n");

                    LogRequestToFile(user, baseUrl, data);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error ocurred while trying to make a request to the server: {0}", e.Message);
            }
            
        }


        private static void LogRequestToFile(string user, string request, string data)
        {
            string currentPath = Directory.GetCurrentDirectory();
            string filePath = Path.Combine(currentPath, "requests-log.txt");
            try
            {
                using (StreamWriter writer = File.AppendText(filePath))
                {
                    writer.WriteLine("-------------------------------------------------------------------------------------");
                    writer.WriteLine("Time requested: {0}", DateTime.Now.ToString());
                    writer.WriteLine("User: {0}", user);
                    writer.WriteLine("Request: {0}", request);
                    writer.WriteLine("Response: {0}", data);
                    writer.WriteLine("-------------------------------------------------------------------------------------");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error ocurred while trying to writing to a file: {0}", e.Message);
            }
            

        }
    }
}
