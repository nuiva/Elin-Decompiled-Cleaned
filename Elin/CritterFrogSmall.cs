public class CritterFrogSmall : Critter
{
	public static int[] I_TILE = new int[4] { 67, 67, 69, 69 };

	public static int[] A_TILE = new int[2] { 67, 68 };

	public override int[] idleTiles => I_TILE;

	public override int[] animeTiles => A_TILE;
}
