/*--------------------------------------------------------------------------------------+
|   Program.cs
|
|   Main entry class establishing a connection to the MicroStation host.
|
|   Addin originally created by Nathan Ferry and Enzo Bertel.
|   Some documentation will be dispatched to help you understand either the code or Microstation's SDK.
|
+--------------------------------------------------------------------------------------*/

using Bentley.MstnPlatformNET;
using GeoCode.Model;
using GeoCode.Utils;
using System;
using System.IO;
using System.Reflection;

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
        private static readonly string DgnLibPath = DirectoryInfo.FullName + Path.DirectorySeparatorChar + "GEO.dgnlib";

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

            
            Session.Instance.Keyin("ATTACH LIBRARY "+CellLibraryFilePath);
            Session.Instance.Keyin("ATTACH LIBRARY " + DgnLibPath);

            if (!DgnHelper.LoadOtherDgnFile(DgnLibPath)) { Log.Write("DGN non chargé"); };
            if (!DgnHelper.LoadOtherDgnFile(CellLibraryFilePath)) { Log.Write("DGN non chargé"); };

            if (!DgnHelper.LocateAllCellsModels())
            {
                Log.Write("Aucune cellules impoortées depuis les bibliothèque de cellules");
            }

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