using System;
using System.Drawing;
namespace TqkLibrary.WinApi
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SpyAgent
    {
        internal class SpyAgentLocator
        {
            public SpyAgentLocator()
            {
                FormBorderStyle = FormBorderStyle.None;
                BackColor = Color.OrangeRed;
                Opacity = 0.25;
                TopLevel = true;
                TopMost = true;
                ShowInTaskbar = false;
            }
            public IntPtr Handle { get; set; }
            public double Opacity { get; set; }
            public Point Location { get; set; }
            public Size Size { get; set; }
            public bool TopLevel { get; set; }
            public bool TopMost { get; set; }
            public bool ShowInTaskbar { get; set; }
            public Color BackColor { get; set; }
            public FormBorderStyle FormBorderStyle { get; set; }
            public void Show()
            {

            }
            public void Hide()
            {

            }
            public void Close()
            {

            }
            public void Dispose()
            {

            }
        }
        /// <summary>
        /// Specifies the border styles for a form.
        /// </summary>
        public enum FormBorderStyle
        {
            /// <summary>
            /// No border.
            /// </summary>
            None = 0,
            /// <summary>
            /// A fixed, single-line border.
            /// </summary>
            FixedSingle = 1,
            /// <summary>
            /// A fixed, three-dimensional border.
            /// </summary>
            Fixed3D = 2,
            /// <summary>
            /// A thick, fixed dialog-style border.
            /// </summary>
            FixedDialog = 3,
            /// <summary>
            /// A resizable border.
            /// </summary>
            Sizable = 4,
            /// <summary>
            /// A tool window border that is not resizable. A tool window does not appear in
            /// the taskbar or in the window that appears when the user presses ALT+TAB. Although
            /// forms that specify System.Windows.Forms.FormBorderStyle.FixedToolWindow typically
            /// are not shown in the taskbar, you must also ensure that the System.Windows.Forms.Form.ShowInTaskbar
            /// property is set to false, since its default value is true.
            /// </summary>
            FixedToolWindow = 5,
            /// <summary>
            /// A resizable tool window border. A tool window does not appear in the taskbar
            /// or in the window that appears when the user presses ALT+TAB.
            /// </summary>
            SizableToolWindow = 6
        }
    }
}