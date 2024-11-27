using System;
using Newtonsoft.Json;

public class ZoneEventPhone : ZoneEvent
{
	public override int hoursToKill
	{
		get
		{
			return 24;
		}
	}

	public TraitPhone Phone
	{
		get
		{
			Thing thing = EClass._map.things.Find((Thing a) => a.uid == this.uidPhone);
			return ((thing != null) ? thing.trait : null) as TraitPhone;
		}
	}

	public override void OnInit()
	{
		this.ring = EClass.rnd(4) + 3;
	}

	public override void OnTickRound()
	{
		TraitPhone phone = this.Phone;
		if (phone == null)
		{
			base.Kill();
			return;
		}
		this.ring--;
		phone.owner.TalkRaw(Lang.Game.Get("phone_boss"), null, null, false);
		phone.owner.PlayAnime(AnimeID.Shiver, false);
		phone.ev = this;
		if (this.ring < 0)
		{
			base.Kill();
		}
	}

	public override void OnKill()
	{
		if (this.Phone != null)
		{
			this.Phone.ev = null;
		}
	}

	[JsonProperty]
	public int uidPhone;

	[JsonProperty]
	public int ring;
}
