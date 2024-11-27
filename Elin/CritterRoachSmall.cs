using System;

public class CritterRoachSmall : Critter
{
	public override int[] idleTiles
	{
		get
		{
			return CritterRoachSmall.I_TILE;
		}
	}

	public override int[] animeTiles
	{
		get
		{
			return CritterRoachSmall.A_TILE;
		}
	}

	public static int[] I_TILE = new int[]
	{
		162
	};

	public static int[] A_TILE = new int[]
	{
		162,
		163
	};
}
