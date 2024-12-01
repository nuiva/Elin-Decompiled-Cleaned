using Newtonsoft.Json;

public class Area : BaseArea
{
	[JsonProperty]
	public TaskList<Task> taskList = new TaskList<Task>();

	public bool isDestroyed;

	public virtual bool isListable => true;

	public virtual bool AutoTask => false;

	public RoomManager manager => EClass._map.rooms;

	public virtual void OnLoad()
	{
		foreach (Point point in points)
		{
			AddPoint(point, onLoad: true);
		}
		taskList.OnLoad();
		type.owner = this;
		manager.mapIDs.Add(uid, this);
	}

	public void AddPoint(Point point, bool onLoad = false)
	{
		if (!onLoad)
		{
			points.Add(point);
		}
		point.cell.GetOrCreateDetail().area = this;
	}

	public void RemovePoint(Point point)
	{
		if (points.Count <= 1)
		{
			return;
		}
		foreach (Point point2 in points)
		{
			if (point2.Equals(point))
			{
				OnRemovePoint(point2);
				points.Remove(point2);
				point.detail.area = null;
				point.cell.TryDespawnDetail();
				break;
			}
		}
	}

	public virtual void OnRemovePoint(Point point)
	{
	}

	public virtual void OnInstallCard(Card t)
	{
	}

	public virtual void OnUninstallCard(Card t)
	{
	}

	public void OnHoverArea(MeshPass pass)
	{
		foreach (Point point in points)
		{
			pass.Add(point, (EClass.scene.actionMode.AreaHihlight == AreaHighlightMode.Edit) ? 34 : 33);
		}
	}

	public virtual byte GetTile(int index)
	{
		return 33;
	}

	public virtual void Update()
	{
	}

	public void OnRemove()
	{
		if (isDestroyed)
		{
			return;
		}
		isDestroyed = true;
		Task[] array = taskList.items.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Destroy();
		}
		foreach (Point point in points)
		{
			OnRemovePoint(point);
			point.detail.area = null;
			point.cell.TryDespawnDetail();
		}
	}

	public static Area Create(string id)
	{
		Area area = new Area();
		area.ChangeType(id);
		EClass._map.rooms.AssignUID(area);
		return area;
	}
}
