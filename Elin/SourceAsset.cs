using System;
using System.Collections.Generic;
using UnityEngine;

public class SourceAsset : EScriptable
{
	public static string PrefPath
	{
		get
		{
			return Application.dataPath + "/Resources/Data/Export/";
		}
	}

	public void DoFix()
	{
	}

	public void SavePrefs(string id = "prefs")
	{
		SourceAsset._SavePrefs(id);
	}

	public static void _SavePrefs(string id = "prefs")
	{
		IO.CopyAs(SourceAsset.PrefPath + id, SourceAsset.PrefPath + id + "_bk");
		SourceAsset.Prefs prefs = new SourceAsset.Prefs();
		prefs.version = 2;
		Debug.Log(EClass.sources.things.rows.Count);
		foreach (SourceThing.Row row in EClass.sources.things.rows)
		{
			if (prefs.things.dict.ContainsKey(row.id))
			{
				Debug.LogError("exception: duplicate id:" + row.id + "/" + row.name);
			}
			else
			{
				prefs.things.dict.Add(row.id, row.pref);
			}
		}
		IO.SaveFile(SourceAsset.PrefPath + id, prefs, false, null);
		Debug.Log("Exported Prefs:" + id);
	}

	public void LoadPrefs()
	{
		SourceAsset._LoadPrefs(this.idLoad);
	}

	public void LoadPrefs_bk()
	{
		SourceAsset._LoadPrefs(this.idLoad);
	}

	public static void _LoadPrefs(string id = "prefs")
	{
		IO.CopyAs(SourceAsset.PrefPath + id, SourceAsset.PrefPath + id + "_loadbk");
		SourceAsset.Prefs prefs = IO.LoadFile<SourceAsset.Prefs>(SourceAsset.PrefPath + id, false, null);
		foreach (SourceThing.Row row in EClass.sources.things.rows)
		{
			if (prefs.things.dict.ContainsKey(row.id))
			{
				row.pref = prefs.things.dict[row.id];
			}
			if (prefs.version == 0)
			{
				row.pref.y = 0f;
			}
		}
		Debug.Log("Imported Prefs:" + id);
	}

	public string idLoad = "prefs";

	public UD_String_String renames;

	public class PrefData
	{
		public Dictionary<string, SourcePref> dict = new Dictionary<string, SourcePref>();
	}

	public class Prefs
	{
		public int version;

		public SourceAsset.PrefData things = new SourceAsset.PrefData();
	}
}
