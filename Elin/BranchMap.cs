using System;
using System.Collections.Generic;

public class BranchMap : EClass
{
	public void Refresh()
	{
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.IsInstalled && thing.trait is TraitAltar)
			{
				TraitAltar traitAltar = thing.trait as TraitAltar;
				this.altarMap.Add(traitAltar.idDeity);
			}
		}
	}

	public HashSet<string> altarMap = new HashSet<string>();
}
