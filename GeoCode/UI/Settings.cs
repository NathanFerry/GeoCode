/*--------------------------------------------------------------------------------------+
|   Settings.cs
|
+--------------------------------------------------------------------------------------*/


using System.Windows;

#region System Namespaces

using System;
using System.Windows.Controls;
#endregion

#region Bentley Namespaces
using Bentley.DgnPlatformNET;
using Bentley.MstnPlatformNET.WPF;
using Bentley.Windowing;
using GeoCode.Utils;

#endregion

namespace GeoCode.UI
{
    public partial class Settings : UserControl
    {
        #region Bentley DockableWindow
        private static DockableWindow SettingsDockableWindow { get; set; }

        internal static void ShowWindow(string unparsed = "")
        {
            if (null != SettingsDockableWindow)
            {
                Log.Write("Settings Window existe déjà. Focus");
                SettingsDockableWindow.Focus();
                return;
            }

            Log.Write("Création Settings Window");

            SettingsDockableWindow = new DockableWindow();
            SettingsDockableWindow.Title = "Geocode paramètres";
            SettingsDockableWindow.Content = new Settings();
            SettingsDockableWindow.Attach(GeoCode.Addin, "control", new System.Drawing.Size(Convert.ToInt32(SettingsDockableWindow.MinWidth),
                        Convert.ToInt32(SettingsDockableWindow.MinHeight)));
            SettingsDockableWindow.WindowContent.CanDockVertically = true;
            SettingsDockableWindow.WindowContent.ContentCloseQuery += new ContentCloseEventHandler(OnClose);
        }

        /// <summary>
        /// Close and dispose the usercontrol.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnClose(object sender, ContentCloseEventArgs e)
        {
            e.CloseAction = ContentCloseAction.Dispose;
            if (null == SettingsDockableWindow) return;
            SettingsDockableWindow.Detach();
            SettingsDockableWindow.Dispose();
            SettingsDockableWindow = null;
        }
        #endregion
    }
}