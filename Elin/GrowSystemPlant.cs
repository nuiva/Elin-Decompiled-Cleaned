public class GrowSystemPlant : GrowSystem
{
	public bool Bud => base.stage.idx == 0;

	public bool Young => base.stage.idx == 1;

	public bool Grown => base.stage.idx == 2;

	public bool Mature => base.stage.idx == 3;

	public bool Withered => base.stage.idx == 4;

	public override void OnMineObj(Chara c = null)
	{
		TryPick(GrowSystem.cell, "grass", EClass.sources.materials.alias["grass"].id, EClass.rnd(2));
		base.OnMineObj(c);
	}
}
