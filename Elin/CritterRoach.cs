using System;

public class CritterRoach : Critter
{
	public override int[] idleTiles
	{
		get
		{
			return CritterRoach.I_TILE;
		}
	}

	public override int[] animeTiles
	{
		get
		{
			return CritterRoach.A_TILE;
		}
	}

	public static int[] I_TILE = new int[]
	{
		160
	};

	public static int[] A_TILE = new int[]
	{
		160,
		161
	};
}
