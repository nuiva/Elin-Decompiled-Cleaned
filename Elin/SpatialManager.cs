using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class SpatialManager : EClass
{
	public Zone Somewhere
	{
		get
		{
			return this.Find("somewhere");
		}
	}

	public void AssignUID(Spatial s)
	{
		s.uid = this.uidNext;
		this.uidNext++;
		this.map.Add(s);
	}

	public void Remove(Spatial s)
	{
		if (s.parent != null)
		{
			s.parent.children.Remove(s);
		}
		this.map.Remove(s.uid);
		this.listDestryoed.Add(s);
	}

	public T Find<T>(Func<T, bool> func) where T : Zone
	{
		foreach (Spatial spatial in this.map.Values)
		{
			T t = spatial as T;
			if (t != null && func(t))
			{
				return t;
			}
		}
		return default(T);
	}

	public Zone Find(string id)
	{
		foreach (Spatial spatial in this.map.Values)
		{
			if (spatial is Zone && spatial.id == id)
			{
				return spatial as Zone;
			}
		}
		return null;
	}

	public Zone Find(int uid)
	{
		return this.map.TryGetValue(uid, null) as Zone;
	}

	public List<Zone> ListReturnLocations()
	{
		if (EClass.debug.returnAnywhere)
		{
			return (from Zone a in 
				from a in this.map.Values
				where a is Zone
				select a
			where a != EClass._zone && (a.IsReturnLocation || a.IsPCFaction || (!(a is Zone_Field) && !a.IsInstance && !a.isRandomSite)) && a.parent == EClass.world.region && !a.source.tag.Contains("closed")
			select a).ToList<Zone>();
		}
		return (from Zone a in 
			from a in this.map.Values
			where a is Zone
			select a
		where a != EClass._zone && a.IsReturnLocation && a.GetTopZone().visitCount > 0 && (a.GetTopZone().FindDeepestZone() == a || EClass.pc.homeZone == a)
		select a).ToList<Zone>();
	}

	public List<Zone> Zones
	{
		get
		{
			return (from a in this.map.Values
			where a is Zone
			select a).Cast<Zone>().ToList<Zone>();
		}
	}

	[JsonProperty]
	public SpatialManager.GlobalSpatialList map = new SpatialManager.GlobalSpatialList();

	[JsonProperty]
	public int uidNext = 1;

	[JsonProperty]
	public RankedZoneManager ranks = new RankedZoneManager();

	public List<Spatial> listDestryoed = new List<Spatial>();

	public class GlobalSpatialList : Dictionary<int, Spatial>
	{
		public void Add(Spatial s)
		{
			base.Add(s.uid, s);
		}
	}
}
