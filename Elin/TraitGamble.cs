public class TraitGamble : TraitItem
{
	public virtual string idMsg => null;

	public virtual string idSound => idMsg;

	public virtual string idTalk => null;

	public override bool IdleUse(Chara c, int dist)
	{
		if (dist > 1)
		{
			return false;
		}
		if (!CanUse(c))
		{
			return false;
		}
		if (!idSound.IsEmpty())
		{
			c.PlaySound(idSound);
		}
		if (!idTalk.IsEmpty())
		{
			EClass.player.forceTalk = true;
			c.Talk(idTalk);
		}
		if (!idMsg.IsEmpty())
		{
			c.Say(idMsg, c, owner);
		}
		bool flag = EClass.rnd(2) == 0;
		if (!c.IsPC)
		{
			int a = (1 + EClass.rnd(10)) * (flag ? 1 : (-1));
			owner.ModCurrency(a);
			if (flag && EClass.rnd(20) == 0)
			{
				owner.ModCurrency(1, "casino_coin");
			}
		}
		if (flag)
		{
			owner.ShowEmo(Emo.happy);
		}
		return true;
	}

	public override bool CanUse(Chara c)
	{
		if (owner.IsInstalled)
		{
			if (Electricity <= 0)
			{
				return true;
			}
			return owner.isOn;
		}
		return false;
	}

	public override bool OnUse(Chara c)
	{
		return IdleUse(c, 0);
	}
}
