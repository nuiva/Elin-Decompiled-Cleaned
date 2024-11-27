using System;
using UnityEngine;

public class Check : EClass
{
	public static Check Get(string id, float dcMod = 1f)
	{
		return new Check
		{
			id = id,
			dcMod = dcMod
		};
	}

	public SourceCheck.Row source
	{
		get
		{
			SourceCheck.Row result;
			if ((result = this._source) == null)
			{
				result = (this._source = EClass.sources.checks.map[this.id]);
			}
			return result;
		}
	}

	public string mainElementName
	{
		get
		{
			return EClass.sources.elements.map[this.source.element].GetName();
		}
	}

	public int baseDC
	{
		get
		{
			return this.source.baseDC;
		}
	}

	public string GetText(Chara p, Card tg, bool inDialog = false)
	{
		string text = "";
		int finalDC = this.GetFinalDC(p, tg);
		string text2 = (finalDC > 20) ? "絶望的" : ((finalDC > 17) ? "ほぼ絶望的" : ((finalDC > 14) ? "とても難しい" : ((finalDC > 11) ? "難しい" : ((finalDC > 8) ? "普通" : ((finalDC > 5) ? "簡単" : ((finalDC > 2) ? "とても簡単" : "とても余裕"))))));
		if (finalDC > 20 || finalDC > 15 || finalDC <= 10)
		{
		}
		text = string.Concat(new string[]
		{
			text,
			"DC ",
			this.GetDC(tg).ToString(),
			" ",
			this.mainElementName,
			" ",
			text2
		});
		return string.Concat(new string[]
		{
			text,
			" ",
			this.source.element.ToString(),
			"/",
			EClass.sources.elements.map[this.source.element].alias
		});
	}

	public int GetDC(Card tg = null)
	{
		float num = (float)this.baseDC * this.dcMod;
		if (tg == null)
		{
			return (int)num;
		}
		num += this.source.lvMod * (float)tg.LV;
		if (this.source.targetElement != 0)
		{
			Element element = tg.elements.GetElement(this.source.element);
			if (element != null)
			{
				num += (float)element.Value;
				if (this.source.targetSubFactor > 0f && !element.source.aliasParent.IsEmpty())
				{
					Element element2 = tg.elements.GetElement(element.source.aliasParent);
					if (element2 != null)
					{
						num += this.source.targetSubFactor * (float)element2.Value;
					}
				}
			}
		}
		return (int)num;
	}

	public int GetFinalDC(Chara p, Card tg = null)
	{
		int num = this.GetDC(tg);
		if (this.source.element != 0)
		{
			Element element = p.elements.GetElement(this.source.element);
			if (element != null)
			{
				num -= element.Value;
				if (this.source.subFactor > 0f && !element.source.aliasParent.IsEmpty())
				{
					Element element2 = p.elements.GetElement(element.source.aliasParent);
					if (element2 != null)
					{
						num -= (int)(this.source.subFactor * (float)element2.Value);
					}
				}
			}
		}
		if (num > 14)
		{
			num = 14 + (int)Mathf.Sqrt((float)(num - 14));
		}
		else if (num < 6)
		{
			num = 6 - (int)Mathf.Sqrt((float)(6 - num));
		}
		return num;
	}

	public Check.Result Perform(Chara p, Card tg = null)
	{
		int finalDC = this.GetFinalDC(p, tg);
		int num = Dice.Roll(1, 20, 0, null);
		if (EClass.debug.logDice)
		{
			string text = string.Concat(new string[]
			{
				"Check:",
				this.source.id,
				" dc=",
				finalDC.ToString(),
				" roll=",
				num.ToString(),
				"  "
			});
			if (num == 20)
			{
				text += "CriticalPass";
			}
			else if (num == 1)
			{
				text += "CriticalFail";
			}
			else if (num >= finalDC)
			{
				text += "Pass";
			}
			else
			{
				text += "Fail";
			}
			Debug.Log(text);
		}
		if (num == 20)
		{
			return Check.Result.CriticalPass;
		}
		if (num == 1)
		{
			return Check.Result.CriticalFail;
		}
		if (num >= finalDC)
		{
			return Check.Result.Pass;
		}
		return Check.Result.Fail;
	}

	public Check.Result Perform(Chara p, Card tg, Action<Check.Result> action)
	{
		Check.Result result = this.Perform(p, tg);
		if (action != null)
		{
			action(result);
		}
		return result;
	}

	public SourceCheck.Row _source;

	public string id;

	public float dcMod;

	public enum Result
	{
		CriticalPass,
		Pass,
		Fail,
		CriticalFail
	}

	public enum Output
	{
		None,
		Default
	}
}
