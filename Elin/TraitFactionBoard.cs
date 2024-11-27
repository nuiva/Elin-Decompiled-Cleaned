using System;
using System.Collections.Generic;

public class TraitFactionBoard : TraitBoard
{
	public override bool IsHomeItem
	{
		get
		{
			return true;
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (!EClass._zone.IsPCFaction)
		{
			return;
		}
		p.TrySetAct("actChangeFactionName", delegate()
		{
			EClass.ui.AddLayer<LayerList>().SetStringList(delegate
			{
				List<string> list = new List<string>();
				for (int i = 0; i < 10; i++)
				{
					list.Add(WordGen.GetCombinedName(this.GetAlias(), "faction", false));
				}
				return list;
			}, delegate(int a, string b)
			{
				EClass.Home.name = b;
			}, true).SetSize(450f, -1f).EnableReroll();
			return false;
		}, this.owner, null, 1, false, true, false);
		p.TrySetAct("actShowSigns", delegate()
		{
			SE.Click();
			foreach (Thing thing in EClass._map.things)
			{
				if (thing.IsInstalled && thing.source._origin == "sign")
				{
					thing.isMasked = false;
				}
			}
			return false;
		}, this.owner, null, 1, false, true, false);
		p.TrySetAct("actHideSigns", delegate()
		{
			SE.Click();
			foreach (Thing thing in EClass._map.things)
			{
				if (thing.IsInstalled && thing.source._origin == "sign")
				{
					thing.isMasked = true;
				}
			}
			return false;
		}, this.owner, null, 1, false, true, false);
	}

	public string GetAlias()
	{
		return EClass.player.title;
	}
}
