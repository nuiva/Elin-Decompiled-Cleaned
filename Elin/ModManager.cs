using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using HeathenEngineering.SteamworksIntegration;
using HeathenEngineering.SteamworksIntegration.API;
using IniParser;
using IniParser.Model;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ModManager : BaseModManager
{
	public static List<string> ListChainLoad
	{
		get
		{
			return BaseModManager.listChainLoad;
		}
	}

	public static DirectoryInfo DirWorkshop
	{
		get
		{
			return BaseModManager.Instance.dirWorkshop;
		}
	}

	public static bool IsInitialized
	{
		get
		{
			return BaseModManager.isInitialized;
		}
	}

	public static string PathIni
	{
		get
		{
			return CorePath.PathIni;
		}
	}

	public IniData GetElinIni()
	{
		IniData result;
		try
		{
			FileIniDataParser fileIniDataParser = new FileIniDataParser();
			if (!File.Exists(ModManager.PathIni))
			{
				File.CreateText(ModManager.PathIni).Close();
			}
			IniData iniData = fileIniDataParser.ReadFile(ModManager.PathIni, Encoding.UTF8);
			if (iniData.GetKey("pass").IsEmpty())
			{
				string text = "";
				for (int i = 0; i < 4; i++)
				{
					text += ModManager.PasswordChar.RandomItem<char>().ToString();
				}
				iniData.Global["pass"] = text;
				fileIniDataParser.WriteFile(ModManager.PathIni, iniData, null);
			}
			result = iniData;
		}
		catch (Exception message)
		{
			Debug.Log(message);
			Debug.Log("exception: Failed to parse:" + ModManager.PathIni);
			result = null;
		}
		return result;
	}

	public override void Init(string path, string defaultPackage = "_Elona")
	{
		base.Init(path, defaultPackage);
		IniData elinIni = this.GetElinIni();
		Debug.Log("IsOffline:" + BaseCore.IsOffline.ToString());
		if (elinIni != null)
		{
			if (BaseCore.IsOffline)
			{
				string key = elinIni.GetKey("path_workshop");
				if (!key.IsEmpty())
				{
					this.dirWorkshop = new DirectoryInfo(key);
				}
			}
			else
			{
				DirectoryInfo parent = new DirectoryInfo(App.Client.GetAppInstallDirectory(SteamSettings.behaviour.settings.applicationId)).Parent.Parent;
				this.dirWorkshop = new DirectoryInfo(parent.FullName + "/workshop/content/2135150");
				elinIni.Global["path_workshop"] = (this.dirWorkshop.FullName ?? "");
				new FileIniDataParser().WriteFile(ModManager.PathIni, elinIni, null);
			}
		}
		if (this.dirWorkshop != null && !this.dirWorkshop.Exists)
		{
			this.dirWorkshop = null;
		}
		string str = "dirWorkshop:";
		DirectoryInfo dirWorkshop = this.dirWorkshop;
		Debug.Log(str + ((dirWorkshop != null) ? dirWorkshop.ToString() : null));
		Debug.Log("Mod Init:" + BaseModManager.rootDefaultPacakge);
		this.packages.Clear();
		DirectoryInfo[] directories = new DirectoryInfo(BaseModManager.rootMod).GetDirectories();
		Array.Reverse<DirectoryInfo>(directories);
		foreach (DirectoryInfo directoryInfo in directories)
		{
			if (!EClass.debug.skipMod || !Application.isEditor || !(directoryInfo.Name != "_Elona"))
			{
				this.AddPackage(directoryInfo, true);
			}
		}
	}

	private void HandleResults(UgcQuery query)
	{
		foreach (WorkshopItem workshopItem in query.ResultsList)
		{
			if (workshopItem.IsSubscribed)
			{
				this.AddWorkshopPackage(workshopItem, false);
			}
		}
	}

	public IEnumerator RefreshMods(Action onComplete, bool syncMods)
	{
		bool sync = !BaseCore.IsOffline && syncMods && UserGeneratedContent.Client.GetNumSubscribedItems() > 0U;
		LoadingScreen loading = sync ? Util.Instantiate<LoadingScreen>("LoadingScreen", null) : null;
		if (!ModManager.disableMod && (!EClass.debug.skipMod || !Application.isEditor))
		{
			if (sync)
			{
				UgcQuery activeQuery = UgcQuery.GetSubscribed(false, false, false, false, 0U);
				activeQuery.Execute(new UnityAction<UgcQuery>(this.HandleResults));
				LoadingScreen loadingScreen = loading;
				if (loadingScreen != null)
				{
					loadingScreen.Log("Fetching subscripted Mods...(Hit ESC to cancel)");
				}
				while (activeQuery.handle != UGCQueryHandle_t.Invalid && !UnityEngine.Input.GetKey(KeyCode.Escape))
				{
					yield return new WaitForEndOfFrame();
				}
				activeQuery = null;
			}
			else
			{
				LoadingScreen loadingScreen2 = loading;
				if (loadingScreen2 != null)
				{
					loadingScreen2.Log("Fetching offline Mods.");
				}
				if (this.dirWorkshop != null)
				{
					foreach (DirectoryInfo dir in this.dirWorkshop.GetDirectories())
					{
						this.AddPackage(dir, false);
					}
				}
			}
		}
		if (sync)
		{
			bool valid = false;
			while (!valid)
			{
				valid = true;
				foreach (BaseModPackage baseModPackage in this.packages)
				{
					WorkshopItem workshopItem = baseModPackage.item as WorkshopItem;
					if (!baseModPackage.installed && workshopItem != null && !workshopItem.IsBanned)
					{
						valid = false;
						string text = "Downloading " + workshopItem.Title + ": ";
						if (!baseModPackage.progressText)
						{
							baseModPackage.progressText = loading.Log(text);
						}
						if (baseModPackage.downloadStarted && workshopItem.DownloadCompletion >= 1f)
						{
							baseModPackage.progressText.text = text + "Done!";
							baseModPackage.installed = true;
						}
						else if (workshopItem.IsDownloading || workshopItem.IsDownloadPending)
						{
							baseModPackage.progressText.text = text + ((int)(workshopItem.DownloadCompletion * 100f)).ToString() + "%";
						}
						else if (!baseModPackage.downloadStarted)
						{
							baseModPackage.downloadStarted = true;
							workshopItem.DownloadItem(true);
							Debug.Log(string.Concat(new string[]
							{
								"start downloading:",
								workshopItem.Title,
								"/",
								workshopItem.IsInstalled.ToString(),
								"/",
								baseModPackage.installed.ToString(),
								"/",
								workshopItem.IsDownloading.ToString(),
								"/",
								workshopItem.IsDownloadPending.ToString(),
								"/",
								workshopItem.DownloadCompletion.ToString()
							}));
						}
					}
				}
				if (!valid && UnityEngine.Input.GetKey(KeyCode.Escape))
				{
					break;
				}
				yield return new WaitForEndOfFrame();
			}
		}
		foreach (BaseModPackage baseModPackage2 in this.packages)
		{
			baseModPackage2.Init();
			Debug.Log(string.Concat(new string[]
			{
				baseModPackage2.title,
				"/",
				baseModPackage2.id,
				"/",
				baseModPackage2.installed.ToString(),
				"/",
				baseModPackage2.dirInfo.FullName
			}));
		}
		this.LoadLoadOrder();
		this.packages.Sort((BaseModPackage a, BaseModPackage b) => a.loadPriority - b.loadPriority);
		foreach (BaseModPackage baseModPackage3 in this.packages)
		{
			if (!baseModPackage3.isInPackages && baseModPackage3.willActivate)
			{
				foreach (BaseModPackage baseModPackage4 in this.packages)
				{
					if (baseModPackage4.isInPackages && baseModPackage3.id == baseModPackage4.id)
					{
						baseModPackage4.hasPublishedPackage = true;
					}
				}
			}
		}
		LoadingScreen loadingScreen3 = loading;
		if (loadingScreen3 != null)
		{
			loadingScreen3.Log("Total number of Mods:" + this.packages.Count.ToString());
		}
		if (loading)
		{
			loading.Log("Activating Mods...");
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
		}
		BaseModManager.listChainLoad.Clear();
		ModManager.ListPluginObject.Clear();
		foreach (BaseModPackage baseModPackage5 in this.packages)
		{
			if (baseModPackage5.IsValidVersion())
			{
				baseModPackage5.Activate();
				if (baseModPackage5.activated)
				{
					BaseModManager.listChainLoad.Add(baseModPackage5.dirInfo.FullName);
				}
			}
		}
		BaseModManager.isInitialized = true;
		yield return new WaitForEndOfFrame();
		if (onComplete != null)
		{
			onComplete();
		}
		if (loading)
		{
			UnityEngine.Object.Destroy(loading.gameObject);
		}
		yield return null;
		yield break;
	}

	public void SaveLoadOrder()
	{
		List<string> list = new List<string>();
		foreach (BaseModPackage baseModPackage in this.packages)
		{
			if (!baseModPackage.builtin && Directory.Exists(baseModPackage.dirInfo.FullName))
			{
				string item = baseModPackage.dirInfo.FullName + "," + (baseModPackage.willActivate ? 1 : 0).ToString();
				list.Add(item);
			}
		}
		File.WriteAllLines(CorePath.rootExe + "loadorder.txt", list);
	}

	public void LoadLoadOrder()
	{
		string path = CorePath.rootExe + "loadorder.txt";
		if (!File.Exists(path))
		{
			return;
		}
		Dictionary<string, BaseModPackage> dictionary = new Dictionary<string, BaseModPackage>();
		foreach (BaseModPackage baseModPackage in this.packages)
		{
			if (!baseModPackage.builtin)
			{
				dictionary[baseModPackage.dirInfo.FullName] = baseModPackage;
			}
		}
		int num = 0;
		string[] array = File.ReadAllLines(path);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(',', StringSplitOptions.None);
			if (dictionary.ContainsKey(array2[0]))
			{
				BaseModPackage baseModPackage2 = dictionary[array2[0]];
				baseModPackage2.loadPriority = num;
				baseModPackage2.willActivate = (array2[1] == "1");
			}
			num++;
		}
	}

	public ModPackage AddPackage(DirectoryInfo dir, bool isInPackages = false)
	{
		ModPackage modPackage = new ModPackage
		{
			dirInfo = dir,
			installed = true
		};
		this.packages.Add(modPackage);
		modPackage.isInPackages = isInPackages;
		modPackage.loadPriority = this.priorityIndex;
		this.priorityIndex++;
		return modPackage;
	}

	public ModPackage AddWorkshopPackage(WorkshopItem item, bool isInPackages = false)
	{
		ulong num;
		string path;
		DateTime dateTime;
		UserGeneratedContent.Client.GetItemInstallInfo(item.FileId, out num, out path, out dateTime);
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		ModPackage modPackage = new ModPackage
		{
			item = item,
			dirInfo = directoryInfo,
			installed = directoryInfo.Exists,
			banned = item.IsBanned
		};
		this.packages.Add(modPackage);
		modPackage.isInPackages = isInPackages;
		modPackage.loadPriority = this.priorityIndex;
		this.priorityIndex++;
		return modPackage;
	}

	public override void ParseExtra(DirectoryInfo dir, BaseModPackage package)
	{
		string name = dir.Name;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
		if (num <= 1151856721U)
		{
			if (num != 688467962U)
			{
				if (num != 1054589944U)
				{
					if (num != 1151856721U)
					{
						return;
					}
					if (!(name == "Map"))
					{
						return;
					}
					if (!package.builtin)
					{
						foreach (FileInfo fileInfo in dir.GetFiles())
						{
							if (fileInfo.Name.EndsWith(".z"))
							{
								MOD.listMaps.Add(fileInfo);
							}
						}
						return;
					}
				}
				else
				{
					if (!(name == "TalkText"))
					{
						return;
					}
					foreach (FileInfo fileInfo2 in dir.GetFiles())
					{
						if (fileInfo2.Name.EndsWith(".xlsx"))
						{
							TalkText.modList.Add(new ExcelData(fileInfo2.FullName));
						}
					}
					return;
				}
			}
			else
			{
				if (!(name == "Portrait"))
				{
					return;
				}
				foreach (FileInfo fileInfo3 in dir.GetFiles())
				{
					if (fileInfo3.Name.EndsWith(".png"))
					{
						if (fileInfo3.Name.StartsWith("BG_"))
						{
							Portrait.modPortraitBGs.Add(fileInfo3, null, "");
						}
						else if (fileInfo3.Name.StartsWith("BGF_"))
						{
							Portrait.modPortraitBGFs.Add(fileInfo3, null, "");
						}
						else if (fileInfo3.Name.EndsWith("-full.png"))
						{
							Portrait.modFull.Add(fileInfo3, null, "");
						}
						else if (fileInfo3.Name.EndsWith("-overlay.png"))
						{
							Portrait.modOverlays.Add(fileInfo3, null, "");
						}
						else
						{
							Portrait.modPortraits.Add(fileInfo3, null, "");
						}
						Portrait.allIds.Add(fileInfo3.Name);
					}
				}
				return;
			}
		}
		else if (num <= 2571916692U)
		{
			if (num != 2026591700U)
			{
				if (num != 2571916692U)
				{
					return;
				}
				if (!(name == "Texture"))
				{
					return;
				}
				foreach (FileInfo fileInfo4 in dir.GetFiles())
				{
					if (fileInfo4.Name.EndsWith(".png"))
					{
						SpriteReplacer.dictModItems[fileInfo4.Name.Replace(".png", "")] = fileInfo4.GetFullFileNameWithoutExtension();
					}
				}
				return;
			}
			else
			{
				if (!(name == "Texture Replace"))
				{
					return;
				}
				foreach (FileInfo fileInfo5 in dir.GetFiles())
				{
					if (fileInfo5.Name.EndsWith(".png"))
					{
						this.replaceFiles.Add(fileInfo5);
					}
				}
				return;
			}
		}
		else if (num != 3658367683U)
		{
			if (num != 4044785525U)
			{
				return;
			}
			if (!(name == "Lang"))
			{
				return;
			}
			foreach (DirectoryInfo directoryInfo in dir.GetDirectories())
			{
				if (!directoryInfo.Name.StartsWith("_") && !this.<ParseExtra>g__TryAddLang|20_0(directoryInfo, false))
				{
					Debug.Log("Generating Language Mod Contents:" + directoryInfo.FullName);
					IO.CopyDir(CorePath.packageCore + "Lang/EN", directoryInfo.FullName, null);
					Directory.CreateDirectory(directoryInfo.FullName + "/Dialog");
					IO.CopyDir(CorePath.packageCore + "Lang/_Dialog", directoryInfo.FullName + "/Dialog", null);
					EClass.sources.ExportSourceTexts(directoryInfo.FullName + "/Game");
					IO.Copy(CorePath.packageCore + "Lang/lang.ini", directoryInfo.FullName + "/");
					this.<ParseExtra>g__TryAddLang|20_0(directoryInfo, true);
				}
			}
		}
		else
		{
			if (!(name == "Map Piece"))
			{
				return;
			}
			if (!package.builtin)
			{
				foreach (FileInfo fileInfo6 in dir.GetFiles())
				{
					if (fileInfo6.Name.EndsWith(".mp"))
					{
						MOD.listPartialMaps.Add(fileInfo6);
					}
				}
				return;
			}
		}
	}

	public void UpdateDialogs(DirectoryInfo dir, string dirTemp)
	{
		foreach (DirectoryInfo directoryInfo in dir.GetDirectories())
		{
			this.UpdateDialogs(directoryInfo, dirTemp + "/" + directoryInfo.Name);
		}
		foreach (FileInfo fileInfo in dir.GetFiles())
		{
			if (fileInfo.Name.EndsWith("xlsx"))
			{
				this.UpdateExcelBook(fileInfo, dirTemp, true);
			}
		}
	}

	public void UpdateTalks(DirectoryInfo dir, string dirTemp)
	{
		foreach (FileInfo fileInfo in dir.GetFiles())
		{
			if (fileInfo.Name == "god_talk.xlsx" || fileInfo.Name == "chara_talk.xlsx")
			{
				this.UpdateExcelBook(fileInfo, dirTemp, false);
			}
		}
	}

	public void UpdateExcelBook(FileInfo f, string dirTemp, bool updateOnlyText)
	{
		string path = dirTemp + "/" + f.Name;
		if (!File.Exists(path))
		{
			return;
		}
		XSSFWorkbook xssfworkbook;
		using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			xssfworkbook = new XSSFWorkbook(fileStream);
		}
		XSSFWorkbook xssfworkbook2;
		using (FileStream fileStream2 = File.Open(f.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			xssfworkbook2 = new XSSFWorkbook(fileStream2);
		}
		for (int i = 0; i < xssfworkbook2.NumberOfSheets; i++)
		{
			ISheet sheetAt = xssfworkbook2.GetSheetAt(i);
			ISheet sheet = xssfworkbook.GetSheet(sheetAt.SheetName);
			if (sheet == null)
			{
				Log.system = Log.system + "Old sheet not found:" + sheetAt.SheetName + Environment.NewLine;
			}
			else
			{
				int num = this.UpdateExcelSheet(sheetAt, sheet, updateOnlyText);
				Log.system = string.Concat(new string[]
				{
					Log.system,
					(num == 0) ? "(No Changes) " : "(Updated) ",
					f.FullName,
					"(",
					sheetAt.SheetName,
					")",
					Environment.NewLine
				});
				if (num != 0)
				{
					Log.system = Log.system + num.ToString() + Environment.NewLine;
				}
				Log.system += Environment.NewLine;
			}
		}
		using (FileStream fileStream3 = new FileStream(f.FullName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
		{
			xssfworkbook2.Write(fileStream3);
		}
	}

	public int UpdateExcelSheet(ISheet destSheet, ISheet oldSheet, bool updateOnlytext)
	{
		Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
		int num = 0;
		int num2 = 0;
		int num3 = 10;
		IRow row = destSheet.GetRow(0);
		IRow row2 = oldSheet.GetRow(0);
		List<ModManager.SheetIndex> list = new List<ModManager.SheetIndex>();
		int cellnum = ModManager.<UpdateExcelSheet>g__FindField|25_0(row, "id");
		int cellnum2 = ModManager.<UpdateExcelSheet>g__FindField|25_0(row2, "id");
		for (int i = 0; i < (int)row.LastCellNum; i++)
		{
			ICell cell = row.GetCell(i);
			if (cell == null)
			{
				break;
			}
			string stringCellValue = cell.StringCellValue;
			if (!(stringCellValue == "id") && (!updateOnlytext || !(stringCellValue != "text")))
			{
				for (int j = 0; j < (int)row2.LastCellNum; j++)
				{
					cell = row2.GetCell(j);
					if (cell == null)
					{
						break;
					}
					if (cell.StringCellValue == stringCellValue)
					{
						list.Add(new ModManager.SheetIndex
						{
							dest = i,
							old = j
						});
						Debug.Log(string.Concat(new string[]
						{
							destSheet.SheetName,
							"/",
							stringCellValue,
							"/",
							i.ToString(),
							"/",
							j.ToString()
						}));
						break;
					}
				}
			}
		}
		for (int k = 2; k <= oldSheet.LastRowNum; k++)
		{
			IRow row3 = oldSheet.GetRow(k);
			if (row3 == null)
			{
				if (num2 >= num3)
				{
					break;
				}
				num2++;
			}
			else
			{
				num2 = 0;
				ICell cell2 = row3.GetCell(cellnum2);
				if (cell2 != null)
				{
					string text = (cell2.CellType == CellType.Numeric) ? cell2.NumericCellValue.ToString() : cell2.StringCellValue;
					if (!text.IsEmpty())
					{
						string[] array = new string[list.Count];
						for (int l = 0; l < list.Count; l++)
						{
							ICell cell3 = row3.GetCell(list[l].old);
							if (cell3 != null)
							{
								string stringCellValue2 = cell3.StringCellValue;
								if (!stringCellValue2.IsEmpty())
								{
									array[l] = stringCellValue2;
								}
							}
						}
						dictionary.Add(text, array);
					}
				}
			}
		}
		num2 = 0;
		for (int m = 2; m <= destSheet.LastRowNum; m++)
		{
			IRow row4 = destSheet.GetRow(m);
			if (row4 == null)
			{
				if (num2 >= num3)
				{
					break;
				}
				num2++;
			}
			else
			{
				num2 = 0;
				ICell cell4 = row4.GetCell(cellnum);
				if (cell4 != null)
				{
					string text2 = (cell4.CellType == CellType.Numeric) ? cell4.NumericCellValue.ToString() : cell4.StringCellValue;
					if (!text2.IsEmpty() && dictionary.ContainsKey(text2))
					{
						string[] array2 = dictionary[text2];
						for (int n = 0; n < list.Count; n++)
						{
							ICell cell5 = row4.GetCell(list[n].dest) ?? row4.CreateCell(list[n].dest, CellType.String);
							if (cell5 != null)
							{
								cell5.SetCellValue(array2[n]);
								cell5.SetCellType(CellType.String);
								cell5.SetAsActiveCell();
								num++;
							}
						}
					}
				}
			}
		}
		return num;
	}

	[CompilerGenerated]
	private bool <ParseExtra>g__TryAddLang|20_0(DirectoryInfo dirLang, bool isNew)
	{
		string name = dirLang.Name;
		foreach (FileInfo fileInfo in dirLang.GetFiles())
		{
			if (fileInfo.Name == "lang.ini")
			{
				LangSetting langSetting = new LangSetting(fileInfo.FullName)
				{
					id = name,
					dir = dirLang.FullName + "/"
				};
				if (isNew)
				{
					langSetting.SetVersion();
				}
				else if ((Application.isEditor || Lang.runUpdate) && !Lang.IsBuiltin(dirLang.Name) && langSetting.GetVersion() != EClass.core.version.GetInt())
				{
					EClass.sources.Init();
					Log.system = "Updated Language Files:" + Environment.NewLine + Environment.NewLine;
					Debug.Log(string.Concat(new string[]
					{
						"Updating Language:",
						langSetting.name,
						"/",
						langSetting.GetVersion().ToString(),
						"/",
						EClass.core.version.GetInt().ToString()
					}));
					string text = dirLang.FullName + "/Game";
					Directory.Move(text, text + "_temp");
					EClass.sources.ExportSourceTexts(text);
					EClass.sources.UpdateSourceTexts(text);
					IO.DeleteDirectory(text + "_temp");
					text = dirLang.FullName + "/Dialog";
					Directory.Move(text, text + "_temp");
					IO.CopyDir(CorePath.packageCore + "Lang/_Dialog", text, null);
					this.UpdateDialogs(new DirectoryInfo(text), text + "_temp");
					IO.DeleteDirectory(text + "_temp");
					text = dirLang.FullName + "/Data";
					IO.CopyDir(text, text + "_temp", null);
					IO.Copy(CorePath.packageCore + "Lang/EN/Data/god_talk.xlsx", text);
					IO.Copy(CorePath.packageCore + "Lang/EN/Data/chara_talk.xlsx", text);
					this.UpdateTalks(new DirectoryInfo(text), text + "_temp");
					IO.DeleteDirectory(text + "_temp");
					langSetting.SetVersion();
					IO.SaveText(dirLang.FullName + "/update.txt", Log.system);
				}
				MOD.langs[name] = langSetting;
				return true;
			}
		}
		return false;
	}

	[CompilerGenerated]
	internal static int <UpdateExcelSheet>g__FindField|25_0(IRow row, string id)
	{
		for (int i = 0; i < (int)row.LastCellNum; i++)
		{
			ICell cell = row.GetCell(i);
			if (cell == null)
			{
				break;
			}
			if (cell.StringCellValue == id)
			{
				return i;
			}
		}
		return -1;
	}

	public static readonly string PasswordChar = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

	public static List<object> ListPluginObject = new List<object>();

	public static bool disableMod = false;

	public List<FileInfo> replaceFiles = new List<FileInfo>();

	public struct SheetIndex
	{
		public int dest;

		public int old;
	}
}
