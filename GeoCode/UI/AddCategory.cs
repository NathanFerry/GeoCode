/*--------------------------------------------------------------------------------------+
|   AddCategory.cs
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
    public partial class AddCategory : UserControl
    {
        #region Bentley DockableWindow
        private static DockableWindow AddCategoryDockableWindow { get; set; }

        internal static void ShowWindow(string unparsed = "")
        {
            if (null != AddCategoryDockableWindow)
            {
                AddCategoryDockableWindow.Focus();
                return;
            }

            AddCategoryDockableWindow = new DockableWindow();
            AddCategoryDockableWindow.Content = new AddCategory();
            AddCategoryDockableWindow.MinWidth = ((AddCategory)AddCategoryDockableWindow.Content).MinWidth;
            AddCategoryDockableWindow.MinHeight = ((AddCategory)AddCategoryDockableWindow.Content).MinHeight;
            AddCategoryDockableWindow.Attach(GeoCode.Addin, "control", new System.Drawing.Size(Convert.ToInt32(AddCategoryDockableWindow.MinWidth),
                        Convert.ToInt32(AddCategoryDockableWindow.MinHeight)));
            AddCategoryDockableWindow.WindowContent.CanDockVertically = false;
            AddCategoryDockableWindow.WindowContent.ContentCloseQuery += new ContentCloseEventHandler(OnClose);
            
            ElementSelector.OwnedWindows.Add(AddCategoryDockableWindow);
        }

        /// <summary>
        /// Close and dispose the usercontrol.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnClose(object sender, ContentCloseEventArgs e)
        {
            e.CloseAction = ContentCloseAction.Dispose;
            if (null != AddCategoryDockableWindow)
            {
                AddCategoryDockableWindow.Detach();
                AddCategoryDockableWindow.Dispose();
                AddCategoryDockableWindow = null;
            }
        }
        #endregion
    }
}