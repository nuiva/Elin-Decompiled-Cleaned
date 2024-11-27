using System;

public class LogicalPoint : Point
{
	public virtual LogicalPointManager manager
	{
		get
		{
			return null;
		}
	}

	public new void Set(Point p)
	{
		this.x = p.x;
		this.z = p.z;
		this.index = base.index;
	}

	public virtual void Update()
	{
	}

	public virtual void Kill()
	{
		this.manager.Remove(this.index);
	}

	public new int index;

	public bool open;

	public int life = 5;
}
