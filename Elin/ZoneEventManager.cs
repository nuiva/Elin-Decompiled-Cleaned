using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ZoneEventManager : EClass
{
	public void OnLoad(Zone _zone)
	{
		this.zone = _zone;
		foreach (ZoneEvent zoneEvent in this.list)
		{
			zoneEvent.OnLoad(this.zone);
		}
	}

	public void Add<T>(bool allowDuplicate = false) where T : ZoneEvent
	{
		this.Add(Activator.CreateInstance<T>(), allowDuplicate);
	}

	public void Add(ZoneEvent e, bool allowDuplicate = false)
	{
		if (EClass.debug.skipEvent && e.debugSkip)
		{
			return;
		}
		if (!allowDuplicate)
		{
			foreach (ZoneEvent zoneEvent in this.list)
			{
				if (e.GetType() == zoneEvent.GetType())
				{
					return;
				}
			}
		}
		this.list.Add(e);
		e.zone = this.zone;
		e.Init();
		string str = "#game zone event ";
		Type type = e.GetType();
		Debug.Log(str + ((type != null) ? type.ToString() : null) + " added.");
	}

	public void AddPreEnter(ZonePreEnterEvent e, bool executeIfActiveZone = true)
	{
		if (this.zone.IsActiveZone && executeIfActiveZone)
		{
			e.Execute();
			return;
		}
		this.listPreEnter.Add(e);
	}

	public T GetEvent<T>() where T : ZoneEvent
	{
		foreach (ZoneEvent zoneEvent in this.list)
		{
			if (zoneEvent is T)
			{
				return zoneEvent as T;
			}
		}
		return default(T);
	}

	public void Remove<T>() where T : ZoneEvent
	{
		for (int i = this.list.Count - 1; i >= 0; i--)
		{
			if (this.list[i] is T)
			{
				this.list[i].Kill();
			}
		}
	}

	public void Remove(ZoneEvent e)
	{
		this.list.Remove(e);
	}

	public void Clear()
	{
		this.list.Clear();
	}

	public void Tick(float delta)
	{
		this.list.ForeachReverse(delegate(ZoneEvent e)
		{
			e.Tick(delta);
		});
	}

	public void OnVisit()
	{
		foreach (ZoneEvent zoneEvent in this.list)
		{
			zoneEvent.OnVisit();
		}
	}

	public void OnLeaveZone()
	{
		foreach (ZoneEvent zoneEvent in this.list)
		{
			zoneEvent.OnLeaveZone();
		}
	}

	public void OnSimulateHour()
	{
		if (this.list.Count == 0)
		{
			return;
		}
		foreach (ZoneEvent zoneEvent in this.list.Copy<ZoneEvent>())
		{
			zoneEvent.OnSimulateHour();
		}
	}

	public Zone zone;

	[JsonProperty]
	public List<ZoneEvent> list = new List<ZoneEvent>();

	[JsonProperty]
	public List<ZonePreEnterEvent> listPreEnter = new List<ZonePreEnterEvent>();
}
