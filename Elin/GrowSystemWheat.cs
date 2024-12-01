public class GrowSystemWheat : GrowSystemCrop
{
	protected override bool UseGenericFirstStageTile => false;

	public override int HarvestStage => 3;

	public override int AutoMineStage => 3;

	public override string GetSoundProgress()
	{
		return source.DefaultMaterial.GetSoundImpact();
	}

	public override int GetStageTile()
	{
		if (GrowSystem.currentStage.idx == StageLength - 1 && GrowSystem.cell.isHarvested)
		{
			return harvestTile + 1;
		}
		return base.GetStageTile();
	}

	public override void OnMineObj(Chara c = null)
	{
		if (IsWithered() || IsHarvestStage(base.stage.idx))
		{
			TryPick(GrowSystem.cell, ThingGen.Create("grass", "straw"), c);
		}
		else
		{
			base.OnMineObj(c);
		}
	}
}
