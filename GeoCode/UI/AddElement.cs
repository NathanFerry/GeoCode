/*--------------------------------------------------------------------------------------+
|   AddElement.cs
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

#endregion

namespace GeoCode.UI
{
    public partial class AddElement : UserControl
    {
        #region Bentley DockableWindow
        public static DockableWindow AddElementDockableWindow { get; set; }

        internal static void ShowWindow(string unparsed = "")
        {
            if (null != AddElementDockableWindow)
            {
                AddElementDockableWindow.Focus();
                return;
            }

            AddElementDockableWindow = new DockableWindow();
            AddElementDockableWindow.Content = new AddElement();
            AddElementDockableWindow.Attach(GeoCode.Addin, "control", new System.Drawing.Size(Convert.ToInt32(AddElementDockableWindow.MinWidth),
                        Convert.ToInt32(AddElementDockableWindow.MinHeight)));
            AddElementDockableWindow.WindowContent.CanDockVertically = false;
            AddElementDockableWindow.WindowContent.ContentCloseQuery += new ContentCloseEventHandler(OnClose);
            
            ElementSelector.OwnedWindows.Add(AddElementDockableWindow);
        }

        /// <summary>
        /// Close and dispose the usercontrol.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnClose(object sender, ContentCloseEventArgs e)
        {
            e.CloseAction = ContentCloseAction.Dispose;
            if (null != AddElementDockableWindow)
            {
                AddElementDockableWindow.Detach();
                AddElementDockableWindow.Dispose();
                AddElementDockableWindow = null;
            }
        }
        #endregion
    }
}