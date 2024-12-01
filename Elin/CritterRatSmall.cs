public class CritterRatSmall : Critter
{
	public static int[] I_TILE = new int[1] { 99 };

	public static int[] A_TILE = new int[2] { 98, 99 };

	public override int[] idleTiles => I_TILE;

	public override int[] animeTiles => A_TILE;
}
