/*--------------------------------------------------------------------------------------+
|   RenameCategory.cs
|
+--------------------------------------------------------------------------------------*/

#region System Namespaces
using System;
using System.Windows.Controls;
#endregion

#region Bentley Namespaces
using Bentley.DgnPlatformNET;
using Bentley.MstnPlatformNET.WPF;
using Bentley.Windowing;

#endregion

namespace GeoCode.UI.CategoryWindows
{
    public partial class RenameCategory : UserControl
    {
        #region Bentley DockableWindow
        private static DockableWindow RenameCategoryDockableWindow { get; set; }

        public static void ShowWindow(string unparsed = "")
        {
            if (null != RenameCategoryDockableWindow)
            {
                RenameCategoryDockableWindow.Focus();
                return;
            }

            RenameCategoryDockableWindow = new DockableWindow();
            RenameCategoryDockableWindow.Content = new RenameCategory();
            RenameCategoryDockableWindow.Attach(GeoCode.Addin, "control", new System.Drawing.Size(Convert.ToInt32(RenameCategoryDockableWindow.MinWidth),
                        Convert.ToInt32(RenameCategoryDockableWindow.MinHeight)));
            RenameCategoryDockableWindow.WindowContent.CanDockVertically = false;
            RenameCategoryDockableWindow.WindowContent.ContentCloseQuery += new ContentCloseEventHandler(OnClose);
        }

        /// <summary>
        /// Close and dispose the usercontrol.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnClose(object sender, ContentCloseEventArgs e)
        {
            e.CloseAction = ContentCloseAction.Dispose;
            if (null != RenameCategoryDockableWindow)
            {
                RenameCategoryDockableWindow.Detach();
                RenameCategoryDockableWindow.Dispose();
                RenameCategoryDockableWindow = null;
            }
        }
        #endregion
    }
}