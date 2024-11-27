using System;
using UnityEngine;

public class TraitBlanket : Trait
{
	public override bool CanStack
	{
		get
		{
			return false;
		}
	}

	public override bool HasCharges
	{
		get
		{
			return true;
		}
	}

	public override void OnCreate(int lv)
	{
		this.owner.c_charges = EClass.rndHalf(8 + Mathf.Clamp(this.owner.QualityLv * 2, -2, 30));
	}

	public override void OnCrafted(Recipe recipe)
	{
		this.OnCreate(this.owner.LV);
	}
}
