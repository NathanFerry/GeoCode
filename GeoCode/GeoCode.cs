/*--------------------------------------------------------------------------------------+
|   Program.cs
|
|   Main entry class establishing a connection to the MicroStation host.
|
|   Addin originally created by Nathan Ferry and Enzo Bertel.
|   Some documentation will be dispatched to help you understand either the code or Microstation's SDK.
|
+--------------------------------------------------------------------------------------*/

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Bentley.DgnPlatformNET;
using Bentley.MstnPlatformNET;
using Bentley.MstnPlatformNET.InteropServices;
using GeoCode.Model;
using GeoCode.Utils;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GeoCode
{
    [AddIn(MdlTaskID = "GeoCode")]
    internal sealed class GeoCode : AddIn
    {
        public static GeoCode Addin = null;
        public static Application Application;

        private static readonly DirectoryInfo DirectoryInfo =
       new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.CreateSubdirectory("GeoCode");

        private static readonly string CellLibraryFilePath = DirectoryInfo.FullName + Path.DirectorySeparatorChar + "cellules_GEO_V2.cel";

        private GeoCode(IntPtr mdlDesc) : base(mdlDesc)
        {
            Addin = this;
        }

        /// <summary>
        /// Initializes the AddIn. Called by the AddIn loader after
        /// it has created the instance of this AddIn class
        /// </summary>
        /// <param name="commandLine"></param>
        /// <returns>0 on success</returns>
        protected override int Run(string[] commandLine)
        {
            // Register event handlers
            Addin.ReloadEvent += GeoCode_ReloadEvent;
            Addin.UnloadedEvent += GeoCode_UnloadedEvent;


            if (!DgnHelper.LoadOtherDgnFile(CellLibraryFilePath)) { Log.Write("DGN non chargé"); };

            //========================================
            // Open user interface
            Session.Instance.Keyin("GeoCode Interface Show");
            
            return 0;
        }

        ///<summary>
        /// Handles MDL ONUNLOAD requests when the application Is being unloaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void GeoCode_ReloadEvent(AddIn sender, ReloadEventArgs eventArgs)
        {
            
            var message = "Reloaded " + eventArgs.CommandLine[0];
            MessageCenter.Instance.ShowInfoMessage(message, "", false);
        }

        /// <summary>
        /// Handles MicroStation UNLOADED event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void GeoCode_UnloadedEvent(AddIn sender, UnloadedEventArgs eventArgs)
        {
            var message = "Unloaded [Reason : " + eventArgs.UnloadKind + "]";
            MessageCenter.Instance.ShowInfoMessage(message, "", false);
        }
        
        internal static GeoCode Instance()
        {
            return Addin;
        }
    }
}