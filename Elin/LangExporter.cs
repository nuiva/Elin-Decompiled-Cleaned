using System;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

public class LangExporter : EClass
{
	public static void Export()
	{
		string str = CorePath.packageCore + "Lang/Template/";
		string str2 = "sample.xlsx";
		XSSFWorkbook xssfworkbook = new XSSFWorkbook();
		LangExporter.currentSheet = xssfworkbook.CreateSheet("newSheet");
		using (FileStream fileStream = new FileStream(str + str2, FileMode.Create))
		{
			xssfworkbook.Write(fileStream);
		}
	}

	public static ICell GetCell(int x, int y)
	{
		IRow row = LangExporter.currentSheet.GetRow(y) ?? LangExporter.currentSheet.CreateRow(y);
		return row.GetCell(x) ?? row.CreateCell(x);
	}

	public static void WriteCell(int x, int y, string value)
	{
		LangExporter.GetCell(x, y).SetCellValue(value);
	}

	public static void WriteCell(int x, int y, double value)
	{
		LangExporter.GetCell(x, y).SetCellValue(value);
	}

	private static ISheet currentSheet;
}
