using System;
using Newtonsoft.Json;

public class ZoneInstance : EClass
{
	public Zone ReturnZone
	{
		get
		{
			return EClass.game.spatials.Find(this.uidZone);
		}
	}

	public virtual ZoneTransition.EnterState ReturnState
	{
		get
		{
			return ZoneTransition.EnterState.PortalReturn;
		}
	}

	public virtual bool WarnExit
	{
		get
		{
			return false;
		}
	}

	public virtual void OnGenerateMap()
	{
	}

	public virtual void OnLeaveZone()
	{
	}

	[JsonProperty]
	public int uidZone;

	[JsonProperty]
	public int x;

	[JsonProperty]
	public int z;

	[JsonProperty]
	public ZoneInstance.Status status;

	public enum Status
	{
		Running,
		Fail,
		Success
	}
}
