using UnityEngine;

public class TraitAmmoTalisman : TraitAmmo
{
	public override bool ConsumeOnMiss => false;

	public override void SetName(ref string s)
	{
		if (EClass.sources.elements.map.TryGetValue(owner.refVal) == null)
		{
			Debug.Log(owner.refVal);
		}
		else
		{
			s = s + " (" + EClass.sources.elements.map[owner.refVal].GetName().ToTitleCase() + ")";
		}
	}
}
