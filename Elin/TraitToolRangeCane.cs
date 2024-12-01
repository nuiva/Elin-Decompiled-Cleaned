using UnityEngine;

public class TraitToolRangeCane : TraitToolRange
{
	public override bool NeedAmmo => false;

	public override bool NeedReload => false;

	public override Element WeaponSkill => owner.elements.GetOrCreateElement(305);

	public override void OnCreate(int lv)
	{
		owner.elements.SetBase(Element.GetRandomElement(lv).id, Mathf.Clamp(EClass.rndHalf(10 + lv / 2), 1, 50));
	}
}
