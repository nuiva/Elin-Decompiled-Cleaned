using System;

public class VirtualRoom : BaseArea
{
	public VirtualRoom(Card t)
	{
		EClass._map.ForeachSphere(t.pos.x, t.pos.z, (float)t.trait.radius, delegate(Point p)
		{
			this.points.Add(p.Copy());
		});
	}
}
