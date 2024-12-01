using System.Collections.Generic;
using UnityEngine;

public class SourceAsset : EScriptable
{
	public class PrefData
	{
		public Dictionary<string, SourcePref> dict = new Dictionary<string, SourcePref>();
	}

	public class Prefs
	{
		public int version;

		public PrefData things = new PrefData();
	}

	public string idLoad = "prefs";

	public UD_String_String renames;

	public static string PrefPath => Application.dataPath + "/Resources/Data/Export/";

	public void DoFix()
	{
	}

	public void SavePrefs(string id = "prefs")
	{
		_SavePrefs(id);
	}

	public static void _SavePrefs(string id = "prefs")
	{
		IO.CopyAs(PrefPath + id, PrefPath + id + "_bk");
		Prefs prefs = new Prefs();
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
		IO.SaveFile(PrefPath + id, prefs);
		Debug.Log("Exported Prefs:" + id);
	}

	public void LoadPrefs()
	{
		_LoadPrefs(idLoad);
	}

	public void LoadPrefs_bk()
	{
		_LoadPrefs(idLoad);
	}

	public static void _LoadPrefs(string id = "prefs")
	{
		IO.CopyAs(PrefPath + id, PrefPath + id + "_loadbk");
		Prefs prefs = IO.LoadFile<Prefs>(PrefPath + id);
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
}
