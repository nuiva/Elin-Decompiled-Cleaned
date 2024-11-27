using System;
using UnityEngine;

public class QuestInstance : QuestRandom
{
	public virtual string IdZone
	{
		get
		{
			return "instance_arena";
		}
	}

	public override bool UseInstanceZone
	{
		get
		{
			return true;
		}
	}

	public override bool CanAbandon
	{
		get
		{
			return EClass._zone.instance == null;
		}
	}

	public override string RefDrama3
	{
		get
		{
			Zone clientZone = base.ClientZone;
			return ((clientZone != null) ? clientZone.Name : null) ?? "???";
		}
	}

	public override int KarmaOnFail
	{
		get
		{
			return -1;
		}
	}

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
		ZoneEventQuest zoneEventQuest = this.CreateEvent();
		zoneEventQuest.uidQuest = this.uid;
		ZoneInstanceRandomQuest zoneInstanceRandomQuest = this.CreateInstance();
		zoneInstanceRandomQuest.uidClient = c.uid;
		zoneInstanceRandomQuest.uidQuest = this.uid;
		Zone zone = SpatialGen.CreateInstance(this.IdZone, zoneInstanceRandomQuest);
		this.deadline = 0;
		zone.events.Add(zoneEventQuest, false);
		string[] array = new string[6];
		array[0] = "Creating QuestInstance:";
		array[1] = ((this != null) ? this.ToString() : null);
		array[2] = "/";
		int num = 3;
		Quest quest = c.quest;
		array[num] = ((quest != null) ? quest.ToString() : null);
		array[4] = "/";
		array[5] = (c.quest == this).ToString();
		Debug.Log(string.Concat(array));
		return zone;
	}
}
