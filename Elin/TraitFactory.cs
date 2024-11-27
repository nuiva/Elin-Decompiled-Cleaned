using System;
using UnityEngine;

public class TraitFactory : TraitCrafter
{
	public override int GetActDuration(Chara c)
	{
		return 20;
	}

	public override bool IsFactory
	{
		get
		{
			return true;
		}
	}

	public override string idSoundProgress
	{
		get
		{
			return this.recipe.GetMainMaterial().GetSoundCraft(this.recipe.renderRow);
		}
	}

	public override int GetCostSp(AI_UseCrafter ai)
	{
		return this.recipe.source.GetSPCost(this.owner);
	}

	public override int GetDuration(AI_UseCrafter ai, int costSp)
	{
		return Mathf.Min(costSp * 4, 30);
	}

	public Recipe recipe;
}
