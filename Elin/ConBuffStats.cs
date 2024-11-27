using System;
using System.Collections.Generic;
using UnityEngine;

public class ConBuffStats : Condition
{
	public override string Name
	{
		get
		{
			return (this.isDebuff ? "debuff" : "buff").lang();
		}
	}

	public override ConditionType Type
	{
		get
		{
			if (!this.isDebuff)
			{
				return ConditionType.Buff;
			}
			return ConditionType.Debuff;
		}
	}

	public bool isDebuff
	{
		get
		{
			return base.refVal2 == 221;
		}
	}

	public override int EvaluateTurn(int p)
	{
		if (base.refVal2 == 266)
		{
			return 7;
		}
		return base.EvaluateTurn(p) * ((base.refVal == 79) ? 50 : 100) / 100;
	}

	public override bool WillOverride
	{
		get
		{
			return true;
		}
	}

	public override bool AllowMultipleInstance
	{
		get
		{
			return true;
		}
	}

	public override int GetPhase()
	{
		return 0;
	}

	public override Color GetColor(SkinColorProfile c)
	{
		if (!this.isDebuff)
		{
			return c.textGood;
		}
		return c.textBad;
	}

	public override string GetPhaseStr()
	{
		return this.GetText();
	}

	public override string GetText()
	{
		string[] list = Lang.GetList("buff_" + EClass.sources.elements.map[base.refVal].alias);
		if (list == null)
		{
			return base.GetText();
		}
		if (!this.isDebuff)
		{
			return list[0];
		}
		return list[1];
	}

	public override void PlayEffect()
	{
		if (Condition.ignoreEffect)
		{
			return;
		}
		this.owner.PlaySound(this.isDebuff ? "debuff" : "buff", 1f, true);
		this.owner.PlayEffect(this.isDebuff ? "debuff" : "buff", true, 0f, default(Vector3));
		this.owner.Say(this.isDebuff ? "buffStats_curse" : "buffStats", this.owner, EClass.sources.elements.map[base.refVal].GetName().ToLower(), null);
	}

	public override bool CanStack(Condition c)
	{
		ConBuffStats conBuffStats = c as ConBuffStats;
		return conBuffStats == null || conBuffStats.refVal == base.refVal;
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner, false);
		this.elements = new ElementContainer();
		this.elements.SetBase(base.refVal, this.CalcValue() * (this.isDebuff ? -1 : 1), 0);
		this.elements.SetParent(this.owner);
	}

	public int CalcValue()
	{
		if (base.refVal2 == 266)
		{
			return 100 + (int)Mathf.Sqrt((float)base.power) * 2;
		}
		if (base.refVal == 79)
		{
			return (int)Mathf.Max(5f, Mathf.Sqrt((float)base.power) * 1.5f + 20f);
		}
		return (int)Mathf.Max(5f, Mathf.Sqrt((float)base.power) * 2f - 15f);
	}

	public override void OnWriteNote(List<string> list)
	{
		list.Add((this.isDebuff ? "hintDebuffStats" : "hintBuffStats").lang(base.sourceElement.GetName(), this.CalcValue().ToString() ?? "", null, null, null));
	}
}
