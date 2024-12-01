using System.Collections.Generic;

public class BranchMap : EClass
{
	public HashSet<string> altarMap = new HashSet<string>();

	public void Refresh()
	{
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.IsInstalled && thing.trait is TraitAltar)
			{
				TraitAltar traitAltar = thing.trait as TraitAltar;
				altarMap.Add(traitAltar.idDeity);
			}
		}
	}
}
