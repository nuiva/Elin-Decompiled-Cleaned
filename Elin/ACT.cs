using System;
using System.Collections.Generic;

public class ACT : EClass
{
	public static void Init()
	{
		foreach (SourceElement.Row row in EClass.sources.elements.rows)
		{
			if (row.group == "ABILITY" || row.group == "SPELL")
			{
				ACT.dict[row.alias] = ACT.Create(row);
			}
		}
	}

	public static Act Create(int id)
	{
		return ACT.Create(EClass.sources.elements.map[id]);
	}

	public static Act Create(string id)
	{
		return ACT.Create(EClass.sources.elements.alias[id]);
	}

	public static Act Create(SourceElement.Row row)
	{
		Act act = ClassCache.Create<Act>(row.type.IsEmpty(row.alias), "Elin") ?? new Act();
		if (act != null)
		{
			act.id = row.id;
			act._source = row;
		}
		return act;
	}

	public new static ActWait Wait = new ActWait();

	public static ActChat Chat = new ActChat();

	public static ActPick Pick = new ActPick();

	public static ActKick Kick = new ActKick();

	public static ActMelee Melee = new ActMelee();

	public static ActRanged Ranged = new ActRanged();

	public static ActThrow Throw = new ActThrow();

	public static ActItem Item = new ActItem();

	public static Dictionary<string, Act> dict = new Dictionary<string, Act>();
}
