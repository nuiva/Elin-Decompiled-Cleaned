using System.Collections.Generic;

public class TraitFactionBoard : TraitBoard
{
	public override bool IsHomeItem => true;

	public override void TrySetAct(ActPlan p)
	{
		if (!EClass._zone.IsPCFaction)
		{
			return;
		}
		p.TrySetAct("actChangeFactionName", delegate
		{
			EClass.ui.AddLayer<LayerList>().SetStringList(delegate
			{
				List<string> list = new List<string>();
				for (int i = 0; i < 10; i++)
				{
					list.Add(WordGen.GetCombinedName(GetAlias()));
				}
				return list;
			}, delegate(int a, string b)
			{
				EClass.Home.name = b;
			}).SetSize()
				.EnableReroll();
			return false;
		}, owner);
		p.TrySetAct("actShowSigns", delegate
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
		}, owner);
		p.TrySetAct("actHideSigns", delegate
		{
			SE.Click();
			foreach (Thing thing2 in EClass._map.things)
			{
				if (thing2.IsInstalled && thing2.source._origin == "sign")
				{
					thing2.isMasked = true;
				}
			}
			return false;
		}, owner);
	}

	public string GetAlias()
	{
		return EClass.player.title;
	}
}
