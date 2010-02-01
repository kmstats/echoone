using System;
using System.Collections.Generic;
using System.Text;

namespace com.echo.File
{
    //Excel相关操作
    static class ExcelUtil
    {
        //取得单元格合适字体大小
        public static float GetFitFontSize(Microsoft.Office.Interop.Excel.Range range)
        {
            float oldHeight = float.Parse(range.RowHeight.ToString());

            range.Rows.AutoFit();
            float newHeight = float.Parse(range.RowHeight.ToString());

            while (newHeight - oldHeight > 0)
            {
                range.Font.Size = newHeight - oldHeight > 0 ? float.Parse(range.Font.Size.ToString()) - 0.5 : float.Parse(range.Font.Size.ToString());
                range.RowHeight = oldHeight;
                range.Rows.AutoFit();
                newHeight = float.Parse(range.RowHeight.ToString());
            }

            range.RowHeight = oldHeight;
            return float.Parse(range.Font.Size.ToString());
        }

        //设置指定单元格边框 range——单元格
        public static void SetCellBorder(Microsoft.Office.Interop.Excel.Range range)
        {
            range.Select();
            range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlDiagonalDown].LineStyle = (object)(-4142);
            range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlDiagonalUp].LineStyle = (object)(-4142);
            range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeLeft].LineStyle = (object)(1);
            range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeRight].LineStyle = (object)(1);
            range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeBottom].LineStyle = (object)(1);
            range.Borders[Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop].LineStyle = (object)(1);
        }

        //取得sheet by name
        public static Microsoft.Office.Interop.Excel.Worksheet GetSheetByName(Microsoft.Office.Interop.Excel.Application myExcel, string name)
        {
            Microsoft.Office.Interop.Excel.Worksheet sheet = null;
            Microsoft.Office.Interop.Excel.Sheets sheets = myExcel.Sheets;
            foreach (Microsoft.Office.Interop.Excel.Worksheet s in sheets)
            {
                if (s.Name == name)
                {
                    sheet = s;
                    break;
                }
            }
            return sheet;
        }

 
    }
}
