using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.IO;
#if R2016
using Autodesk.Revit.Utility;
#elif R2018
using Autodesk.Revit.DB.Visual;
#endif
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RvtToObj
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Command : IExternalCommand, IExternalCommandAvailability
    {
        List<string> files = new List<string>();
        public static string foldPath = string.Empty;

        /// <summary>
        /// 导出三维视图，调用CustomExporter.Export
        /// </summary>
        /// <param name="view3d"></param>
        /// <param name="filename"></param>
        public void ExportView3D(View3D view3d, string filename, AssetSet objlibraryAsset)
        {
            Document doc = view3d.Document;
            RvtExportContext context = new RvtExportContext(doc, filename, objlibraryAsset);
            CustomExporter exporter = new CustomExporter(doc, context);
            exporter.ShouldStopOnError = false;
            exporter.Export(view3d);
        }

        /// <summary>
        /// 选择文件保存的路径
        /// </summary>
        public void SelcetFile()
        {          
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件保存路径";        
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foldPath = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// 选择所要操作的文件
        /// </summary>
        private bool FilesOption()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;

            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "所有文件(*rvt*)|*.rvt;*.rfa"; //设置要选择的文件的类型

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show( "即将导出，接下来选择导出路径！", "通知", MessageBoxButtons.OK);
                SelcetFile();           
                for (int i = 0; i < fileDialog.SafeFileNames.Length; i++)
                {
                    string file = fileDialog.FileNames.GetValue(i).ToString();//返回文件的完整路径 
                    files.Add(file);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 创建文件保存的具体形式。
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private string CreatFilePath(Document doc)
        {
            string filename = doc.PathName;
            if (0 == filename.Length)
            {
                filename = doc.Title;
            }
            filename = Path.GetFileNameWithoutExtension(filename) + ".obj";
            string subPath = foldPath + "\\" + Path.GetFileNameWithoutExtension(filename);
            if (!Directory.Exists(subPath))
            {
                Directory.CreateDirectory(subPath);
            }
            filename = Path.Combine(subPath + "\\" + filename);
            return filename;
        }

        /// <summary>
        /// 可在打开revit时使用按钮，不然必须在打开文件后才可以。
        /// </summary>
        /// <param name="applicationData"></param>
        /// <param name="selectedCategories"></param>
        /// <returns></returns>
        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            return true;
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if(FilesOption())
            {
                foreach (var file in files)
                {
                    string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                    UriBuilder uri = new UriBuilder(codeBase);
                    string path = Uri.UnescapeDataString(uri.Path);
                    var placeholderFile = Path.GetDirectoryName(path) + @"\placeholderFile.rte";

                    UIApplication uiapp = commandData.Application;
                    UIDocument uidoc = uiapp.OpenAndActivateDocument(file);
                    Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
                    Document doc = uidoc.Document;
                    AssetSet objlibraryAsset = app.get_Assets(AssetType.Appearance);

                    #region///判断是否为空视图,若为空视图，切换为3d视图
                    View3D view = doc.ActiveView as View3D;
                    if (null == view)
                    {
                        IEnumerable<ViewFamilyType> viewFamilyTypes = from elem in new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType))
                                                                      let type = elem as ViewFamilyType
                                                                      where type.ViewFamily == ViewFamily.ThreeDimensional
                                                                      select type;
                        Transaction ts = new Transaction(doc, "Change3D");
                        try
                        {
                            ts.Start();
                            XYZ direction = new XYZ(-1, 1, -1);
                            View3D view3D = View3D.CreateIsometric(doc, viewFamilyTypes.First().Id);
                            //View3D view3D = uiDoc.Document.Create.NewView3D(new XYZ(-1, 1, -1));//斜视45度
                            ts.Commit();
                            //切换视图必须在事务结束后，否则会提示错误：
                            //Cannot change the active view of a modifiable document
                            uidoc.ActiveView = view3D;
                        }
                        catch (Exception ex)
                        {
                            TaskDialog.Show("ex", ex.ToString());
                            ts.RollBack();
                        }
                        //Util.ErrorMsg("You must be in a 3D view to export.");
                    }
                    #endregion

                    ExportView3D(doc.ActiveView as View3D, CreatFilePath(doc), objlibraryAsset);

                    //通过占位文件关闭当前文件
                    var docPlaceholder = commandData.Application.OpenAndActivateDocument(placeholderFile);
                    doc.Close(false);
                }
                MessageBox.Show("导出成功！", "通知");
            }            
            return Result.Succeeded;
        }
    }
}
