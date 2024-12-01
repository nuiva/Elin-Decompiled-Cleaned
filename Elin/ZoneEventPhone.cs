using Newtonsoft.Json;

public class ZoneEventPhone : ZoneEvent
{
	[JsonProperty]
	public int uidPhone;

	[JsonProperty]
	public int ring;

	public override int hoursToKill => 24;

	public TraitPhone Phone => EClass._map.things.Find((Thing a) => a.uid == uidPhone)?.trait as TraitPhone;

	public override void OnInit()
	{
		ring = EClass.rnd(4) + 3;
	}

	public override void OnTickRound()
	{
		TraitPhone phone = Phone;
		if (phone == null)
		{
			Kill();
			return;
		}
		ring--;
		phone.owner.TalkRaw(Lang.Game.Get("phone_boss"));
		phone.owner.PlayAnime(AnimeID.Shiver);
		phone.ev = this;
		if (ring < 0)
		{
			Kill();
		}
	}

	public override void OnKill()
	{
		if (Phone != null)
		{
			Phone.ev = null;
		}
	}
}
