using System;
using System.Drawing;
using System.Windows.Forms;
namespace TqkLibrary.WinApi
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SpyAgent
    {
        internal class SpyAgentLocator : Form
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
        }
    }
}