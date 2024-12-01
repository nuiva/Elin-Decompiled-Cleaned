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
			c.Say("book_secret2", c);
			return;
		}
		c.Say("book_secret", c);
		c.Say("dingExp", c);
		c.feat += (c.IsPC ? 1 : 3);
		if (c.IsPC)
		{
			EClass.player.stats.kumi++;
		}
		c.PlaySound("godbless");
		c.PlayEffect("aura_heaven");
		c.Say("spellbookCrumble", owner.Duplicate(1));
		owner.ModNum(-1);
	}
}
