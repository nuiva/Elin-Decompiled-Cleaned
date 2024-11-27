using System;

public class CritterFrogSmall : Critter
{
	public override int[] idleTiles
	{
		get
		{
			return CritterFrogSmall.I_TILE;
		}
	}

	public override int[] animeTiles
	{
		get
		{
			return CritterFrogSmall.A_TILE;
		}
	}

	public static int[] I_TILE = new int[]
	{
		67,
		67,
		69,
		69
	};

	public static int[] A_TILE = new int[]
	{
		67,
		68
	};
}
