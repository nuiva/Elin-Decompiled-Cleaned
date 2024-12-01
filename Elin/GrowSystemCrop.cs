public class GrowSystemCrop : GrowSystem
{
	public override bool IsCrimeToHarvest => true;

	public override int HarvestStage => 3;

	public override void OnMineObj(Chara c = null)
	{
		if (source.alias == "grape" && (base.stage.idx == 2 || base.stage.idx == 3))
		{
			TryPick(GrowSystem.cell, "vine");
		}
		base.OnMineObj(c);
	}
}
