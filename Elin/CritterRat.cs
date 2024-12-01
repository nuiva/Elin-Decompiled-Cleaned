public class CritterRat : Critter
{
	public static int[] I_TILE = new int[1] { 97 };

	public static int[] A_TILE = new int[2] { 96, 97 };

	public override int[] idleTiles => I_TILE;

	public override int[] animeTiles => A_TILE;
}
