public class CritterCancer : Critter
{
	public static int[] I_TILE = new int[1] { 128 };

	public static int[] A_TILE = new int[2] { 128, 129 };

	public override int[] idleTiles => I_TILE;

	public override int[] animeTiles => A_TILE;
}
