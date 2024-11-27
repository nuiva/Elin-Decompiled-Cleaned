using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using UnityEngine;

public class LayerLoadGame : ELayer
{
	public void Init(bool _backup, string pathBackup = "", string _idDest = "")
	{
		this.backup = _backup;
		this.idDest = _idDest;
		this.pathRoot = (this.backup ? pathBackup : GameIO.pathSaveRoot);
		if (this.backup)
		{
			for (int i = 0; i < 2; i++)
			{
				Vector2 anchoredPosition = this.windows[i].Rect().anchoredPosition;
				this.windows[i].Rect().anchoredPosition = new Vector2(anchoredPosition.x + 40f, anchoredPosition.y - 30f);
			}
		}
		if (!this.backup && Application.isEditor)
		{
			foreach (FileInfo fileInfo in new DirectoryInfo("D:\\Download").GetFiles())
			{
				if (fileInfo.Name.StartsWith("world") && fileInfo.Name.EndsWith(".zip"))
				{
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
					string path = CorePath.RootSave + fileNameWithoutExtension;
					if (!Directory.Exists(path))
					{
						Directory.CreateDirectory(path);
						new StreamWriter(new MemoryStream());
						ReadOptions options = new ReadOptions();
						ZipFile.Read(fileInfo.FullName, options).ExtractAll(path);
					}
				}
			}
		}
		this.RefreshList();
	}

	public void RefreshList()
	{
		if (this.worlds == null)
		{
			this.worlds = GameIO.GetGameList(this.pathRoot, this.backup);
		}
		this.goInfo.SetActive(false);
		this.goNoInfo.SetActive(true);
		this.list.Clear();
		BaseList baseList = this.list;
		UIList.Callback<GameIndex, UIButton> callback = new UIList.Callback<GameIndex, UIButton>();
		callback.onInstantiate = delegate(GameIndex a, UIButton b)
		{
			FontColor c = ELayer.core.version.IsSaveCompatible(a.version) ? FontColor.Good : FontColor.Bad;
			string s = a.Title + ((ELayer.core.IsGameStarted && a.id == Game.id) ? "currentSave".lang() : "") + Environment.NewLine;
			b.mainText.SetText(s, c);
			b.subText.SetText(a.RealDate ?? "");
			b.subText2.SetText(string.Concat(new string[]
			{
				(a.difficulty == 2) ? "★" : ((a.difficulty == 1) ? "☆" : ""),
				a.pcName,
				" (",
				a.zoneName,
				")"
			}), c);
			b.GetComponent<UIItem>().text1.SetText(a.version.GetText() ?? "");
		};
		callback.onClick = delegate(GameIndex a, UIButton b)
		{
			this.RefreshInfo(a);
		};
		baseList.callbacks = callback;
		foreach (GameIndex o in this.worlds)
		{
			this.list.Add(o);
		}
		this.list.Refresh(false);
		if (this.list.items.Count == 0)
		{
			this.buttonLoad.SetActive(false);
			this.buttonDelete.SetActive(false);
			this.goNoInfo.SetActive(true);
		}
	}

