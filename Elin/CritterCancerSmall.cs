using System;

public class CritterCancerSmall : Critter
{
	public override int[] idleTiles
	{
		get
		{
			return CritterCancerSmall.I_TILE;
		}
	}

	public override int[] animeTiles
	{
		get
		{
			return CritterCancerSmall.A_TILE;
		}
	}

	public static int[] I_TILE = new int[]
	{
		130
	};

	public static int[] A_TILE = new int[]
	{
		130,
		131
	};
}
