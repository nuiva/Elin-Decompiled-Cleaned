using System;
using Newtonsoft.Json;

public class ZoneTransition : EClass
{
	public Zone lastZone
	{
		get
		{
			return RefZone.Get(this.uidLastZone);
		}
	}

	public static ZoneTransition.EnterState DirToState(int dir)
	{
		if (dir == 0)
		{
			return ZoneTransition.EnterState.Top;
		}
		if (dir == 1)
		{
			return ZoneTransition.EnterState.Right;
		}
		if (dir == 3)
		{
			return ZoneTransition.EnterState.Bottom;
		}
		return ZoneTransition.EnterState.Left;
	}

	[JsonProperty]
	public int uidLastZone;

	[JsonProperty]
	public int x;

	[JsonProperty]
	public int z;

	[JsonProperty]
	public ZoneTransition.EnterState state;

	[JsonProperty]
	public string idTele;

	public float ratePos = -1f;

	public enum EnterState
	{
		Auto,
		Center,
		Encounter,
		Dir,
		Top,
		Right,
		Bottom,
		Left,
		Dead,
		Exact,
		PortalReturn,
		RandomVisit,
		Down,
		Up,
		Return,
		Teleport,
		Elevator,
		Region,
		UndergroundOrSky,
		Moongate,
		Fall
	}
}
