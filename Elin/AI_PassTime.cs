using System;
using System.Collections.Generic;

public class AI_PassTime : AIAct
{
	public virtual AI_PassTime.Type type
	{
		get
		{
			return AI_PassTime.Type.passTime;
		}
	}

	public virtual int exp
	{
		get
		{
			return 0;
		}
	}

	public virtual int turns
	{
		get
		{
			return 3000;
		}
	}

	public override bool IsAutoTurn
	{
		get
		{
			return true;
		}
	}

	public override bool CanManualCancel()
	{
		return true;
	}

	public override bool LocalAct
	{
		get
		{
			return this.type == AI_PassTime.Type.selfHarm;
		}
	}

	public override TargetType TargetType
	{
		get
		{
			if (this.type != AI_PassTime.Type.passTime)
			{
				return TargetType.Self;
			}
			return TargetType.Any;
		}
	}

	public override MultiSprite GetStateIcon()
	{
		AI_PassTime.Type type = this.type;
		if (type == AI_PassTime.Type.meditate)
		{
			return EClass.core.refs.stateIcons.meditation;
		}
		if (type != AI_PassTime.Type.selfHarm)
		{
			return base.GetStateIcon();
		}
		return EClass.core.refs.stateIcons.selfharm;
	}

	public bool IsFull
	{
		get
		{
			return this.owner == null || (this.owner.hp == this.owner.MaxHP && this.owner.mana.value == this.owner.mana.max);
		}
	}

	public override void OnStart()
	{
		this.startedFull = this.IsFull;
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		if (this.target != null)
		{
			yield return base.DoGoto(this.target.pos, 0, false, null);
		}
		this.owner.Say(this.type.ToString() + "_start", this.owner, null, null);
		int num2;
		for (int i = 0; i < this.turns; i = num2 + 1)
		{
			if (this.exp > 0 && i % 5 == 0)
			{
				this.owner.ModExp(base.GetType().Name, this.exp);
			}
			if (this.type == AI_PassTime.Type.selfHarm)
			{
				if (EClass.rnd(10) == 0)
				{
					this.owner.AddCondition<ConBleed>(50, false);
				}
				else if (EClass.rnd(10) == 0)
				{
					this.owner.DamageHP(5 + EClass.rnd(5), AttackSource.None, null);
					if (this.owner != null)
					{
						this.owner.Teleport(ActEffect.GetTeleportPos(EClass.pc.pos, 6), false, false);
					}
				}
			}
			else
			{
				foreach (Chara chara in EClass.pc.party.members)
				{
					bool flag = false;
					using (List<Condition>.Enumerator enumerator2 = chara.conditions.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.PreventRegen)
							{
								flag = true;
							}
						}
					}
					if (!flag)
					{
						int num = 1 + EClass.pc.Evalue(6003) / 5;
						if (EClass.rnd(3) == 0)
						{
							chara.HealHP(num * (chara.IsPC ? 1 : 2), HealSource.None);
						}
						chara.mana.Mod(num * (chara.IsPC ? 1 : 2));
					}
				}
				if (i == 50 && EClass.pc.pos.IsHotSpring && (!EClass.pc.IsPCC || EClass.pc.pccData.state == PCCState.Undie))
				{
					int p = EClass._zone.elements.Has(3701) ? 150 : 100;
					foreach (Chara chara2 in EClass.pc.party.members)
					{
						Condition condition = chara2.AddCondition<ConHotspring>(p, false);
						if (condition != null)
						{
							condition.SetPerfume(3);
						}
					}
				}
				if (!this.startedFull && this.IsFull)
				{
					yield return base.Success(null);
				}
				if (!EClass.debug.enable && this.owner.CanSleep() && EClass.rnd(10) == 0)
				{
					this.owner.Sleep(null, null, false, null, null);
					yield return base.Success(null);
				}
			}
			yield return base.KeepRunning();
			num2 = i;
		}
		if (this.owner != null)
		{
			this.owner.Say(this.type.ToString() + "_end", this.owner, null, null);
		}
		yield break;
	}

	public override void OnCancel()
	{
		if (this.owner != null)
		{
			this.owner.Say(this.type.ToString() + "_end", this.owner, null, null);
		}
	}

	public Thing target;

	public bool startedFull;

	public enum Type
	{
		passTime,
		meditate,
		selfHarm
	}
}
