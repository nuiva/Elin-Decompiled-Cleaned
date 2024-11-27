using System;

public class TraitCatalyst : TraitTool
{
	public Act act
	{
		get
		{
			Act result;
			if ((result = this._act) == null)
			{
				result = (this._act = this.CreateAct());
			}
			return result;
		}
	}

	public virtual bool IsRod
	{
		get
		{
			return true;
		}
	}

	public virtual Act CreateAct()
	{
		return ACT.Create(this.owner.Thing.sourceCard.vals[0].IsEmpty("AI_SelfHarm"));
	}

	public override bool CanUse(Chara c)
	{
		return (!this.act.LocalAct || !EClass._zone.IsRegion) && this.act.TargetType.CanSelectSelf;
	}

	public override bool OnUse(Chara c)
	{
		return ButtonAbility.TryUse(this.act, null, null, this.owner, true, true);
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		Card tg = null;
		p.pos.ListCards(false).ForeachReverse(delegate(Card a)
		{
			if (EClass.pc.CanSee(a) && !a.HasHost && this.act.IsValidTC(a))
			{
				tg = a;
				return true;
			}
			return false;
		});
		if (!this.act.CanPerform(EClass.pc, tg, p.pos))
		{
			return;
		}
		p.TrySetAct(this.act.source.GetName(), delegate()
		{
			if (tg != null && !tg.ExistsOnMap)
			{
				return false;
			}
			Point point = (tg != null && tg.ExistsOnMap) ? tg.pos : p.pos;
			if (!this.act.CanPerform(EClass.pc, tg, point))
			{
				return false;
			}
			if (p.performed && this.act.CanPressRepeat)
			{
				return EClass.pc.UseAbility(this.act.source.alias, tg, point, false);
			}
			return ButtonAbility.TryUse(this.act, tg, point, this.owner, true, true);
		}, tg, null, -1, EClass._zone.IsCrime(EClass.pc, this.act), this.act.LocalAct, this.act.CanPressRepeat);
	}

	private Act _act;
}
