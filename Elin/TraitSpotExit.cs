public class TraitSpotExit : Trait
{
	public override int radius => 2;

	public override bool CanBuiltAt(Point p)
	{
		if (p.x >= 2 && p.z >= 2 && p.x < EClass._map.Size - 2 && p.z < EClass._map.Size - 2)
		{
			return false;
		}
		return true;
	}
}
