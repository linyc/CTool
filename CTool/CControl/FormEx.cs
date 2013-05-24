using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading;
using System.Drawing;

namespace CN100.Assistant.Main.Class
{
    public static class FormEx
    {
        static float[][] MatrixArray ={ 
                new float[] {1, 0, 0, 0, 0},
                new float[] {0, 1, 0, 0, 0},
                new float[] {0, 0, 1, 0, 0},
                new float[] {0, 0, 0, 0.5f, 0}, //注意：此处为0.5f，图像为半透明
                new float[] {0, 0, 0, 0, 1}};


        private static Image ImageOpacity(Image img, double opacity)
        {
            ColorMatrix matrix = new ColorMatrix(MatrixArray);
            matrix.Matrix33 = (float)opacity;
            ImageAttributes imgAttributes = new ImageAttributes();
            imgAttributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            var image = new Bitmap(img.Width, img.Height);
            var g = Graphics.FromImage(image);
            Rectangle rect = new Rectangle(Point.Empty, img.Size);
            g.DrawImage(img, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, imgAttributes);
            g.Dispose();
            return image;
        }

        public static void Loading(this Control ctl,bool show = true)
        {
            if (show)
            {
                var rect = ctl.DisplayRectangle;
                var image = new Bitmap(rect.Width, rect.Height);
                ctl.DrawToBitmap(image, ctl.DisplayRectangle);
                var image2 = ImageOpacity(image, 0.2);
                image.Dispose();

                var pic = new PictureBox();
                pic.Name = "LoadingPic";
                pic.BackgroundImage = image2;
                pic.Image = global::CN100.Assistant.Main.Properties.Resources.loading;
                pic.SizeMode = PictureBoxSizeMode.CenterImage;
                pic.Parent = ctl;

                ctl.SuspendLayout();
                pic.Dock = DockStyle.Fill;
                pic.BringToFront();
                pic.Show();
                ctl.ResumeLayout();
            }
            else
            {
                UnLoading(ctl);
            }
        }
        
        public static void UnLoading(this Control ctl)
        {
            if (IsInvalid(ctl.Controls["LoadingPic"]) == false)
            {
                ctl.Controls["LoadingPic"].Dispose();
            }
        }


        /// <summary> 判断窗体对象是否是无效的
        /// </summary>
        /// <param name="frm"></param>
        /// <returns></returns>
        public static bool IsInvalid(this Control frm)
        {
            return frm == null || frm.Disposing || frm.IsDisposed;
        }

        /// <summary> 在窗体同步线程上执行委托
        /// </summary>
        /// <param name="frm"></param>
        /// <param name="action"></param>
        public static void Invoke(this Control frm, ThreadStart action)
        {
            if (!IsInvalid(frm))
            {
                try
                {
                    frm.Invoke(action);
                }
                catch (ObjectDisposedException)
                {

                }
                catch
                {
                    throw;
                }
            }
        }


        static Dictionary<Type, Dictionary<string, Form>> _Cache = new Dictionary<Type, Dictionary<string, Form>>();
        /// <summary>
        /// 获取一个单例对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static T Singleton<T>(string flag = "")
            where T : Form, new()
        {
            Form frm;
            Dictionary<string, Form> forms;
            if (_Cache.TryGetValue(typeof(T), out forms) == false)
            {
                forms = new Dictionary<string, Form>();
                _Cache[typeof(T)] = forms;
            }

            if (!forms.TryGetValue(flag, out frm) || FormEx.IsInvalid(frm))
            {
                forms.Remove(flag);
                frm = new T();
                Info info = new Info()
                {
                    Dict = forms,
                    Form = frm,
                    Flag = flag
                };
                frm.Disposed += new EventHandler(info.FormDisposed);
                forms.Add(flag, frm);
            }

            return (T)frm;
        }

        /// <summary>
        /// 获取一个单例对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static Form Singleton(Type formType,string flag = "")
        {
            if (typeof(Form).IsAssignableFrom(formType) == false)
            {
                throw new Exception("必须提供一个Form类型对象");
            }
            Form frm;
            Dictionary<string, Form> forms;
            if (_Cache.TryGetValue(formType, out forms) == false)
            {
                forms = new Dictionary<string, Form>();
                _Cache[formType] = forms;
            }

            if (!forms.TryGetValue(flag, out frm) || FormEx.IsInvalid(frm))
            {
                forms.Remove(flag);
                var ctor = formType.GetConstructor(Type.EmptyTypes);
                if (ctor == null)
	            {
                    throw new Exception("窗体没有默认的构造函数");
	            }
                frm = (Form)ctor.Invoke(null);
                Info info = new Info()
                {
                    Dict = forms,
                    Form = frm,
                    Flag = flag
                };
                frm.Disposed += new EventHandler(info.FormDisposed);
                forms.Add(flag, frm);
            }

            return frm;
        }

        class Info
        {
            public Dictionary<string, Form> Dict { get; set; }
            public Form Form { get; set; }
            public string Flag { get; set; }

            public void FormDisposed(object sender, EventArgs e)
            {
                Dict.Remove(Flag);
            }
        }

        //支持透明的控件
        class OpacityPicture : Control
        {
            public OpacityPicture()
            {
                float[][] ptsArray ={ 
                new float[] {1, 0, 0, 0, 0},
                new float[] {0, 1, 0, 0, 0},
                new float[] {0, 0, 1, 0, 0},
                new float[] {0, 0, 0, 0.5f, 0}, //注意：此处为0.5f，图像为半透明
                new float[] {0, 0, 0, 0, 1}};

                ColorMatrix = new ColorMatrix(ptsArray);
                Opacity = 1;
                SetStyle(ControlStyles.SupportsTransparentBackColor
              | ControlStyles.UserPaint
              , true);
                this.BackColor = Color.Transparent;
            }
            public ColorMatrix ColorMatrix { get; private set; }
            public Image Image { get; set; }
            float _Opacity;
            public float Opacity
            {
                get { return _Opacity; }
                set
                {
                    _Opacity = value;
                    base.Invalidate();
                }
            }

            protected override void OnPaintBackground(PaintEventArgs pevent)
            {
                base.OnPaintBackground(pevent);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (e.ClipRectangle.IsEmpty)
                {
                    return;
                }
                //var img = new Bitmap(this.Width, this.Height);
                //var g = Graphics.FromImage(img);
                var g = e.Graphics;
                // g.Clear(Color.Transparent);
                //g.DrawImage(this.BackgroundImage, Point.Empty);//绘制背景


                ColorMatrix.Matrix33 = Opacity;
                ImageAttributes imgAttributes = new ImageAttributes();
                imgAttributes.SetColorMatrix(ColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                g.DrawImage(Image, e.ClipRectangle, 0, 0, this.Width, this.Height, GraphicsUnit.Pixel, imgAttributes);


                //e.Graphics.DrawImage(img, Point.Empty);
                //g.Dispose();
                //img.Dispose();
            }
        }
    }
}
