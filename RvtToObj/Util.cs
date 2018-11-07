using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Diagnostics;
using WinForms = System.Windows.Forms;

namespace RvtToObj
{
    class Util
    {
        const string _caption = "RevitToObj";
        public static void ErrorMsg(string msg)
        {
            Debug.WriteLine(msg);
            WinForms.MessageBox.Show(msg,
                                    _caption,
                                    WinForms.MessageBoxButtons.OK,
                                    WinForms.MessageBoxIcon.Error
                                    );
        }

        public static string RealString(double a)
        {
            return a.ToString("0.##########");
        }

        public static string PointString(XYZ p)
        {
            return string.Format("({0},{1},{2})",
              RealString(p.X),
              RealString(p.Y),
              RealString(p.Z));
        }

        public static int ColorToInt(Color color)
        {
            return color.Red << 16 | color.Green << 8 | color.Blue;
        }

        static string ColorString(Color color)//指定X2，16进制显示的更整齐
        {
            return color.Red.ToString("X2") + color.Green.ToString("X2") + color.Blue.ToString("X2");
        }
        public static string ColorTransparencyString(Color color, int transparency)
        {
            return transparency.ToString("X2") + ColorString(color);
        }

        public static Color IntToColor(int rgb)
        {
            return new Color((byte)((rgb & 0xFF0000) >> 16), (byte)((rgb & 0xFF00) >> 8), (byte)(rgb & 0xFF));
        }
    }
}
