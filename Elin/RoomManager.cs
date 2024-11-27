using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class RoomManager : EClass
{
	public void OnLoad()
	{
		foreach (Area area in this.listArea)
		{
			area.OnLoad();
		}
		foreach (Room room in this.listRoom)
		{
			room.OnLoad();
		}
		this.Refresh();
	}

	public void AssignCharas()
	{
	}

	public void RefreshAll()
	{
		foreach (Room room in this.listRoom)
		{
			room.SetDirty();
		}
		this.Refresh();
	}

	public void Refresh()
	{
		if (this.dirtyRooms)
		{
			foreach (Room room in this.listRoom)
			{
				if (room.dirty)
				{
					room.Refresh();
				}
			}
		}
		if (this.dirtyLots)
		{
			this.RebuildLots();
		}
	}

	public void AssignUID(BaseArea a)
	{
		a.uid = this.uidRoom;
		this.uidRoom++;
	}

	public Area AddArea(Area a, Point p)
	{
		if (!this.listArea.Contains(a))
		{
			this.listArea.Add(a);
			this.mapIDs.Add(a.uid, a);
		}
		a.AddPoint(p.Copy(), false);
		return a;
	}

	public Area TryAddArea(Point p, Area existingArea)
	{
		if (p.area != null)
		{
			return existingArea;
		}
		if (existingArea == null)
		{
			existingArea = Area.Create("public");
			this.listArea.Add(existingArea);
			this.mapIDs.Add(existingArea.uid, existingArea);
		}
		existingArea.AddPoint(p.Copy(), false);
		return existingArea;
	}

	public void RemoveArea(Area a)
	{
		this.listArea.Remove(a);
		this.mapIDs.Remove(a.uid);
		a.OnRemove();
	}

	public Room AddRoom(Room r)
	{
		this.listRoom.Add(r);
		this.AssignUID(r);
		r.SetDirty();
		this.mapIDs.Add(r.uid, r);
		return r;
	}

	public void RemoveRoom(Room r)
	{
		this.listRoom.Remove(r);
		this.mapIDs.Remove(r.uid);
		r.OnRemove();
	}

	public HitResult GetHitResult(Point point, Point start)
	{
		if (point.area != null)
		{
			return HitResult.Invalid;
		}
		if (!point.HasBlock)
		{
			return HitResult.Valid;
		}
		return HitResult.Default;
	}

	public void RebuildLots()
	{
		this.listLot.Clear();
		foreach (Room room in this.listRoom)
		{
			room.lot = null;
		}
		foreach (Room room2 in this.listRoom)
		{
			if (room2.lot == null)
			{
				Lot lot = new Lot
				{
					x = room2.x,
					z = room2.z,
					mx = room2.mx,
					mz = room2.mz
				};
				lot.SetBaseRoom(room2);
				this.listLot.Add(lot);
			}
		}
		this.dirtyLots = false;
	}

	public BaseArea FindBaseArea(string id)
	{
		this.tempList.Clear();
		foreach (BaseArea baseArea in this.listRoom.Concat(this.listArea))
		{
			if (baseArea.type.id == id)
			{
				this.tempList.Add(baseArea);
			}
		}
		if (this.tempList.Count == 0)
		{
			return null;
		}
		return this.tempList.RandomItem<BaseArea>();
	}

	private List<BaseArea> tempList = new List<BaseArea>();

	[JsonProperty]
	public List<Area> listArea = new List<Area>();

	[JsonProperty]
	public List<Room> listRoom = new List<Room>();

	[JsonProperty]
	public int uidRoom = 1;

	public Dictionary<int, BaseArea> mapIDs = new Dictionary<int, BaseArea>();

	public List<Lot> listLot = new List<Lot>();

	public bool dirtyLots;

	public bool dirtyRooms;
}
