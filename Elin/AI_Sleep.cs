using System;

public class AI_Sleep : AI_TargetThing
{
	public override bool GotoTarget
	{
		get
		{
			return true;
		}
	}

	public override void OnProgressComplete()
	{
		if (!this.owner.CanSleep())
		{
			Msg.Say((EClass._zone.events.GetEvent<ZoneEventQuest>() != null) ? "badidea" : "notSleepy");
			return;
		}
		this.owner.Sleep(base.target, null, false, null, null);
	}
}
