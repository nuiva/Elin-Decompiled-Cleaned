using UnityEngine;

public class TraitTrainingDummyArmor : TraitTrainingDummy
{
	public void CalcPV()
	{
		owner.elements.SetBase(65, owner.material.hardness * owner.material.hardness / 10 * Mathf.Max(owner.encLV + 1, 1));
	}

	public override void OnCreate(int lv)
	{
		CalcPV();
	}

	public override void OnChangePlaceState(PlaceState state)
	{
		CalcPV();
	}
}
