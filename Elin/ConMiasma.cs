using System;
using System.Collections.Generic;
using UnityEngine;

public class ConMiasma : TimeDebuff
{
	public override bool IsElemental
	{
		get
		{
			return true;
		}
	}

	public override void Tick()
	{
		Dice dice = Dice.Create("miasma_", base.power, null, null);
		try
		{
			this.owner.DamageHP(dice.Roll(), base.refVal, EClass.rnd(base.power / 2) + base.power / 4, AttackSource.Condition, null, true);
			if (this.owner.IsAliveInCurrentZone && base.value > 1)
			{
				for (int i = 0; i < 6; i++)
				{
					foreach (Chara chara in this.owner.pos.GetRandomPoint(2, true, true, false, 100).Charas)
					{
						if ((!this.owner.IsPCFaction || chara.IsPCFaction) && (this.owner.IsPCFaction || !chara.IsPCFaction) && chara.IsFriendOrAbove(this.owner) && !chara.HasCondition<ConMiasma>())
						{
							Condition condition = chara.AddCondition(Condition.Create<ConMiasma>(base.power / 2, delegate(ConMiasma con)
							{
								con.givenByPcParty = base.givenByPcParty;
								con.SetElement(base.refVal);
							}), false);
							if (condition != null)
							{
								condition.value = base.value - 1;
							}
							if (EClass.rnd(2) == 0)
							{
								break;
							}
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
			Debug.Log(this.owner);
			Debug.Log(base.refVal);
		}
		base.Mod(-1, false);
	}

	public override void OnWriteNote(List<string> list)
	{
		Dice dice = Dice.Create("miasma_", base.power, null, null);
		list.Add("hintDOT".lang(dice.ToString(), base.sourceElement.GetName().ToLower(), null, null, null));
	}
}
