using UnityEngine;

public class TraitFactory : TraitCrafter
{
	public Recipe recipe;

	public override bool IsFactory => true;

	public override string idSoundProgress => recipe.GetMainMaterial().GetSoundCraft(recipe.renderRow);

	public override int GetActDuration(Chara c)
	{
		return 20;
	}

	public override int GetCostSp(AI_UseCrafter ai)
	{
		return recipe.source.GetSPCost(owner);
	}

	public override int GetDuration(AI_UseCrafter ai, int costSp)
	{
		return Mathf.Min(costSp * 4, 30);
	}
}
