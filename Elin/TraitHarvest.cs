public class TraitHarvest : Trait
{
	public override string ReqHarvest => GetParam(1) + "," + GetParam(2);
}
