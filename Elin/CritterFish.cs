public class CritterFish : Critter
{
	public static int[] I_TILE = new int[1] { 192 };

	public static int[] A_TILE = new int[2] { 192, 193 };

	public override int[] idleTiles => I_TILE;

	public override int[] animeTiles => A_TILE;

	public override int SnowTile => 194;
}
