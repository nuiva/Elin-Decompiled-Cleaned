using Newtonsoft.Json;

public class ZoneInstance : EClass
{
	public enum Status
	{
		Running,
		Fail,
		Success
	}

	[JsonProperty]
	public int uidZone;

	[JsonProperty]
	public int x;

	[JsonProperty]
	public int z;

	[JsonProperty]
	public Status status;

	public Zone ReturnZone => EClass.game.spatials.Find(uidZone);

	public virtual ZoneTransition.EnterState ReturnState => ZoneTransition.EnterState.PortalReturn;

	public virtual bool WarnExit => false;

	public virtual bool ShowEnemyOnMinimap => false;

	public virtual void OnGenerateMap()
	{
	}

	public virtual void OnLeaveZone()
	{
	}
}
