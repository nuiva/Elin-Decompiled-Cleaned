using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using AeLa.EasyFeedback;
using AeLa.EasyFeedback.FormElements;
using AeLa.EasyFeedback.FormFields;
using Ionic.Zip;
using UnityEngine;
using UnityEngine.UI;

public class LayerFeedback : ELayer
{
	public override void OnInit()
	{
		this.inputVersion.text = string.Concat(new string[]
		{
			ELayer.core.version.GetText(),
			"/",
			ELayer.config.lang,
			" ",
			LayerFeedback.steamName,
			"/",
			LayerFeedback.userName,
			" hours:",
			LayerFeedback.playedHours.ToString(),
			" backer:",
			LayerFeedback.backerId
		});
		TextField.onAddEmail = delegate()
		{
			LayerFeedback.<>c__DisplayClass25_0 CS$<>8__locals1;
			CS$<>8__locals1.s = "";
			foreach (BaseModPackage baseModPackage in ELayer.core.mods.packages)
			{
				if (!baseModPackage.builtin)
				{
					LayerFeedback.<OnInit>g__Append|25_8(baseModPackage.id + "/" + baseModPackage.activated.ToString(), ref CS$<>8__locals1);
				}
			}
			LayerFeedback.<OnInit>g__Append|25_8("", ref CS$<>8__locals1);
			LayerFeedback.<OnInit>g__Append|25_8("Save Path: " + CorePath.RootSave, ref CS$<>8__locals1);
			LayerFeedback.<OnInit>g__Append|25_8("EXE Path: " + CorePath.rootExe, ref CS$<>8__locals1);
			LayerFeedback.<OnInit>g__Append|25_8("OS: " + SystemInfo.operatingSystem, ref CS$<>8__locals1);
			LayerFeedback.<OnInit>g__Append|25_8("Processor: " + SystemInfo.processorType, ref CS$<>8__locals1);
			LayerFeedback.<OnInit>g__Append|25_8("Memory: " + SystemInfo.systemMemorySize.ToString(), ref CS$<>8__locals1);
			LayerFeedback.<OnInit>g__Append|25_8("Graphics API: " + SystemInfo.graphicsDeviceType.ToString(), ref CS$<>8__locals1);
			LayerFeedback.<OnInit>g__Append|25_8("Graphics Processor: " + SystemInfo.graphicsDeviceName, ref CS$<>8__locals1);
			LayerFeedback.<OnInit>g__Append|25_8("Graphics Memory: " + SystemInfo.graphicsMemorySize.ToString(), ref CS$<>8__locals1);
			LayerFeedback.<OnInit>g__Append|25_8("Graphics Vendor: " + SystemInfo.graphicsDeviceVendor, ref CS$<>8__locals1);
			LayerFeedback.<OnInit>g__Append|25_8("Quality Level: " + QualitySettings.names[QualitySettings.GetQualityLevel()], ref CS$<>8__locals1);
			LayerFeedback.<OnInit>g__Append|25_8("Resolution: " + Screen.width.ToString() + "x" + Screen.height.ToString(), ref CS$<>8__locals1);
			LayerFeedback.<OnInit>g__Append|25_8("Full Screen: " + Screen.fullScreen.ToString(), ref CS$<>8__locals1);
			LayerFeedback.<OnInit>g__Append|25_8("regionName: " + RegionInfo.CurrentRegion.Name, ref CS$<>8__locals1);
			LayerFeedback.<OnInit>g__Append|25_8("cultureName: " + CultureInfo.CurrentCulture.Name, ref CS$<>8__locals1);
			LayerFeedback.<OnInit>g__Append|25_8("cname: " + Thread.CurrentThread.CurrentCulture.Name, ref CS$<>8__locals1);
			LayerFeedback.<OnInit>g__Append|25_8("uiname: " + Thread.CurrentThread.CurrentUICulture.Name, ref CS$<>8__locals1);
			LayerFeedback.<OnInit>g__Append|25_8("device identifier: " + SystemInfo.deviceUniqueIdentifier, ref CS$<>8__locals1);
			return Environment.NewLine + Environment.NewLine + CS$<>8__locals1.s;
		};
		this.buttonLinkFAQ.SetOnClick(delegate
		{
			LayerHelp.Toggle("general", "FAQ");
		});
		this.buttonLinkDiscord.SetOnClick(delegate
		{
			SE.Click();
			Application.OpenURL("https://discord.gg/elona");
		});
		this.transUpload.SetActive(false);
		this.buttonSubmit.interactable = false;
		if (!ELayer.config.nameReport.IsEmpty())
		{
			this.inputName.text = ELayer.config.nameReport;
		}
		else if (ELayer.core.IsGameStarted)
		{
			this.inputName.text = "nameBraced".lang(ELayer.pc.NameSimple, ELayer.pc.Aka, null, null, null);
		}
		if (!ELayer.config.emailReport.IsEmpty())
		{
			this.inputEmail.text = ELayer.config.emailReport;
		}
		this.inputLang.text = ELayer.config.lang;
		this.inputDetail.text = LayerFeedback.lastDetail;
		this.inputSummary.text = LayerFeedback.lastSummary;
		this.collector.onSubmit = delegate()
		{
			this.CollectFiles();
		};
		this.category.Init((string s) => ("form_" + s).lang());
		List<GameIndex> gameList = GameIO.GetGameList(GameIO.pathSaveRoot, false);
		if (gameList.Count > 0)
		{
			int index = 0;
			this.saveIndex = gameList[0];
			for (int i = 0; i < gameList.Count; i++)
			{
				if (ELayer.core.IsGameStarted && gameList[i].id == Game.id)
				{
					index = i;
					this.saveIndex = gameList[i];
				}
			}
			this.ddSave.SetList<GameIndex>(index, gameList, (GameIndex a, int b) => ((ELayer.core.IsGameStarted && a.id == Game.id) ? "currentSave".lang() : "") + a.FormTitle, delegate(int a, GameIndex b)
			{
				this.saveIndex = b;
			}, true);
		}
		else
		{
			this.toggleSave.SetActive(false);
		}
		this.toggleSave.onValueChanged.AddListener(delegate(bool a)
		{
			this.ddSave.SetActive(a);
		});
		this.toggleSave.isOn = false;
		this.ddSave.SetActive(this.toggleSave.isOn);
		this.form.Init();
		this.inputDetail.onValueChanged.AddListener(delegate(string a)
		{
			this.Validate();
		});
		this.inputSummary.onValueChanged.AddListener(delegate(string a)
		{
			this.Validate();
		});
		this.Validate();
	}

