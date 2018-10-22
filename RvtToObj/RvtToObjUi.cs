using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Windows.Forms;

namespace RvtToObj
{
    public partial class RvtToObjUi : System.Windows.Forms.Form
    {
        public string text;
        public static string foldPath = string.Empty;
        ExternalCommandData cmdDataForm;
        ElementSet elementsForm = new ElementSet();
        string msgForm;

        public RvtToObjUi()
        {
            InitializeComponent();
        }

        public RvtToObjUi(ExternalCommandData cmdData, string msg, ElementSet elements)
        {
            InitializeComponent();
            cmdDataForm = cmdData;
            msgForm = msg;
            elementsForm = elements;
        }

        public void SelcetFile()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件保存路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foldPath = dialog.SelectedPath;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            string valu = "";

            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*rvt*)|*.rvt;*.rfa"; //设置要选择的文件的类型

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                SelcetFile();
                this.Close();               
                TaskDialog.Show("info", "开始导入");
                for (int i = 0; i < fileDialog.SafeFileNames.Length; i++)
                {
                    string file = fileDialog.FileNames.GetValue(i).ToString();//返回文件的完整路径 
                    valu = fileDialog.SafeFileNames.GetValue(i).ToString();
                    textBox1.Text += valu;//File.ReadAllText(file, Encoding.GetEncoding("utf-8"));
                    textBox1.Text += Environment.NewLine;

                    Command cl1 = new Command();
                    cl1.getTextPath(file);
                    cl1.Execute(cmdDataForm, ref msgForm, elementsForm);
                }

                if (File.Exists("exceptionfiles.txt"))
                {
                    TaskDialog.Show("Fineshed!", "有部分文件发生异常。"+ Environment.NewLine + "文件名保存在：C:\\exceptionfiles.txt");
                }
                else
                {
                    TaskDialog.Show("Succeeded!", "导出成功!");
                }               
            }
        }
    }
}
