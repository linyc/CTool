using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.Globalization;
using System.Reflection;
using System.IO;

namespace CShell
{
    public partial class FrmCShell : Form
    {
        public FrmCShell()
        {
            InitializeComponent();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            string code = this.txtCode.Text;
            // 创建编译器对象
            CSharpCodeProvider p = new CSharpCodeProvider();
            ICodeCompiler cc = p.CreateCompiler();

            // 设置编译参数
            CompilerParameters options = new CompilerParameters();
            options.ReferencedAssemblies.Add("System.dll");
            options.GenerateInMemory = true;
            options.OutputAssembly = "MyTest6";

            // 开始编译
            CodeSnippetCompileUnit cu = new CodeSnippetCompileUnit(code);
            CompilerResults cr = cc.CompileAssemblyFromDom(options, cu);
            try
            {
                // 执行动态程序集相关内容。
                Type t = cr.CompiledAssembly.GetType("MyNamespace.MyClass");
                object o = cr.CompiledAssembly.CreateInstance("MyNamespace.MyClass", false, BindingFlags.Default,
                  null, null, CultureInfo.CurrentCulture, null);
                t.InvokeMember("Test", BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod,
                  null, o, null);
            }
            catch (Exception ex)
            {
                this.txtOutput.Text = ex.StackTrace;
            }
            //using (StringReader sr = new StringReader(code))
            //{

            //    List<string> AssemblyList = new List<string>();
            //    string nameSpace = string.Empty;
            //    string className = string.Empty;

            //    string funcName = this.txtFuncName.Text.Trim();

            //    while (sr.Peek() != -1)
            //    {
            //        string tmpLine = sr.ReadLine().Trim();
            //        if (tmpLine.StartsWith("using"))
            //        {
            //            AssemblyList.Add(tmpLine.Split(' ')[1].Replace(";", ".dll"));
            //        }
            //        else if (tmpLine.StartsWith("namespace"))
            //        {
            //            nameSpace = tmpLine.Split(' ')[1];
            //        }
            //        else if (tmpLine.Contains("class"))
            //        {
            //            className = tmpLine.Substring(tmpLine.LastIndexOf("class ") + 6);
            //        }
            //    }

            //    // 创建编译器对象
            //    CSharpCodeProvider p = new CSharpCodeProvider();
            //    ICodeCompiler cc = p.CreateCompiler();

            //    // 设置编译参数
            //    CompilerParameters options = new CompilerParameters();
            //    options.ReferencedAssemblies.Add("System.dll");
            //    options.GenerateInMemory = true;
            //    options.OutputAssembly = "MyTest";

            //    // 开始编译
            //    CodeSnippetCompileUnit cu = new CodeSnippetCompileUnit(code);
            //    CompilerResults cr = cc.CompileAssemblyFromDom(options, cu);


            //    string tAssembly = string.Format("{0}.{1}", nameSpace, className);

            //    Type t = cr.CompiledAssembly.GetType(tAssembly);
            //    object obj = cr.CompiledAssembly.CreateInstance(tAssembly
            //                                                            , false
            //                                                            , BindingFlags.Default
            //                                                            , null
            //                                                            , null
            //                                                            , CultureInfo.CurrentCulture
            //                                                            , null);
            //    t.InvokeMember(funcName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod, null, obj, null);
            //}
        }
    }
}