	public void Validate()
	{
		bool interactable = true;
		if (this.inputSummary.text.Length < 4)
		{
			interactable = false;
		}
		if (this.inputDetail.text.Length < 4)
		{
			interactable = false;
		}
		this.buttonSubmit.interactable = interactable;
	}

	public void CollectFiles()
	{
		string str = Application.persistentDataPath + "/";
		string text = str + "_temp";
		string text2 = text + "/log.zip";
		IO.CreateDirectory(text);
		IO.Copy(str + "Player.log", text);
		IO.Copy(str + "Player-prev.log", text);
		IO.SaveText(text + "/System.txt", this.GetSystemText());
		LayerFeedback.<>c__DisplayClass27_0 CS$<>8__locals1;
		CS$<>8__locals1.errors = 0;
		LayerFeedback.<CollectFiles>g__ParseLog|27_0(text + "/Player.log", ref CS$<>8__locals1);
		LayerFeedback.<CollectFiles>g__ParseLog|27_0(text + "/Player-prev.log", ref CS$<>8__locals1);
		ReportTitle.strAdd = ((CS$<>8__locals1.errors == 0) ? "" : ("(" + CS$<>8__locals1.errors.ToString() + ")"));
		if (ELayer.debug.enable)
		{
			ReportTitle.strAdd += "(debug)";
		}
		string text3 = this.inputDetail.text;
		int num = 0;
		foreach (string c in new string[]
		{
			"ナーフ",
			"つまらない",
			"面白くない",
			"不合理",
			"やめて",
			"無駄",
			"弱体化",
			"不親切",
			"クソ",
			"糞",
			"バカ",
			"馬鹿",
			"nerf",
			"boring",
			"please don't",
			"waste",
			"fuck",
			"suck",
			"tbh",
			"shit",
			"stupid"
		})
		{
			num += LayerFeedback.<CollectFiles>g__CountString|27_1(text3, c);
		}
		if (num > 0)
		{
			ReportTitle.strAdd = "[" + num.ToString() + "]";
		}
		using (ZipFile zipFile = new ZipFile())
		{
			zipFile.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
			zipFile.AddDirectory(text);
			zipFile.Save(text2);
		}
		this.form.CurrentReport.AttachFile("log.zip", File.ReadAllBytes(text2));
		if (this.toggleSave.isOn)
		{
			string text4 = text + "/save.zip";
			using (ZipFile zipFile2 = new ZipFile())
			{
				zipFile2.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
				zipFile2.AddDirectory(GameIO.pathSaveRoot + this.saveIndex.id);
				zipFile2.Save(text4);
			}
			this.form.CurrentReport.AttachFile(this.saveIndex.id + ".zip", File.ReadAllBytes(text4));
		}
		IO.DeleteDirectory(text);
	}

	public void OnCancel()
	{
		LayerFeedback.lastDetail = this.inputDetail.text;
		LayerFeedback.lastSummary = this.inputSummary.text;
		this.form.Clean();
		this.Close();
	}

	public void Submit()
	{
		int num = 0;
		foreach (BaseModPackage baseModPackage in ELayer.core.mods.packages)
		{
			if (!baseModPackage.builtin && baseModPackage.activated)
			{
				num++;
			}
		}
		if (num > 0)
		{
			Dialog.YesNo("warn_mod".lang(num.ToString() ?? "", null, null, null, null), delegate
			{
			}, delegate
			{
				this.form.Submit();
			}, "warn_mod_yes", "warn_mod_no");
			return;
		}
		this.form.Submit();
	}

