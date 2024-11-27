using System;

public class CritterRat : Critter
{
	public override int[] idleTiles
	{
		get
		{
			return CritterRat.I_TILE;
		}
	}

	public override int[] animeTiles
	{
		get
		{
			return CritterRat.A_TILE;
		}
	}

	public static int[] I_TILE = new int[]
	{
		97
	};

	public static int[] A_TILE = new int[]
	{
		96,
		97
	};
}
