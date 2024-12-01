public class VirtualRoom : BaseArea
{
	public VirtualRoom(Card t)
	{
		EClass._map.ForeachSphere(t.pos.x, t.pos.z, t.trait.radius, delegate(Point p)
		{
			points.Add(p.Copy());
		});
	}
}
