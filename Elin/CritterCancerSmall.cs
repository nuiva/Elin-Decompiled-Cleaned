public class CritterCancerSmall : Critter
{
	public static int[] I_TILE = new int[1] { 130 };

	public static int[] A_TILE = new int[2] { 130, 131 };

	public override int[] idleTiles => I_TILE;

	public override int[] animeTiles => A_TILE;
}
