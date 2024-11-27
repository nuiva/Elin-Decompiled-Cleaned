using System;
using System.Collections.Generic;

public class LogicalPointManager : EClass
{
	public virtual LogicalPoint Create()
	{
		return null;
	}

	public virtual bool AllowBlock
	{
		get
		{
			return false;
		}
	}

	public LogicalPoint GetOrCreate(Point point)
	{
		if (!point.IsValid)
		{
			return null;
		}
		int index = point.index;
		LogicalPoint logicalPoint = this.dict.TryGetValue(index, null);
		if (logicalPoint == null)
		{
			if (!this.AllowBlock && point.cell.HasBlock)
			{
				return null;
			}
			logicalPoint = this.Create();
			logicalPoint.Set(point);
			this.list.Add(logicalPoint);
			this.dict.Add(index, logicalPoint);
		}
		return logicalPoint;
	}

	public LogicalPoint Refresh(Point point)
	{
		return this.GetOrCreate(point);
	}

	public void Remove(int key)
	{
		LogicalPoint item = this.dict[key];
		this.list.Remove(item);
		this.dict.Remove(key);
	}

	public List<LogicalPoint> list = new List<LogicalPoint>();

	public Dictionary<int, LogicalPoint> dict = new Dictionary<int, LogicalPoint>();

	public HashSet<Point> refreshList = new HashSet<Point>();
}
