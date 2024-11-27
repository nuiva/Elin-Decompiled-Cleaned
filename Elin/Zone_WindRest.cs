using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Zone_WindRest : Zone
{
	public override bool IsSnowCovered
	{
		get
		{
			return false;
		}
	}

	public override void OnGenerateMap()
	{
		base.OnGenerateMap();
		List<Dictionary<string, string>> list = new ExcelData("Data/Raw/dagger_graves", 1).BuildList("_default");
		list.RemoveAt(0);
		int num = 0;
		foreach (Thing thing in EClass._map.things)
		{
			if (!(thing.id != "grave_dagger1") || !(thing.id != "grave_dagger2"))
			{
				thing.isOn = false;
				if (num < list.Count)
				{
					Dictionary<string, string> dictionary = list[num];
					int num2 = dictionary["id"].ToInt();
					num++;
					if (num2 != 0)
					{
						thing.c_note = dictionary["First_" + (Lang.isJP ? "JP" : "EN")];
						thing.c_context = dictionary["Full"];
						thing.c_idBacker = num2;
						thing.isModified = true;
					}
				}
			}
		}
		Debug.Log(num.ToString() + "/" + list.Count.ToString());
	}

	public override void OnAfterSimulate()
	{
		base.OnAfterSimulate();
		if (base.HourSinceLastActive >= 168)
		{
			Zone_WindRest.<OnAfterSimulate>g__TryRevive|3_0("eureka", 60, 61);
			Zone_WindRest.<OnAfterSimulate>g__TryRevive|3_0("billy", 35, 64);
		}
	}

	[CompilerGenerated]
	internal static void <OnAfterSimulate>g__TryRevive|3_0(string id, int x, int y)
	{
		if (EClass.game.cards.globalCharas.Find(id) == null)
		{
			bool flag = false;
			using (List<Chara>.Enumerator enumerator = EClass._map.charas.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.id == id)
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				EClass._zone.AddCard(CharaGen.Create(id, -1), x, y);
			}
		}
	}
}
