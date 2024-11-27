using System;
using UnityEngine;

public class TraitTrainingDummyArmor : TraitTrainingDummy
{
	public void CalcPV()
	{
		this.owner.elements.SetBase(65, this.owner.material.hardness * this.owner.material.hardness / 10 * Mathf.Max(this.owner.encLV + 1, 1), 0);
	}

	public override void OnCreate(int lv)
	{
		this.CalcPV();
	}

	public override void OnChangePlaceState(PlaceState state)
	{
		this.CalcPV();
	}
}
