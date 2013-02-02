using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace CTool.CControl
{
    public delegate void DataGridViewCheckBoxHeaderCellEventHander(object sender, DataGridViewCheckBoxHeaderCellEventArgs e);

    public class DataGridViewCheckBoxHeaderCellEventArgs : EventArgs
    {
        private bool _checkedState;

        public bool CheckedState
        {
            get { return _checkedState; }
            set { _checkedState = value; }
        }
    }
    /// <summary>
    /// 自定义类，继承DataGridViewColumnHeaderCell实现列头添加CheckedBox
    /// </summary>
    /// <example>
    /// <code>
    /// DataGridViewCheckBoxHeaderCell cbc = new DataGridViewCheckBoxHeaderCell();
    /// cbc.OnCheckBoxClicked += new DataGridViewCheckBoxHeaderCellEventHander(cbc_OnCheckBoxClicked);
    /// DataGridViewCheckBoxColumn cbCloumn = new DataGridViewCheckBoxColumn();
    /// cbCloumn.HeaderCell = cbc;
    /// this.dgvFileData.Columns.Insert(0, cbCloumn);//dgvFileData是datagridview控件
    /// </code>
    /// </example>
    public class DataGridViewCheckBoxHeaderCell : DataGridViewColumnHeaderCell
    {
        Point checkBoxLocation;
        Size checkBoxSize;
        bool _checked = false;
        Point _cellLocation = new Point();
        CheckBoxState _cbState = CheckBoxState.UncheckedNormal;
        public event DataGridViewCheckBoxHeaderCellEventHander OnCheckBoxClicked;


        //绘制列头checkbox 
        protected override void Paint(System.Drawing.Graphics graphics,
                                      System.Drawing.Rectangle clipBounds,
                                      System.Drawing.Rectangle cellBounds,
                                      int rowIndex,
                                      DataGridViewElementStates dataGridViewElementState,
                                      object value,
                                      object formattedValue,
                                      string errorText,
                                      DataGridViewCellStyle cellStyle,
                                      DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                      DataGridViewPaintParts paintParts)
        {
            base.Paint(graphics, clipBounds, cellBounds, rowIndex,
                        dataGridViewElementState, value,
                        formattedValue, errorText, cellStyle,
                        advancedBorderStyle, paintParts);

            Point p = new Point();
            Size s = CheckBoxRenderer.GetGlyphSize(graphics,
            System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);
            p.X = cellBounds.Location.X + (cellBounds.Width / 2) - (s.Width / 2) - 1;//列头checkbox的X坐标 
            p.Y = cellBounds.Location.Y + (cellBounds.Height / 2) - (s.Height / 2);//列头checkbox的Y坐标 
            _cellLocation = cellBounds.Location;
            checkBoxLocation = p;
            checkBoxSize = s;
            if (_checked)
                _cbState = CheckBoxState.CheckedNormal;
            else
                _cbState = CheckBoxState.UncheckedNormal;
            CheckBoxRenderer.DrawCheckBox(graphics, checkBoxLocation, _cbState);
        }


        protected override void OnMouseClick(System.Windows.Forms.DataGridViewCellMouseEventArgs e)
        {
            Point p = new Point(e.X + _cellLocation.X, e.Y + _cellLocation.Y);
            if (p.X >= checkBoxLocation.X && p.X <= checkBoxLocation.X + checkBoxSize.Width
                && p.Y >= checkBoxLocation.Y && p.Y <= checkBoxLocation.Y + checkBoxSize.Height)
            {
                _checked = !_checked;

                //获取列头checkbox的选择状态 
                DataGridViewCheckBoxHeaderCellEventArgs ex = new DataGridViewCheckBoxHeaderCellEventArgs();
                ex.CheckedState = _checked;

                object sender = new object();//此处不代表选择的列头checkbox，只是作为参数传递。应该列头checkbox是绘制出来的，无法获得它的实例

                if (OnCheckBoxClicked != null)
                {
                    OnCheckBoxClicked(sender, ex);//触发单击事件 
                    this.DataGridView.InvalidateCell(this);
                }
            }
            base.OnMouseClick(e);
        }
    }

}