using System;

public class CritterRandom : Critter
{
	public override int AnimeTile
	{
		get
		{
			return this._tile;
		}
	}

	public override int IdleTile
	{
		get
		{
			return this._tile;
		}
	}

	public CritterRandom()
	{
		this._tile = EClass.rnd(12) + 1;
	}

	public int _tile;
}
