public class Level : Element
{
	public override bool CanGainExp => true;

	public override bool UsePotential => false;

	public override bool UseExpMod => false;

	public override int ExpToNext => (100 + base.Value * 10) * (100 - owner.Value(403)) / 100;
}
