using System;

public class CritterCancer : Critter
{
	public override int[] idleTiles
	{
		get
		{
			return CritterCancer.I_TILE;
		}
	}

	public override int[] animeTiles
	{
		get
		{
			return CritterCancer.A_TILE;
		}
	}

	public static int[] I_TILE = new int[]
	{
		128
	};

	public static int[] A_TILE = new int[]
	{
		128,
		129
	};
}
