using System;

public class CharaGen : CardGen
{
	public static Chara _Create(string id, int idMat = -1, int lv = -1)
	{
		Chara chara = new Chara();
		if (lv < 1)
		{
			lv = 1;
		}
		CharaGen.objLv = lv;
		chara.Create(id, idMat, lv);
		if (EClass.player != null)
		{
			EClass.player.codex.AddSpawn(id);
		}
		return chara;
	}

	public static Chara Create(string id, int lv = -1)
	{
		return CharaGen._Create(id, -1, lv);
	}

	public static Chara CreateFromFilter(string id, int lv = -1, int levelRange = -1)
	{
		return CharaGen.CreateFromFilter(SpawnList.Get(id, null, null), lv, levelRange);
	}

	public static Chara CreateFromFilter(SpawnList list, int lv = -1, int levelRange = -1)
	{
		return CharaGen.Create(list.Select(lv, levelRange).id, lv);
	}

	public static Chara CreateFromElement(string idEle, int lv = -1, string idFilter = "chara")
	{
		SpawnList spawnList = SpawnListChara.Get("chara_ele" + idEle, delegate(SourceChara.Row c)
		{
			string[] mainElement = c.mainElement;
			for (int i = 0; i < mainElement.Length; i++)
			{
				if (mainElement[i].Split(',', StringSplitOptions.None)[0] == idEle)
				{
					return true;
				}
			}
			return false;
		});
		CardBlueprint.Set(new CardBlueprint
		{
			idEle = idEle
		});
		return CharaGen.Create(spawnList.Select(lv, -1).id, lv);
	}

	public static Chara CreateWealthy(int lv = -1)
	{
		return CharaGen.CreateFromFilter(SpawnListChara.Get("c_wealthy", (SourceChara.Row r) => r.works.Contains("Rich") || r.hobbies.Contains("Rich")), lv, -1);
	}

	public static int objLv;
}
