public class GrowSystemBerry : GrowSystemCrop
{
	protected override bool DrawHarvestOnTop => true;

	public override void OnMineObj(Chara c = null)
	{
		TryPick(GrowSystem.cell, "grass", EClass.sources.materials.alias["grass_forest"].id, EClass.rnd(2));
		base.OnMineObj(c);
	}
}
