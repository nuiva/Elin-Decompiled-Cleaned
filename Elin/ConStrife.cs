using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ConStrife : BaseBuff
{
	public override string TextDuration
	{
		get
		{
			return "Lv." + this.lv.ToString();
		}
	}

	public override bool WillOverride
	{
		get
		{
			return false;
		}
	}

	public int ExpToNext
	{
		get
		{
			return (this.lv + 1) * (this.lv + 1);
		}
	}

	public void AddKill()
	{
		this.exp++;
		if (this.exp >= this.ExpToNext)
		{
			this.exp = 0;
			this.lv++;
		}
		this.SetTurn();
	}

	public Dice GetDice()
	{
		return new Dice(1, 1 + this.lv * 2, 0, null);
	}

	public void SetTurn()
	{
		this.turn = Mathf.Max(100 - this.lv * 10, 10);
	}

	public override void Tick()
	{
		this.turn--;
		if (this.turn < 0)
		{
			this.lv--;
			if (this.lv >= 1)
			{
				this.SetTurn();
				this.exp = this.ExpToNext / 2;
				return;
			}
			base.Kill(false);
		}
	}

	public override void OnWriteNote(List<string> list)
	{
		list.Add("hintStrife".lang(this.lv.ToString() ?? "", this.exp.ToString() + "/" + this.ExpToNext.ToString(), this.GetDice().ToString(), null, null));
	}

	[JsonProperty]
	public int exp;

	[JsonProperty]
	public int lv;

	[JsonProperty]
	public int turn;
}