	public void RefreshInfo(GameIndex i)
	{
		this.goInfo.SetActive(true);
		this.goNoInfo.SetActive(false);
		this.note.Clear();
		this.note.AddTopic("version".lang(), i.version.GetText());
		this.note.AddTopic("date_real".lang(), i.RealDate);
		this.note.AddTopic("date_game".lang(), i.GameData);
		this.note.AddTopic("ID", i.id);
		bool flag = ELayer.core.version.IsSaveCompatible(i.version);
		if (!flag)
		{
			this.note.Space(0, 1);
			this.note.AddText("incompatible".lang(), FontColor.Bad);
		}
		else
		{
			this.note.AddTopic("currentZone".lang(), i.zoneName);
		}
		this.textAka.SetText(i.aka);
		this.textName.SetText(i.pcName);
		this.textDays.SetText("infoHire".lang(i.days.ToString() ?? "", null, null, null, null));
		this.textDeepest.SetText("deepestLv2".lang(i.deepest.ToString() ?? "", null, null, null, null));
		UIText uitext = this.textRace;
		SourceRace.Row row = ELayer.sources.races.map.TryGetValue(i.idRace, null);
		uitext.SetText(((row != null) ? row.GetName().ToTitleCase(false) : null) ?? "");
		UIText uitext2 = this.textJob;
		SourceJob.Row row2 = ELayer.sources.jobs.map.TryGetValue(i.idJob, null);
		uitext2.SetText(((row2 != null) ? row2.GetName().ToTitleCase(false) : null) ?? "");
		if (!i.idPortrait.IsEmpty())
		{
			this.portrait.SetActive(true);
			Color white = Color.white;
			ColorUtility.TryParseHtmlString("#" + i.color, out white);
			this.portrait.SetPortrait(i.idPortrait, white);
		}
		else
		{
			this.portrait.SetActive(false);
		}
		this.note.Build();
		this.buttonListBackup.SetActive(!this.backup);
		this.buttonDelete.SetActive(!this.backup && !ELayer.core.IsGameStarted);
		this.buttonBackup.SetActive(!this.backup && (!ELayer.core.IsGameStarted || i.id == Game.id));
		this.buttonOpen.SetActive(this.backup);
		this.buttonLoad.onClick.RemoveAllListeners();
		this.buttonDelete.onClick.RemoveAllListeners();
		Action <>9__5;
		this.buttonLoad.SetOnClick(delegate
		{
			LayerTitle.KillActor();
			if (this.backup)
			{
				string langDetail = "dialog_restoreWarning";
				Action actionYes;
				if ((actionYes = <>9__5) == null)
				{
					actionYes = (<>9__5 = delegate()
					{
						GameIO.DeleteGame(this.idDest, false);
						IO.CopyDir(this.pathRoot + "/" + i.id, GameIO.pathSaveRoot + "/" + this.idDest, null);
						SE.WriteJournal();
						this.Close();
						Game.Load(this.idDest);
					});
				}
				Dialog.YesNo(langDetail, actionYes, null, "yes", "no");
				return;
			}
			if (!i.madeBackup && i.version.IsBelow(ELayer.core.version))
			{
				GameIO.MakeBackup(i, "");
				ELayer.ui.Say("backupDone", null);
				GameIO.UpdateGameIndex(i);
			}
			Game.Load(i.id);
		});
		Action <>9__6;
		this.buttonDelete.SetOnClick(delegate
		{
			string langDetail = "dialogDeleteGame";
			Action actionYes;
			if ((actionYes = <>9__6) == null)
			{
				actionYes = (<>9__6 = delegate()
				{
					GameIO.DeleteGame(i.id, true);
					this.worlds = null;
					this.RefreshList();
					SE.Trash();
				});
			}
			Dialog.YesNo(langDetail, actionYes, null, "yes", "no");
		});
		this.buttonListBackup.SetOnClick(delegate
		{
			ELayer.ui.AddLayer<LayerLoadGame>().Init(true, GameIO.pathBackup + i.id + "/", i.id);
		});
		this.buttonBackup.SetOnClick(delegate
		{
			if (ELayer.core.IsGameStarted)
			{
				ELayer.game.backupTime = 0.0;
				ELayer.game.Save(false, null, false);
			}
			GameIO.MakeBackup(i, "");
			ELayer.ui.Say("backupDone", null);
			SE.WriteJournal();
		});
		this.buttonOpen.SetOnClick(delegate
		{
			Util.ShowExplorer(i.path, false);
		});
		this.buttonLoad.SetActive(flag);
	}

	public UIList list;

	public GameObject goInfo;

	public GameObject goNoInfo;

	public UINote note;

	public UIButton buttonLoad;

	public UIButton buttonDelete;

	public UIButton buttonBackup;

	public UIButton buttonOpen;

	public UIButton buttonListBackup;

	public List<GameIndex> worlds;

	public Portrait portrait;

	public UIText textAka;

	public UIText textName;

	public UIText textInfo;

	public UIText textRace;

	public UIText textJob;

	public UIText textDeepest;

	public UIText textDays;

	private string pathRoot;

	private string idDest;

	private bool backup;
}
