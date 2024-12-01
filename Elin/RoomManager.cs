using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class RoomManager : EClass
{
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

	public void OnLoad()
	{
		foreach (Area item in listArea)
		{
			item.OnLoad();
		}
		foreach (Room item2 in listRoom)
		{
			item2.OnLoad();
		}
		Refresh();
	}

	public void AssignCharas()
	{
	}

	public void RefreshAll()
	{
		foreach (Room item in listRoom)
		{
			item.SetDirty();
		}
		Refresh();
	}

	public void Refresh()
	{
		if (dirtyRooms)
		{
			foreach (Room item in listRoom)
			{
				if (item.dirty)
				{
					item.Refresh();
				}
			}
		}
		if (dirtyLots)
		{
			RebuildLots();
		}
	}

	public void AssignUID(BaseArea a)
	{
		a.uid = uidRoom;
		uidRoom++;
	}

	public Area AddArea(Area a, Point p)
	{
		if (!listArea.Contains(a))
		{
			listArea.Add(a);
			mapIDs.Add(a.uid, a);
		}
		a.AddPoint(p.Copy());
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
			listArea.Add(existingArea);
			mapIDs.Add(existingArea.uid, existingArea);
		}
		existingArea.AddPoint(p.Copy());
		return existingArea;
	}

	public void RemoveArea(Area a)
	{
		listArea.Remove(a);
		mapIDs.Remove(a.uid);
		a.OnRemove();
	}

	public Room AddRoom(Room r)
	{
		listRoom.Add(r);
		AssignUID(r);
		r.SetDirty();
		mapIDs.Add(r.uid, r);
		return r;
	}

	public void RemoveRoom(Room r)
	{
		listRoom.Remove(r);
		mapIDs.Remove(r.uid);
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
		listLot.Clear();
		foreach (Room item in listRoom)
		{
			item.lot = null;
		}
		foreach (Room item2 in listRoom)
		{
			if (item2.lot == null)
			{
				Lot lot = new Lot
				{
					x = item2.x,
					z = item2.z,
					mx = item2.mx,
					mz = item2.mz
				};
				lot.SetBaseRoom(item2);
				listLot.Add(lot);
			}
		}
		dirtyLots = false;
	}

	public BaseArea FindBaseArea(string id)
	{
		tempList.Clear();
		foreach (BaseArea item in ((IEnumerable<BaseArea>)listRoom).Concat((IEnumerable<BaseArea>)listArea))
		{
			if (item.type.id == id)
			{
				tempList.Add(item);
			}
		}
		if (tempList.Count == 0)
		{
			return null;
		}
		return tempList.RandomItem();
	}
}
