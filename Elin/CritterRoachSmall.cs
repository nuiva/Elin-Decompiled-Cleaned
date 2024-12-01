public class CritterRoachSmall : Critter
{
	public static int[] I_TILE = new int[1] { 162 };

	public static int[] A_TILE = new int[2] { 162, 163 };

	public override int[] idleTiles => I_TILE;

	public override int[] animeTiles => A_TILE;
}
