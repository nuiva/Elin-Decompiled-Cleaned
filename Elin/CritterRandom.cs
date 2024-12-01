public class CritterRandom : Critter
{
	public int _tile;

	public override int AnimeTile => _tile;

	public override int IdleTile => _tile;

	public CritterRandom()
	{
		_tile = EClass.rnd(12) + 1;
	}
}
