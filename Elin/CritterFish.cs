using System;

public class CritterFish : Critter
{
	public override int[] idleTiles
	{
		get
		{
			return CritterFish.I_TILE;
		}
	}

	public override int[] animeTiles
	{
		get
		{
			return CritterFish.A_TILE;
		}
	}

	public override int SnowTile
	{
		get
		{
			return 194;
		}
	}

	public static int[] I_TILE = new int[]
	{
		192
	};

	public static int[] A_TILE = new int[]
	{
		192,
		193
	};
}
