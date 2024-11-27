using System;
using UnityEngine;

public class TraitShackle : Trait
{
	public virtual Vector3 GetRestrainPos
	{
		get
		{
			return EClass.setting.render.posShackle;
		}
	}

	public virtual AnimeID animeId
	{
		get
		{
			return AnimeID.Shiver;
		}
	}

	public override bool CanStackTo(Thing to)
	{
		return false;
	}

	public override bool CanBeHeld
	{
		get
		{
			return !this.IsRestraining();
		}
	}

	public override bool CanUse(Chara c)
	{
		return EClass._zone.IsPCFaction && this.owner.IsInstalled && !EClass.pc.isRestrained && (this.owner.pos.FirstChara == null || !this.owner.pos.FirstChara.isRestrained);
	}

	public override string LangUse
	{
		get
		{
			return "ActRestrain";
		}
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		p.TrySetAct(new ActRestrain
		{
			shackle = this
		}, p.pos.FirstChara);
	}

	public override bool OnUse(Chara c)
	{
		c.MoveImmediate(this.owner.pos, true, true);
		new ActRestrain
		{
			shackle = this
		}.Perform(c, c, c.pos);
		return true;
	}

	public void Restrain(Card tc, bool msg = false)
	{
		tc.isRestrained = true;
		this.owner.c_uidRefCard = tc.uid;
		if (msg)
		{
			tc.Say("restrained", tc, null, null);
		}
	}

	public bool IsRestraining()
	{
		if (!this.owner.IsInstalled || this.owner.pos == null || EClass.player.simulatingZone)
		{
			return false;
		}
		foreach (Chara chara in this.owner.pos.Charas)
		{
			if (chara.isRestrained && this.owner.c_uidRefCard == chara.uid)
			{
				return true;
			}
		}
		foreach (Chara chara2 in EClass._map.charas)
		{
			if (chara2.isChara && chara2.isRestrained && chara2.pos.Equals(this.owner.pos))
			{
				this.owner.c_uidRefCard = chara2.uid;
				return true;
			}
		}
		return false;
	}
}
