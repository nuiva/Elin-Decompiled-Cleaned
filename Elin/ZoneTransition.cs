using Newtonsoft.Json;

public class ZoneTransition : EClass
{
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

	[JsonProperty]
	public int uidLastZone;

	[JsonProperty]
	public int x;

	[JsonProperty]
	public int z;

	[JsonProperty]
	public EnterState state;

	[JsonProperty]
	public string idTele;

	public float ratePos = -1f;

	public Zone lastZone => RefZone.Get(uidLastZone);

	public static EnterState DirToState(int dir)
	{
		return dir switch
		{
			0 => EnterState.Top, 
			1 => EnterState.Right, 
			3 => EnterState.Bottom, 
			_ => EnterState.Left, 
		};
	}
}
