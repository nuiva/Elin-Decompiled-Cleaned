using System;

public class TraitGamble : TraitItem
{
	public virtual string idMsg
	{
		get
		{
			return null;
		}
	}

	public virtual string idSound
	{
		get
		{
			return this.idMsg;
		}
	}

	public virtual string idTalk
	{
		get
		{
			return null;
		}
	}

	public override bool IdleUse(Chara c, int dist)
	{
		if (dist > 1)
		{
			return false;
		}
		if (!this.CanUse(c))
		{
			return false;
		}
		if (!this.idSound.IsEmpty())
		{
			c.PlaySound(this.idSound, 1f, true);
		}
		if (!this.idTalk.IsEmpty())
		{
			EClass.player.forceTalk = true;
			c.Talk(this.idTalk, null, null, false);
		}
		if (!this.idMsg.IsEmpty())
		{
			c.Say(this.idMsg, c, this.owner, null, null);
		}
		bool flag = EClass.rnd(2) == 0;
		if (!c.IsPC)
		{
			int a = (1 + EClass.rnd(10)) * (flag ? 1 : -1);
			this.owner.ModCurrency(a, "money");
			if (flag && EClass.rnd(20) == 0)
			{
				this.owner.ModCurrency(1, "casino_coin");
			}
		}
		if (flag)
		{
			this.owner.ShowEmo(Emo.happy, 0f, true);
		}
		return true;
	}

	public override bool CanUse(Chara c)
	{
		return this.owner.IsInstalled && (this.Electricity <= 0 || this.owner.isOn);
	}

	public override bool OnUse(Chara c)
	{
		return this.IdleUse(c, 0);
	}
}
