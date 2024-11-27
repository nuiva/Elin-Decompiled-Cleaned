using System;

public class QuestSubdue : QuestInstance
{
	public override string IdZone
	{
		get
		{
			return "instance_arena2";
		}
	}

	public override bool FameContent
	{
		get
		{
			return true;
		}
	}

	public override int BaseMoney
	{
		get
		{
			return base.source.money + EClass.curve(this.DangerLv, 20, 15, 75) * 10;
		}
	}

	public override ZoneEventQuest CreateEvent()
	{
		return new ZoneEventSubdue();
	}

	public override ZoneInstanceRandomQuest CreateInstance()
	{
		return new ZoneInstanceSubdue();
	}

	public override string GetTextProgress()
	{
		ZoneEventSubdue @event = EClass._zone.events.GetEvent<ZoneEventSubdue>();
		if (@event == null)
		{
			return "";
		}
		return "progressHunt".lang((@event.max - @event.enemies.Count).ToString() ?? "", @event.max.ToString() ?? "", null, null, null);
	}
}
