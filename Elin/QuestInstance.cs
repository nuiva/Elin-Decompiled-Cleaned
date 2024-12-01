using UnityEngine;

public class QuestInstance : QuestRandom
{
	public virtual string IdZone => "instance_arena";

	public override bool UseInstanceZone => true;

	public override bool CanAbandon => EClass._zone.instance == null;

	public override string RefDrama3 => base.ClientZone?.Name ?? "???";

	public override int KarmaOnFail => -1;

	public virtual ZoneEventQuest CreateEvent()
	{
		return new ZoneEventQuest();
	}

	public virtual ZoneInstanceRandomQuest CreateInstance()
	{
		return new ZoneInstanceRandomQuest();
	}

	public override Zone CreateInstanceZone(Chara c)
	{
		ZoneEventQuest zoneEventQuest = CreateEvent();
		zoneEventQuest.uidQuest = uid;
		ZoneInstanceRandomQuest zoneInstanceRandomQuest = CreateInstance();
		zoneInstanceRandomQuest.uidClient = c.uid;
		zoneInstanceRandomQuest.uidQuest = uid;
		Zone zone = SpatialGen.CreateInstance(IdZone, zoneInstanceRandomQuest);
		deadline = 0;
		zone.events.Add(zoneEventQuest);
		Debug.Log("Creating QuestInstance:" + this?.ToString() + "/" + c.quest?.ToString() + "/" + (c.quest == this));
		return zone;
	}
}
