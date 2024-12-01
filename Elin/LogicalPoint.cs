public class LogicalPoint : Point
{
	public new int index;

	public bool open;

	public int life = 5;

	public virtual LogicalPointManager manager => null;

	public new void Set(Point p)
	{
		x = p.x;
		z = p.z;
		index = base.index;
	}

	public virtual void Update()
	{
	}

	public virtual void Kill()
	{
		manager.Remove(index);
	}
}
