using System;

public class CritterFrog : Critter
{
	public override int[] idleTiles
	{
		get
		{
			return CritterFrog.I_TILE;
		}
	}

	public override int[] animeTiles
	{
		get
		{
			return CritterFrog.A_TILE;
		}
	}

	public static int[] I_TILE = new int[]
	{
		64,
		64,
		66,
		66
	};

	public static int[] A_TILE = new int[]
	{
		64,
		65
	};
}
