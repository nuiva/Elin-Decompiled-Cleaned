using System;
using System.Collections.Generic;
using UnityEngine;

public class VirtualDate : Date
{
	public static bool IsActive
	{
		get
		{
			return VirtualDate.current != null;
		}
	}

	public VirtualDate(int elapsedHours = 0)
	{
		for (int i = 0; i < this.raw.Length; i++)
		{
			this.raw[i] = EClass.world.date.raw[i];
		}
		base.min = 0;
		while (elapsedHours > 0)
		{
			int num = base.hour;
			base.hour = num - 1;
			if (base.hour < 0)
			{
				base.hour = 23;
				num = base.day;
				base.day = num - 1;
				if (base.day <= 0)
				{
					base.day = 30;
					num = base.month;
					base.month = num - 1;
					if (base.month <= 0)
					{
						base.month = 12;
						num = base.year;
						base.year = num - 1;
					}
				}
			}
			elapsedHours--;
		}
	}

	public BranchMap GetBranchMap()
	{
		if (this.branchMap == null)
		{
			this.branchMap = new BranchMap();
			this.branchMap.Refresh();
		}
		return this.branchMap;
	}

	public void BuildSunMap()
	{
		Debug.Log("Building Sunmap");
		this.sunMap = new HashSet<int>();
		foreach (Trait trait in EClass._map.props.installed.traits.suns.Values)
		{
			foreach (Point point in trait.ListPoints(null, false))
			{
				this.sunMap.Add(point.index);
			}
		}
	}

	public void SimulateHour()
	{
		int hour = base.hour;
		base.hour = hour + 1;
		if (base.hour >= 24)
		{
			base.hour = 0;
			this.SimulateDay();
		}
		EClass._zone.OnSimulateHour(this);
	}

	public void SimulateDay()
	{
		int day = base.day;
		base.day = day + 1;
		if (base.day > 30)
		{
			base.day = 1;
			this.SimulateMonth();
		}
		EClass._zone.OnSimulateDay(this);
	}

	public void SimulateMonth()
	{
		int month = base.month;
		base.month = month + 1;
		if (base.month > 12)
		{
			base.month = 1;
			this.SimulateYear();
		}
		EClass._zone.OnSimulateMonth(this);
	}

	public void SimulateYear()
	{
		int year = base.year;
		base.year = year + 1;
	}

	public static Date current;

	public bool IsRealTime;

	public HashSet<int> sunMap;

	public BranchMap branchMap;
}
