using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace CTool.CControl
{

    /// <summary>
    /// �ÿؼ����ڷ��ÿ����û��Լ����ô�С��λ�õĿؼ����̳���Panel,ʵ����IControlsMove�ӿ�
    /// Ҫʹ�������ڵĿؼ����ƶ��͸ı䣬��Ҫ�ڳ������ʱ���øÿؼ���InitMouseAndContolStyle(XmlDocument XmlDoc)����
    /// ����:panelEx1.InitMouseAndContolStyle(AppDomain.CurrentDomain.BaseDirectory + "App.xml");
    /// </summary>
    public partial class PanelEx : Panel
    {
        /// <summary>
        /// ���״̬
        /// </summary>
        private enum EnumMousePointPosition
        {
            MouseSizeNone = 0, //'��   
            MouseSizeRight = 1, //'�����ұ߿�   
            MouseSizeLeft = 2, //'������߿�   
            MouseSizeBottom = 3, //'�����±߿�   
            MouseSizeTop = 4, //'�����ϱ߿�   
            MouseSizeTopLeft = 5, //'�������Ͻ�   
            MouseSizeTopRight = 6, //'�������Ͻ�   
            MouseSizeBottomLeft = 7, //'�������½�   
            MouseSizeBottomRight = 8, //'�������½�   
            MouseDrag = 9   // '����϶�   
        }
        #region ����
        private static string xmlDocPath = "";
        private XmlDocument doc;
        private const int Band = 5;
        private const int MinWidth = 10;
        private const int MinHeight = 10;
        private EnumMousePointPosition m_MousePointPosition;
        private Point p, p1;
        #endregion
        public PanelEx()
        {
            InitializeComponent();
        }

        #region �ı�ؼ���С���ƶ�λ���õ��ķ���



        private void MyMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            p.X = e.X;
            p.Y = e.Y;
            p1.X = e.X;
            p1.Y = e.Y;
        }
        /// <summary>
        /// ����뿪�¼���Ҫ�Ľ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyMouseLeave(object sender, EventArgs e)
        {
            Control s = (Control)sender;

            XmlNodeList nodes = doc.GetElementsByTagName(s.Name);
            XmlElement xn;
            if (nodes.Count != 1)
            {
                xn = doc.CreateElement(s.Name);
            }
            else
            {
                xn = (XmlElement)doc.GetElementsByTagName(s.Name)[0];
            }
            xn.SetAttribute("Top", s.Top.ToString());
            xn.SetAttribute("Left", s.Left.ToString());
            xn.SetAttribute("Width", s.Width.ToString());
            xn.SetAttribute("Height", s.Height.ToString());


            XmlNodeList xnl = doc.GetElementsByTagName(this.Name);
            XmlElement xnp;
            if (xnl.Count < 1)
            {
                xnp = doc.CreateElement(this.Name);
            }
            else
            {
                xnp = (XmlElement)xnl[0];
            }
            xnp.AppendChild((XmlNode)xn);
            doc.DocumentElement.AppendChild((XmlNode)xnp);
            doc.Save(xmlDocPath);

            m_MousePointPosition = EnumMousePointPosition.MouseSizeNone;
            this.Cursor = Cursors.Arrow;
        }

        private EnumMousePointPosition MousePointPosition(Size size, System.Windows.Forms.MouseEventArgs e)
        {

            if ((e.X >= -1 * Band) | (e.X <= size.Width) | (e.Y >= -1 * Band) | (e.Y <= size.Height))
            {
                if (e.X < Band)
                {
                    if (e.Y < Band) { return EnumMousePointPosition.MouseSizeTopLeft; }
                    else
                    {
                        if (e.Y > -1 * Band + size.Height)
                        { return EnumMousePointPosition.MouseSizeBottomLeft; }
                        else
                        { return EnumMousePointPosition.MouseSizeLeft; }
                    }
                }
                else
                {
                    if (e.X > -1 * Band + size.Width)
                    {
                        if (e.Y < Band)
                        { return EnumMousePointPosition.MouseSizeTopRight; }
                        else
                        {
                            if (e.Y > -1 * Band + size.Height)
                            { return EnumMousePointPosition.MouseSizeBottomRight; }
                            else
                            { return EnumMousePointPosition.MouseSizeRight; }
                        }
                    }
                    else
                    {
                        if (e.Y < Band)
                        { return EnumMousePointPosition.MouseSizeTop; }
                        else
                        {
                            if (e.Y > -1 * Band + size.Height)
                            { return EnumMousePointPosition.MouseSizeBottom; }
                            else
                            { return EnumMousePointPosition.MouseDrag; }
                        }
                    }
                }
            }
            else
            { return EnumMousePointPosition.MouseSizeNone; }
        }
        private void MyMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Control lCtrl = (sender as Control);

            if (e.Button == MouseButtons.Left)
            {
                switch (m_MousePointPosition)
                {
                    case EnumMousePointPosition.MouseDrag:
                        lCtrl.Left = lCtrl.Left + e.X - p.X;
                        lCtrl.Top = lCtrl.Top + e.Y - p.Y;
                        break;
                    case EnumMousePointPosition.MouseSizeBottom:
                        lCtrl.Height = lCtrl.Height + e.Y - p1.Y;
                        p1.X = e.X;
                        p1.Y = e.Y; //'��¼����϶��ĵ�ǰ��   
                        break;
                    case EnumMousePointPosition.MouseSizeBottomRight:
                        lCtrl.Width = lCtrl.Width + e.X - p1.X;
                        lCtrl.Height = lCtrl.Height + e.Y - p1.Y;
                        p1.X = e.X;
                        p1.Y = e.Y; //'��¼����϶��ĵ�ǰ��   
                        break;
                    case EnumMousePointPosition.MouseSizeRight:
                        lCtrl.Width = lCtrl.Width + e.X - p1.X;
                        //       lCtrl.Height = lCtrl.Height + e.Y - p1.Y;   
                        p1.X = e.X;
                        p1.Y = e.Y; //'��¼����϶��ĵ�ǰ��   
                        break;
                    case EnumMousePointPosition.MouseSizeTop:
                        lCtrl.Top = lCtrl.Top + (e.Y - p.Y);
                        lCtrl.Height = lCtrl.Height - (e.Y - p.Y);
                        break;
                    case EnumMousePointPosition.MouseSizeLeft:
                        lCtrl.Left = lCtrl.Left + e.X - p.X;
                        lCtrl.Width = lCtrl.Width - (e.X - p.X);
                        break;
                    case EnumMousePointPosition.MouseSizeBottomLeft:
                        lCtrl.Left = lCtrl.Left + e.X - p.X;
                        lCtrl.Width = lCtrl.Width - (e.X - p.X);
                        lCtrl.Height = lCtrl.Height + e.Y - p1.Y;
                        p1.X = e.X;
                        p1.Y = e.Y; //'��¼����϶��ĵ�ǰ��   
                        break;
                    case EnumMousePointPosition.MouseSizeTopRight:
                        lCtrl.Top = lCtrl.Top + (e.Y - p.Y);
                        lCtrl.Width = lCtrl.Width + (e.X - p1.X);
                        lCtrl.Height = lCtrl.Height - (e.Y - p.Y);
                        p1.X = e.X;
                        p1.Y = e.Y; //'��¼����϶��ĵ�ǰ��   
                        break;
                    case EnumMousePointPosition.MouseSizeTopLeft:
                        lCtrl.Left = lCtrl.Left + e.X - p.X;
                        lCtrl.Top = lCtrl.Top + (e.Y - p.Y);
                        lCtrl.Width = lCtrl.Width - (e.X - p.X);
                        lCtrl.Height = lCtrl.Height - (e.Y - p.Y);
                        break;
                    default:
                        break;
                }
                if (lCtrl.Width < MinWidth) lCtrl.Width = MinWidth;
                if (lCtrl.Height < MinHeight) lCtrl.Height = MinHeight;

            }
            else
            {
                m_MousePointPosition = MousePointPosition(lCtrl.Size, e);   //'�жϹ���λ��״̬   
                switch (m_MousePointPosition)   //'�ı���   
                {
                    case EnumMousePointPosition.MouseSizeNone:
                        this.Cursor = Cursors.Arrow;        //'��ͷ   
                        break;
                    case EnumMousePointPosition.MouseDrag:
                        this.Cursor = Cursors.SizeAll;      //'�ķ���   
                        break;
                    case EnumMousePointPosition.MouseSizeBottom:
                        this.Cursor = Cursors.SizeNS;       //'�ϱ�   
                        break;
                    case EnumMousePointPosition.MouseSizeTop:
                        this.Cursor = Cursors.SizeNS;       //'�ϱ�   
                        break;
                    case EnumMousePointPosition.MouseSizeLeft:
                        this.Cursor = Cursors.SizeWE;       //'����   
                        break;
                    case EnumMousePointPosition.MouseSizeRight:
                        this.Cursor = Cursors.SizeWE;       //'����   
                        break;
                    case EnumMousePointPosition.MouseSizeBottomLeft:
                        this.Cursor = Cursors.SizeNESW;     //'����������   
                        break;
                    case EnumMousePointPosition.MouseSizeBottomRight:
                        this.Cursor = Cursors.SizeNWSE;     //'���ϵ�����   
                        break;
                    case EnumMousePointPosition.MouseSizeTopLeft:
                        this.Cursor = Cursors.SizeNWSE;     //'���ϵ�����   
                        break;
                    case EnumMousePointPosition.MouseSizeTopRight:
                        this.Cursor = Cursors.SizeNESW;     //'����������   
                        break;
                    default:
                        break;
                }
            }

        }


        #endregion

        #region ��ʼ������¼�ί�кͿؼ���С���ƶ�
        private void initProperty()
        {
            for (int i = 0; i < this.Controls.Count; i++)
            {
                this.Controls[i].MouseDown += new System.Windows.Forms.MouseEventHandler(MyMouseDown);
                this.Controls[i].MouseLeave += new System.EventHandler(MyMouseLeave);
                this.Controls[i].MouseMove += new System.Windows.Forms.MouseEventHandler(MyMouseMove);
            }

        }
        private void initStyle()
        {
            Control s;
            for (int i = 0; i < this.Controls.Count; i++)
            {
                s = this.Controls[i];
                XmlNodeList nodes = doc.GetElementsByTagName(s.Name);
                if (nodes.Count == 1)
                {
                    XmlAttributeCollection xac = nodes[0].Attributes;
                    foreach (XmlAttribute xa in xac)
                    {
                        if (xa.Value == "")
                            continue;
                        switch (xa.Name)
                        {
                            case "Top":
                                var Top = Convert.ToInt32(xa.Value);
                                if (Top > 0 && Top < this.Height - s.Height)
                                    s.Top = Top;
                                break;
                            case "Left":
                                var Left = Convert.ToInt32(xa.Value);
                                if (Left > 0 && Left < this.Width - s.Width)
                                    s.Left = Left;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        #endregion
        /// <summary>
        /// ����ʵ�������ڿؼ��ƶ��͸ı��С�ķ���
        /// </summary>
        /// <param name="XmlDoc">���ڱ���ؼ������Ե�XML�ĵ�</param>
        public void InitMouseAndContolStyle(string XmlDocPath)
        {
            xmlDocPath = XmlDocPath;
            doc = new XmlDocument();
            doc.Load(XmlDocPath);
            initProperty();
            initStyle();
        }


        string xml���� = @"<?xml version='1.0' encoding='utf-8'?>
                            <Form1>
                              <panel1>
                                <button1 Top='26' Left='60' Height='23' Width='75' />
                                <textBox1 Top='46' Left='157' Width='100' Height='21' />
                              </panel1>
                              <panelEx2>
                                <button2 Top='89' Left='246' Width='75' Height='23' />
                              </panelEx2>
                            </Form1>";
        string �������� = @"panelEx2.InitMouseAndContolStyle(AppDomain.CurrentDomain.BaseDirectory + 'file.xml');";
    }


}
