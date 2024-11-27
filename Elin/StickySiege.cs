using System;

public class StickySiege : BaseSticky
{
	public override bool ShouldShow
	{
		get
		{
			return EClass._zone.events.GetEvent<ZoneEventSiege>() != null;
		}
	}
}
