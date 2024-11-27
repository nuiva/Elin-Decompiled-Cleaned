using System;
using UnityEngine;

public class TraitAmmoTalisman : TraitAmmo
{
	public override bool ConsumeOnMiss
	{
		get
		{
			return false;
		}
	}

	public override void SetName(ref string s)
	{
		if (EClass.sources.elements.map.TryGetValue(this.owner.refVal, null) == null)
		{
			Debug.Log(this.owner.refVal);
			return;
		}
		s = s + " (" + EClass.sources.elements.map[this.owner.refVal].GetName().ToTitleCase(false) + ")";
	}
}
