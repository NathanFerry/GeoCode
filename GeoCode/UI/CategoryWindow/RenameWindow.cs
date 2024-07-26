/*--------------------------------------------------------------------------------------+
|   RenameWindow.cs
|
+--------------------------------------------------------------------------------------*/

#region System Namespaces
using System;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
#endregion

#region Bentley Namespaces
using Bentley.DgnPlatformNET;
using Bentley.MstnPlatformNET.WPF;
using Bentley.Windowing;

#endregion

namespace GeoCode.UI.CategoryWindows
{
    public partial class RenameWindow : UserControl
    {
        #region Bentley DockableWindow
        public static DockableWindow RenameWindowDockableWindow { get; set; }

        public static void ShowWindow(string unparsed = "")
        {
            if (null != RenameWindowDockableWindow)
            {
                RenameWindowDockableWindow.Focus();
                return;
            }

            RenameWindowDockableWindow = new DockableWindow();
            RenameWindowDockableWindow.Content = new RenameWindow();
            RenameWindowDockableWindow.Title = "Renommer catégorie";
            RenameWindowDockableWindow.Attach(GeoCode.Addin, "control", new System.Drawing.Size(Convert.ToInt32(RenameWindowDockableWindow.MinWidth),
                        Convert.ToInt32(RenameWindowDockableWindow.MinHeight)));
            RenameWindowDockableWindow.WindowContent.CanDockVertically = false;
            RenameWindowDockableWindow.WindowContent.ContentCloseQuery += new ContentCloseEventHandler(OnClose);
        }

        /// <summary>
        /// Close and dispose the usercontrol.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnClose(object sender, ContentCloseEventArgs e)
        {
            RenameWindow.cat = null;
            e.CloseAction = ContentCloseAction.Dispose;
            if (null != RenameWindowDockableWindow)
            {
                RenameWindowDockableWindow.Detach();
                RenameWindowDockableWindow.Dispose();
                RenameWindowDockableWindow = null;
            }
        }
        #endregion
    }
}