using System;

public class TraitPhone : TraitItem
{
	public override void OnSimulateHour(VirtualDate date)
	{
		if (!date.IsRealTime || !this.owner.IsInstalled)
		{
			return;
		}
		if (EClass.rnd(10) == 0)
		{
			EClass._zone.events.Add(new ZoneEventPhone
			{
				uidPhone = this.owner.uid
			}, false);
		}
	}

	public override bool CanUse(Chara c)
	{
		return this.ev != null;
	}

	public override bool OnUse(Chara c)
	{
		SE.Play("electricity_insufficient");
		Msg.SayNothingHappen();
		this.ev.Kill();
		return true;
	}

	public ZoneEventPhone ev;
}
