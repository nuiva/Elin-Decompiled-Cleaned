public class CritterFrog : Critter
{
	public static int[] I_TILE = new int[4] { 64, 64, 66, 66 };

	public static int[] A_TILE = new int[2] { 64, 65 };

	public override int[] idleTiles => I_TILE;

	public override int[] animeTiles => A_TILE;
}
