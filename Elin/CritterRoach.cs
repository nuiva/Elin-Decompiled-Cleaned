public class CritterRoach : Critter
{
	public static int[] I_TILE = new int[1] { 160 };

	public static int[] A_TILE = new int[2] { 160, 161 };

	public override int[] idleTiles => I_TILE;

	public override int[] animeTiles => A_TILE;
}
