public class StickySiege : BaseSticky
{
	public override bool ShouldShow => EClass._zone.events.GetEvent<ZoneEventSiege>() != null;
}
