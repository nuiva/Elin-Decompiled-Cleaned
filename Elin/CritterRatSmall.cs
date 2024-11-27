using System;

public class CritterRatSmall : Critter
{
	public override int[] idleTiles
	{
		get
		{
			return CritterRatSmall.I_TILE;
		}
	}

	public override int[] animeTiles
	{
		get
		{
			return CritterRatSmall.A_TILE;
		}
	}

	public static int[] I_TILE = new int[]
	{
		99
	};

	public static int[] A_TILE = new int[]
	{
		98,
		99
	};
}
