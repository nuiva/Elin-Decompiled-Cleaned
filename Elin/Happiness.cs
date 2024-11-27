using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Happiness : EClass
{
	public string Name
	{
		get
		{
			return ("member" + this.type.ToString()).lang();
		}
	}

	public void SetOwner(FactionBranch _branch, FactionMemberType _type)
	{
		this.branch = _branch;
		this.type = _type;
	}

	public void OnAdvanceDay()
	{
		this.lastValue = this.value;
		this.value = (int)Mathf.Lerp((float)this.value, (float)this.GetTargetValue(), 0.1f);
	}

	public void OnRefreshEffect()
	{
		this.modPolicy = 0;
	}

	public int GetTargetValue()
	{
		int num = 50;
		if (this.branch.resources.food.value < 0)
		{
			num -= 100;
		}
		num += (this.branch.resources.fun.value - 50) / 4;
		num += (this.branch.resources.culture.value - 50) / 4;
		num += (this.branch.resources.medicine.value - 50) / 4;
		num += (this.branch.resources.safety.value - 50) / 4;
		foreach (Chara chara in this.branch.members)
		{
		}
		foreach (BaseHomeResource baseHomeResource in this.branch.resources.list)
		{
		}
		foreach (Policy policy in this.branch.policies.list)
		{
		}
		num += this.modPolicy;
		return num;
	}

	public string GetText()
	{
		if (this.CountMembers() == 0)
		{
			return " - %".TagColor(FontColor.Passive, null);
		}
		string text = this.value.ToString() + "%";
		int num = this.value - this.lastValue;
		if (num != 0)
		{
		}
		if (this.value <= 30)
		{
			text = text.TagColor(FontColor.Bad, null);
		}
		else if (this.value >= 70)
		{
			text = text.TagColor(FontColor.Good, null);
		}
		string str = "";
		if (num > 0)
		{
			str = "↑".TagColor(FontColor.Good, null);
		}
		else if (num < 0)
		{
			str = "↓".TagColor(FontColor.Bad, null);
		}
		return text + " " + str;
	}

	public int CountMembers()
	{
		int num = 0;
		using (List<Chara>.Enumerator enumerator = this.branch.members.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.memberType == this.type)
				{
					num++;
				}
			}
		}
		return num;
	}

	public void WriteNote(UINote n)
	{
		n.Clear();
		n.AddHeader(this.Name, null);
		n.Space(0, 1);
		n.AddText("vTarget".lang() + this.GetTargetValue().ToString(), FontColor.DontChange);
		n.AddText("vCurrent".lang() + this.value.ToString(), FontColor.DontChange);
		n.AddText("vLast".lang() + this.lastValue.ToString(), FontColor.DontChange);
		n.Space(0, 1);
		n.AddText("policyHappiness".lang(this.modPolicy.ToString() ?? "", null, null, null, null), FontColor.DontChange);
		n.Build();
	}

	[JsonProperty]
	public int value;

	[JsonProperty]
	public int lastValue;

	public int modPolicy;

	public FactionBranch branch;

	public FactionMemberType type;
}
