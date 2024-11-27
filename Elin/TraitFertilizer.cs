using System;
using UnityEngine;

public class TraitFertilizer : Trait
{
	public override bool CanExtendBuild
	{
		get
		{
			return true;
		}
	}

	public bool Defertilize
	{
		get
		{
			return this is TraitDefertilizer;
		}
	}

	public override void OnSimulateHour(VirtualDate date)
	{
		if (!this.owner.IsInstalled)
		{
			return;
		}
		PlantData plantData = EClass._map.TryGetPlant(this.owner.Cell);
		bool flag = false;
		if (this.Defertilize)
		{
			if (plantData == null)
			{
				plantData = EClass._map.AddPlant(this.owner.pos, null);
			}
			plantData.fert = -1;
		}
		else
		{
			foreach (Card card in this.owner.pos.ListCards(false))
			{
				if (card.trait is TraitSeed && !card.isSale)
				{
					flag = true;
					(card.trait as TraitSeed).TrySprout(true, true, null);
					break;
				}
			}
			if (!this.owner.pos.HasObj)
			{
				if (flag)
				{
					return;
				}
				EClass._map.SetObj(this.owner.pos.x, this.owner.pos.z, 5, 1, 0);
			}
			else if (plantData == null)
			{
				if (this.owner.pos.growth == null)
				{
					return;
				}
				this.owner.pos.growth.TryGrow(date);
			}
			else if (plantData.fert == 0 && this.owner.pos.growth != null)
			{
				this.owner.pos.growth.TryGrow(date);
			}
			plantData = EClass._map.TryGetPlant(this.owner.Cell);
			if (plantData != null)
			{
				plantData.fert++;
			}
		}
		if (date.IsRealTime)
		{
			this.owner.PlaySound("mutation", 1f, true);
			this.owner.PlayEffect("mutation", true, 0f, default(Vector3));
		}
		this.owner.Destroy();
	}
}
