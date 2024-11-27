using System;
using System.Collections.Generic;
using UnityEngine;

public class TraitTrap : TraitFloorSwitch
{
	public override byte WeightMod
	{
		get
		{
			if (!this.owner.isHidden)
			{
				return 10;
			}
			return 0;
		}
	}

	public virtual int DestroyChanceOnActivateTrap
	{
		get
		{
			return 50;
		}
	}

	public override bool CanBeHeld
	{
		get
		{
			return EClass._zone.IsPCFaction;
		}
	}

	public override bool StartHidden
	{
		get
		{
			return true;
		}
	}

	public override bool CanDisarmTrap
	{
		get
		{
			return !this.IsJammed;
		}
	}

	public override bool IsNegativeEffect
	{
		get
		{
			return true;
		}
	}

	public override bool IgnoreWhenLevitating()
	{
		if (this.owner.sourceCard.vals.Length == 0)
		{
			return false;
		}
		string text = this.owner.sourceCard.vals[0];
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 1426490280U)
		{
			if (num != 163485141U)
			{
				if (num != 274826164U)
				{
					if (num != 1426490280U)
					{
						return false;
					}
					if (!(text == "acid"))
					{
						return false;
					}
				}
				else if (!(text == "blind"))
				{
					return false;
				}
			}
			else if (!(text == "paralysis"))
			{
				return false;
			}
		}
		else if (num <= 2313861896U)
		{
			if (num != 2122968502U)
			{
				if (num != 2313861896U)
				{
					return false;
				}
				if (!(text == "sleep"))
				{
					return false;
				}
			}
			else if (!(text == "mine"))
			{
				return false;
			}
		}
		else if (num != 2574777438U)
		{
			if (num != 3649609552U)
			{
				return false;
			}
			if (!(text == "banana"))
			{
				return false;
			}
		}
		else if (!(text == "spear"))
		{
			return false;
		}
		return true;
	}

	public override void OnActivateTrap(Chara c)
	{
		if (this.owner.sourceCard.vals == null || this.owner.sourceCard.vals.Length == 0)
		{
			Debug.Log(this.owner.id);
			return;
		}
		string text = this.owner.sourceCard.vals[0];
		c.PlaySound("trap", 1f, true);
		c.Say("trap", c, this.owner, null, null);
		if (text == "mine")
		{
			Msg.Say("trap_mine", c, null, null, null);
		}
		else
		{
			c.Say("trap_" + text, c, null, null);
		}
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 2313861896U)
		{
			if (num <= 274826164U)
			{
				if (num != 163485141U)
				{
					if (num != 274826164U)
					{
						goto IL_3C8;
					}
					if (!(text == "blind"))
					{
						goto IL_3C8;
					}
					ActEffect.Proc(EffectId.Blind, c, null, 100, default(ActRef));
					goto IL_3C8;
				}
				else
				{
					if (!(text == "paralysis"))
					{
						goto IL_3C8;
					}
					ActEffect.Proc(EffectId.Paralyze, c, null, 100, default(ActRef));
					goto IL_3C8;
				}
			}
			else if (num != 1426490280U)
			{
				if (num != 2122968502U)
				{
					if (num != 2313861896U)
					{
						goto IL_3C8;
					}
					if (!(text == "sleep"))
					{
						goto IL_3C8;
					}
					ActEffect.Proc(EffectId.Sleep, c, null, 100, default(ActRef));
					goto IL_3C8;
				}
				else
				{
					if (!(text == "mine"))
					{
						goto IL_3C8;
					}
					c.PlayEffect("explosion", true, 0f, default(Vector3));
					c.PlaySound("explosion", 1f, true);
					int num2 = (EClass.debug.enable ? 100000 : 50) + base.TrapLv * 2 + EClass.rnd(150);
					if (this.IsLaidByDog)
					{
						num2 = num2 * 3 / 2;
					}
					c.DamageHP(num2, AttackSource.Trap, null);
					Shaker.ShakeCam("default", 1f);
					goto IL_3C8;
				}
			}
			else if (!(text == "acid"))
			{
				goto IL_3C8;
			}
		}
		else if (num <= 2574777438U)
		{
			if (num != 2369798645U)
			{
				if (num != 2574777438U)
				{
					goto IL_3C8;
				}
				if (!(text == "spear"))
				{
					goto IL_3C8;
				}
				if (c.IsLevitating)
				{
					c.Say("trap_spear_nullify", c, null, null);
					goto IL_3C8;
				}
				c.DamageHP(base.TrapLv * 2 + 10, AttackSource.Trap, null);
				goto IL_3C8;
			}
			else
			{
				if (!(text == "curse"))
				{
					goto IL_3C8;
				}
				ActEffect.Proc(EffectId.CurseEQ, c, null, 100, default(ActRef));
				goto IL_3C8;
			}
		}
		else if (num != 3289626814U)
		{
			if (num != 3649609552U)
			{
				if (num != 4268299041U)
				{
					goto IL_3C8;
				}
				if (!(text == "sister"))
				{
					goto IL_3C8;
				}
				using (List<Chara>.Enumerator enumerator = EClass._map.charas.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Chara chara = enumerator.Current;
						chara.RemoveCondition<ConSuspend>();
					}
					goto IL_3C8;
				}
			}
			else
			{
				if (!(text == "banana"))
				{
					goto IL_3C8;
				}
				c.Stumble(150);
				goto IL_3C8;
			}
		}
		else
		{
			if (!(text == "teleport"))
			{
				goto IL_3C8;
			}
			ActEffect.Proc(EffectId.Teleport, c, null, 100, default(ActRef));
			goto IL_3C8;
		}
		c.PlayEffect("Element/eleAcid", true, 0f, default(Vector3));
		ActEffect.Proc(EffectId.Acid, c, null, 100, default(ActRef));
		IL_3C8:
		if (this.DestroyChanceOnActivateTrap >= EClass.rnd(100))
		{
			this.owner.Destroy();
		}
	}

	public override void SetName(ref string s)
	{
		if (this.IsJammed)
		{
			s = "_jammed".lang(s, null, null, null, null);
		}
	}
}
