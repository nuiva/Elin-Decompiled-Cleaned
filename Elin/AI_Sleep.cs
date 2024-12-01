public class AI_Sleep : AI_TargetThing
{
	public override bool GotoTarget => true;

	public override void OnProgressComplete()
	{
		if (!owner.CanSleep())
		{
			Msg.Say((EClass._zone.events.GetEvent<ZoneEventQuest>() != null) ? "badidea" : "notSleepy");
		}
		else
		{
			owner.Sleep(base.target);
		}
	}
}
