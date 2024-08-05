/*--------------------------------------------------------------------------------------+
|   SimpleLinearChoice.cs
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

namespace GeoCode.UI.LinearWindows
{
    public partial class SimpleLinearChoice : UserControl
    {
        #region Bentley DockableWindow
        public static DockableWindow SimpleLinearChoiceDockableWindow { get; set; }

        public static void ShowWindow(string unparsed = "")
        {
            if (null != SimpleLinearChoiceDockableWindow)
            {
                SimpleLinearChoiceDockableWindow.Focus();
                return;
            }

            SimpleLinearChoiceDockableWindow = new DockableWindow();
            SimpleLinearChoiceDockableWindow.Content = new SimpleLinearChoice();
            SimpleLinearChoiceDockableWindow.Attach(GeoCode.Addin, "control", new System.Drawing.Size(Convert.ToInt32(SimpleLinearChoiceDockableWindow.MinWidth),
                        Convert.ToInt32(SimpleLinearChoiceDockableWindow.MinHeight)));
            SimpleLinearChoiceDockableWindow.WindowContent.CanDockVertically = false;
            SimpleLinearChoiceDockableWindow.WindowContent.ContentCloseQuery += new ContentCloseEventHandler(OnClose);
        }

        /// <summary>
        /// Close and dispose the usercontrol.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnClose(object sender, ContentCloseEventArgs e)
        {
            e.CloseAction = ContentCloseAction.Dispose;
            if (null != SimpleLinearChoiceDockableWindow)
            {
                SimpleLinearChoiceDockableWindow.Detach();
                SimpleLinearChoiceDockableWindow.Dispose();
                SimpleLinearChoiceDockableWindow = null;
            }
        }
        #endregion
    }
}