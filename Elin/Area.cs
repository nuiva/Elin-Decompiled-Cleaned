using System;
using Newtonsoft.Json;

public class Area : BaseArea
{
	public virtual bool isListable
	{
		get
		{
			return true;
		}
	}

	public virtual bool AutoTask
	{
		get
		{
			return false;
		}
	}

	public RoomManager manager
	{
		get
		{
			return EClass._map.rooms;
		}
	}

	public virtual void OnLoad()
	{
		foreach (Point point in this.points)
		{
			this.AddPoint(point, true);
		}
		this.taskList.OnLoad();
		this.type.owner = this;
		this.manager.mapIDs.Add(this.uid, this);
	}

	public void AddPoint(Point point, bool onLoad = false)
	{
		if (!onLoad)
		{
			this.points.Add(point);
		}
		point.cell.GetOrCreateDetail().area = this;
	}

	public void RemovePoint(Point point)
	{
		if (this.points.Count <= 1)
		{
			return;
		}
		foreach (Point point2 in this.points)
		{
			if (point2.Equals(point))
			{
				this.OnRemovePoint(point2);
				this.points.Remove(point2);
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
		foreach (Point point in this.points)
		{
			pass.Add(point, (float)((EClass.scene.actionMode.AreaHihlight == AreaHighlightMode.Edit) ? 34 : 33), 0f);
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
		if (this.isDestroyed)
		{
			return;
		}
		this.isDestroyed = true;
		Task[] array = this.taskList.items.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Destroy();
		}
		foreach (Point point in this.points)
		{
			this.OnRemovePoint(point);
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

	[JsonProperty]
	public TaskList<Task> taskList = new TaskList<Task>();

	public bool isDestroyed;
}
