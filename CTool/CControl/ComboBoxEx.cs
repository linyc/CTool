using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace CTool.CControl
{
    /// <summary>
    /// 设置item的高度
    /// </summary>
    public partial class ComboBoxEx : System.Windows.Forms.ComboBox
    {
        public ComboBoxEx()
        {
            InitializeComponent();
        }

        protected override void OnDrawItem(System.Windows.Forms.DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            e.DrawBackground();
            e.DrawFocusRectangle();
            e.Graphics.DrawString(this.Items[e.Index].ToString(), e.Font, new System.Drawing.SolidBrush(e.ForeColor), e.Bounds.X, e.Bounds.Y + 3);

            base.OnDrawItem(e);
        }
    }
}