	public void OnSubmit()
	{
		if (!this.inputEmail.text.IsEmpty())
		{
			ELayer.config.emailReport = this.inputEmail.text;
		}
		if (!this.inputName.text.IsEmpty())
		{
			ELayer.config.nameReport = this.inputName.text;
		}
		this.transUpload.SetActive(true);
		this.form.IncludeScreenshot = this.toggleScreenshot.isOn;
		this.cgForm.alpha = 0.5f;
		this.form.transform.SetParent(ELayer.ui.transform, false);
		this.Close();
		LayerFeedback.lastDetail = (LayerFeedback.lastSummary = "");
	}

	public string GetSystemText()
	{
		LayerFeedback.<>c__DisplayClass31_0 CS$<>8__locals1;
		CS$<>8__locals1.txt = "";
		LayerFeedback.<GetSystemText>g__Append|31_0("OS: " + SystemInfo.operatingSystem, ref CS$<>8__locals1);
		LayerFeedback.<GetSystemText>g__Append|31_0("Processor: " + SystemInfo.processorType, ref CS$<>8__locals1);
		LayerFeedback.<GetSystemText>g__Append|31_0("Memory: " + SystemInfo.systemMemorySize.ToString(), ref CS$<>8__locals1);
		LayerFeedback.<GetSystemText>g__Append|31_0("Graphics API: " + SystemInfo.graphicsDeviceType.ToString(), ref CS$<>8__locals1);
		LayerFeedback.<GetSystemText>g__Append|31_0("Graphics Processor: " + SystemInfo.graphicsDeviceName, ref CS$<>8__locals1);
		LayerFeedback.<GetSystemText>g__Append|31_0("Graphics Memory: " + SystemInfo.graphicsMemorySize.ToString(), ref CS$<>8__locals1);
		LayerFeedback.<GetSystemText>g__Append|31_0("Graphics Vendor: " + SystemInfo.graphicsDeviceVendor, ref CS$<>8__locals1);
		LayerFeedback.<GetSystemText>g__Append|31_0("Quality Level: " + QualitySettings.names[QualitySettings.GetQualityLevel()], ref CS$<>8__locals1);
		LayerFeedback.<GetSystemText>g__Append|31_0("Resolution: " + Screen.width.ToString() + "x" + Screen.height.ToString(), ref CS$<>8__locals1);
		LayerFeedback.<GetSystemText>g__Append|31_0("Full Screen: " + Screen.fullScreen.ToString(), ref CS$<>8__locals1);
		return CS$<>8__locals1.txt;
	}

	[CompilerGenerated]
	internal static void <OnInit>g__Append|25_8(string text, ref LayerFeedback.<>c__DisplayClass25_0 A_1)
	{
		if (text != null)
		{
			A_1.s = A_1.s + text + Environment.NewLine;
		}
	}

	[CompilerGenerated]
	internal static void <CollectFiles>g__ParseLog|27_0(string path, ref LayerFeedback.<>c__DisplayClass27_0 A_1)
	{
		if (!File.Exists(path))
		{
			return;
		}
		foreach (string text in IO.LoadTextArray(path))
		{
			if (!(text == "InvalidOperationException: Steamworks is not initialized."))
			{
				if (text == "InvalidOperationException: Collection was modified; enumeration operation may not execute." || text == "NullReferenceException: Object reference not set to an instance of an object")
				{
					int errors = A_1.errors;
					A_1.errors = errors + 1;
				}
				else if (text.Contains("exception"))
				{
					int errors = A_1.errors;
					A_1.errors = errors + 1;
				}
				else if (text.Contains("Exception"))
				{
					int errors = A_1.errors;
					A_1.errors = errors + 1;
				}
			}
		}
	}

	[CompilerGenerated]
	internal static int <CollectFiles>g__CountString|27_1(string s, string c)
	{
		string newValue = c.Substring(0, c.Length - 1);
		return s.Length - s.Replace(c, newValue).Length;
	}

	[CompilerGenerated]
	internal static void <GetSystemText>g__Append|31_0(string s, ref LayerFeedback.<>c__DisplayClass31_0 A_1)
	{
		A_1.txt = A_1.txt + s + Environment.NewLine;
	}

	public static string lastSummary = "";

	public static string lastDetail = "";

	public static string steamName = "";

	public static string userName = "";

	public static string backerId = "";

	public static int playedHours;

	public UIText textProgress;

	public InputField inputVersion;

	public InputField inputName;

	public InputField inputEmail;

	public InputField inputLang;

	public InputField inputSummary;

	public InputField inputDetail;

	public FeedbackForm form;

	public DebugLogCollector collector;

	public Transform transUpload;

	public UIDropdown ddSave;

	public Toggle toggleSave;

	public Toggle toggleScreenshot;

	public GameIndex saveIndex;

	public CanvasGroup cgForm;

	public CategoryDropdown category;

	public Button buttonSubmit;

	public UIButton buttonLinkFAQ;

	public UIButton buttonLinkDiscord;
}
