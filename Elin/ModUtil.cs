using System;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEngine;

public class ModUtil : EClass
{
	public static void Test()
	{
		ModUtil.ImportExcel("", "", EClass.sources.charas);
	}

	public static void ImportExcel(string pathToExcelFile, string sheetName, SourceData source)
	{
		Debug.Log("ImportExcel source:" + ((source != null) ? source.ToString() : null) + " Path:" + pathToExcelFile);
		using (FileStream fileStream = File.Open(pathToExcelFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			XSSFWorkbook xssfworkbook = new XSSFWorkbook(fileStream);
			for (int i = 0; i < xssfworkbook.NumberOfSheets; i++)
			{
				ISheet sheetAt = xssfworkbook.GetSheetAt(i);
				if (!(sheetAt.SheetName != sheetName))
				{
					Debug.Log("Importing Sheet:" + sheetName);
					try
					{
						if (!source.ImportData(sheetAt, new FileInfo(pathToExcelFile).Name, true))
						{
							Debug.LogError(ERROR.msg);
							break;
						}
						Debug.Log("Imported " + sheetAt.SheetName);
						source.Reset();
					}
					catch (Exception ex)
					{
						Debug.LogError(string.Concat(new string[]
						{
							"[Error] Skipping import ",
							sheetAt.SheetName,
							" :",
							ex.Message,
							"/",
							ex.Source,
							"/",
							ex.StackTrace
						}));
						break;
					}
				}
			}
		}
	}
}
