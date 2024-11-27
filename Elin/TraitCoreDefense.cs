using System;

public class TraitCoreDefense : Trait
{
	public override bool CanBeDestroyed
	{
		get
		{
			return false;
		}
	}

	public override bool CanOnlyCarry
	{
		get
		{
			return true;
		}
	}

	public override bool CanPutAway
	{
		get
		{
			return false;
		}
	}

	public override bool IsLightOn
	{
		get
		{
			return true;
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		ZoneEventDefenseGame ev = EClass._zone.events.GetEvent<ZoneEventDefenseGame>();
		if (ev == null)
		{
			return;
		}
		if (ev.wave % 5 != 0 && !ev.retreated)
		{
			p.TrySetAct("actWarhorn", delegate()
			{
				ev.Horn_Next();
				return true;
			}, null, 1);
		}
		if (ev.CanRetreat && !ev.retreated)
		{
			p.TrySetAct("actEvacDefense", delegate()
			{
				ev.Horn_Retreat();
				return true;
			}, null, 1);
		}
		if (ev.CanCallAlly)
		{
			p.TrySetAct("actCallAlly", delegate()
			{
				ev.Horn_Ally();
				return true;
			}, null, 1);
		}
	}
}
