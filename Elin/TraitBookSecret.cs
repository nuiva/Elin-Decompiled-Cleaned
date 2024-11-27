using System;
using UnityEngine;

public class TraitBookSecret : TraitScroll
{
	public override bool CanRead(Chara c)
	{
		return !c.isBlind;
	}

	public override int GetActDuration(Chara c)
	{
		return 5;
	}

	public override void OnRead(Chara c)
	{
		if (c.IsPC && EClass.player.stats.kumi >= 5)
		{
			c.Say("book_secret2", c, null, null);
			return;
		}
		c.Say("book_secret", c, null, null);
		c.Say("dingExp", c, null, null);
		c.feat += (c.IsPC ? 1 : 3);
		if (c.IsPC)
		{
			EClass.player.stats.kumi++;
		}
		c.PlaySound("godbless", 1f, true);
		c.PlayEffect("aura_heaven", true, 0f, default(Vector3));
		c.Say("spellbookCrumble", this.owner.Duplicate(1), null, null);
		this.owner.ModNum(-1, true);
	}
}
