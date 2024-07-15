using System;
using System.IO;
using System.Reflection;

namespace GeoCode.Utils
{
    class Log
    {
        private static readonly DirectoryInfo DirectoryInfo =
        new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.CreateSubdirectory("GeoCode");

        private static readonly string _fullFileName = DirectoryInfo.FullName + Path.DirectorySeparatorChar + "Log.txt";

        /// <summary>
        /// Ecrit dans le log le message
        /// </summary>
        /// <param name="Message"> string : le message à écrire </param>
        public static void Write(string Message)
        {
            using (StreamWriter file = new StreamWriter(_fullFileName, append: true))
            {
                file.WriteLine(DateTime.Now + " : " + Message);
            }
        }
    }
}

