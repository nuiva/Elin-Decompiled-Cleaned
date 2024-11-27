using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class HotItemAct : HotItemGameAction
{
	public override Act act
	{
		get
		{
			Act result;
			if ((result = this._act) == null)
			{
				result = (this._act = ACT.Create(this.id));
			}
			return result;
		}
	}

	public override string Name
	{
		get
		{
			return this.source.GetName();
		}
	}

	public SourceElement.Row source
	{
		get
		{
			return EClass.sources.elements.map.TryGetValue(this.id, null) ?? EClass.sources.elements.rows[0];
		}
	}

	public override Sprite GetSprite()
	{
		return this.act.GetSprite();
	}

	public override bool UseUIObjMaterial
	{
		get
		{
			return true;
		}
	}

	public override Color SpriteColor
	{
		get
		{
			if (!this.source.aliasRef.IsEmpty())
			{
				return EClass.setting.elements[this.source.aliasRef].colorSprite;
			}
			return Color.white;
		}
	}

	public HotItemAct()
	{
	}

	public HotItemAct(string alias)
	{
		this.id = EClass.sources.elements.alias[alias].id;
	}

	public HotItemAct(SourceElement.Row row)
	{
		this.id = row.id;
	}

	public override void SetImage(Image icon)
	{
		this.act.SetImage(icon);
	}

	public override bool TrySetAct(ActPlan p)
	{
		Card tg = null;
		p.pos.ListVisibleCards().ForeachReverse(delegate(Card a)
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
			return false;
		}
		p.TrySetAct(this.source.GetName(), delegate()
		{
			Card tg;
			if (tg != null && !tg.ExistsOnMap)
			{
				return false;
			}
			Point point = (tg != null && tg.ExistsOnMap) ? tg.pos : p.pos;
			if (!this.act.CanPerform(EClass.pc, tg, point))
			{
				return false;
			}
			string[] array = new string[8];
			array[0] = "hotitemact:";
			int num = 1;
			HotItemAct <>4__this = this;
			array[num] = ((<>4__this != null) ? <>4__this.ToString() : null);
			array[2] = "/";
			array[3] = this.act.id.ToString();
			array[4] = "/";
			int num2 = 5;
			tg = tg;
			array[num2] = ((tg != null) ? tg.ToString() : null);
			array[6] = "/";
			array[7] = p.performed.ToString();
			Debug.Log(string.Concat(array));
			if (p.performed && this.act.CanPressRepeat)
			{
				return EClass.pc.UseAbility(this.act.source.alias, tg, point, false);
			}
			return ButtonAbility.TryUse(this.act, tg, point, null, true, true);
		}, tg, null, -1, EClass._zone.IsCrime(EClass.pc, this.act), this.act.LocalAct, this.act.CanPressRepeat);
		return true;
	}

	[JsonProperty]
	public int id;

	public Act _act;
}
