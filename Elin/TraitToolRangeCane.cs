using System;
using UnityEngine;

public class TraitToolRangeCane : TraitToolRange
{
	public override bool NeedAmmo
	{
		get
		{
			return false;
		}
	}

	public override bool NeedReload
	{
		get
		{
			return false;
		}
	}

	public override Element WeaponSkill
	{
		get
		{
			return this.owner.elements.GetOrCreateElement(305);
		}
	}

	public override void OnCreate(int lv)
	{
		this.owner.elements.SetBase(Element.GetRandomElement(lv).id, Mathf.Clamp(EClass.rndHalf(10 + lv / 2), 1, 50), 0);
	}
}
