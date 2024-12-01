public class TraitPhone : TraitItem
{
	public ZoneEventPhone ev;

	public override void OnSimulateHour(VirtualDate date)
	{
		if (date.IsRealTime && owner.IsInstalled && EClass.rnd(10) == 0)
		{
			EClass._zone.events.Add(new ZoneEventPhone
			{
				uidPhone = owner.uid
			});
		}
	}

	public override bool CanUse(Chara c)
	{
		return ev != null;
	}

	public override bool OnUse(Chara c)
	{
		SE.Play("electricity_insufficient");
		Msg.SayNothingHappen();
		ev.Kill();
		return true;
	}
}
