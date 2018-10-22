using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace UiRibbon
{
    [Transaction(TransactionMode.Automatic)]
    class RibbonUi : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            string TabName = "批量导出";
            application.CreateRibbonTab(TabName);
            RibbonPanel P_TabName1 = application.CreateRibbonPanel(TabName, "BIM");

            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);

            PushButtonData button1 = new PushButtonData("RvtToObj", " 模型转换 ", Path.GetDirectoryName(path) + @"\RvtToObj.dll", "RvtToObj.Command");
            Uri uriImage1 = new Uri(Path.GetDirectoryName(path) + @"\picture\obj.ico");
            BitmapImage largeimage1 = new BitmapImage(uriImage1);
            button1.LargeImage = largeimage1;

            PushButton btn = (PushButton)P_TabName1.AddItem(button1);
            btn.AvailabilityClassName = "RvtToObj.Command";
            return Result.Succeeded;
        }
    }
}


