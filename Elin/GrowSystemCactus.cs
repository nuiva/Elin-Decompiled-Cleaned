public class GrowSystemCactus : GrowSystemCrop
{
	protected override bool DrawHarvestOnTop => true;

	protected override bool UseGenericFirstStageTile => false;

	public override void OnMineObj(Chara c = null)
	{
		TryPick(GrowSystem.cell, "needle");
	}
}
