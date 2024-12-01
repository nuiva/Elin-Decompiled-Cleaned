using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

public class LangExporter : EClass
{
	private static ISheet currentSheet;

	public static void Export()
	{
		string text = CorePath.packageCore + "Lang/Template/";
		string text2 = "sample.xlsx";
		XSSFWorkbook xSSFWorkbook = new XSSFWorkbook();
		currentSheet = xSSFWorkbook.CreateSheet("newSheet");
		using FileStream stream = new FileStream(text + text2, FileMode.Create);
		xSSFWorkbook.Write(stream);
	}

	public static ICell GetCell(int x, int y)
	{
		IRow row = currentSheet.GetRow(y) ?? currentSheet.CreateRow(y);
		return row.GetCell(x) ?? row.CreateCell(x);
	}

	public static void WriteCell(int x, int y, string value)
	{
		GetCell(x, y).SetCellValue(value);
	}

	public static void WriteCell(int x, int y, double value)
	{
		GetCell(x, y).SetCellValue(value);
	}
}
