using System.Collections.Generic;

public class LogicalPointManager : EClass
{
	public List<LogicalPoint> list = new List<LogicalPoint>();

	public Dictionary<int, LogicalPoint> dict = new Dictionary<int, LogicalPoint>();

	public HashSet<Point> refreshList = new HashSet<Point>();

	public virtual bool AllowBlock => false;

	public virtual LogicalPoint Create()
	{
		return null;
	}

	public LogicalPoint GetOrCreate(Point point)
	{
		if (!point.IsValid)
		{
			return null;
		}
		int index = point.index;
		LogicalPoint logicalPoint = dict.TryGetValue(index);
		if (logicalPoint == null)
		{
			if (!AllowBlock && point.cell.HasBlock)
			{
				return null;
			}
			logicalPoint = Create();
			logicalPoint.Set(point);
			list.Add(logicalPoint);
			dict.Add(index, logicalPoint);
		}
		return logicalPoint;
	}

	public LogicalPoint Refresh(Point point)
	{
		return GetOrCreate(point);
	}

	public void Remove(int key)
	{
		LogicalPoint item = dict[key];
		list.Remove(item);
		dict.Remove(key);
	}
}
