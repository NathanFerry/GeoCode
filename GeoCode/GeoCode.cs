/*--------------------------------------------------------------------------------------+
|   Program.cs
|
|   Main entry class establishing a connection to the MicroStation host.
|
+--------------------------------------------------------------------------------------*/

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using Bentley.MstnPlatformNET;
using GeoCode.Cells.Placement;
using GeoCode.Model;
using Newtonsoft.Json;
using GeoCode.Cells;
using GeoCode.Saving;
using GeoCode.UI;

namespace GeoCode
{
    [AddIn(MdlTaskID = "GeoCode")]
    internal sealed class GeoCode : AddIn
    {
        public static GeoCode Addin = null;
        public static Application Application;

        private GeoCode(System.IntPtr mdlDesc) : base(mdlDesc)
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
            Bentley.MstnPlatformNET.MessageCenter.Instance.ShowInfoMessage(message, "", false);
        }

        /// <summary>
        /// Handles MicroStation UNLOADED event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void GeoCode_UnloadedEvent(AddIn sender, UnloadedEventArgs eventArgs)
        {
            var message = "Unloaded [Reason : " + eventArgs.UnloadKind + "]";
            Bentley.MstnPlatformNET.MessageCenter.Instance.ShowInfoMessage(message, "", false);
        }

        /// <summary>
        /// Handles MDL ONUNLOAD requests when the application is being unloaded.
        /// </summary>
        /// <param name="eventArgs"></param>
        protected override void OnUnloading(UnloadingEventArgs eventArgs)
        {
            base.OnUnloading(eventArgs);
        }
        
        internal static GeoCode Instance()
        {
            return Addin;
        }
    }
}