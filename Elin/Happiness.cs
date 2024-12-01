using Newtonsoft.Json;
using UnityEngine;

public class Happiness : EClass
{
	[JsonProperty]
	public int value;

	[JsonProperty]
	public int lastValue;

	public int modPolicy;

	public FactionBranch branch;

	public FactionMemberType type;

	public string Name => ("member" + type).lang();

	public void SetOwner(FactionBranch _branch, FactionMemberType _type)
	{
		branch = _branch;
		type = _type;
	}

	public void OnAdvanceDay()
	{
		lastValue = value;
		value = (int)Mathf.Lerp(value, GetTargetValue(), 0.1f);
	}

	public void OnRefreshEffect()
	{
		modPolicy = 0;
	}

	public int GetTargetValue()
	{
		int num = 50;
		if (branch.resources.food.value < 0)
		{
			num -= 100;
		}
		num += (branch.resources.fun.value - 50) / 4;
		num += (branch.resources.culture.value - 50) / 4;
		num += (branch.resources.medicine.value - 50) / 4;
		num += (branch.resources.safety.value - 50) / 4;
		foreach (Chara member in branch.members)
		{
			_ = member;
		}
		foreach (BaseHomeResource item in branch.resources.list)
		{
			_ = item;
		}
		foreach (Policy item2 in branch.policies.list)
		{
			_ = item2;
		}
		return num + modPolicy;
	}

	public string GetText()
	{
		if (CountMembers() == 0)
		{
			return " - %".TagColor(FontColor.Passive);
		}
		string text = value + "%";
		int num = value - lastValue;
		if (num != 0)
		{
			_ = 0;
		}
		if (value <= 30)
		{
			text = text.TagColor(FontColor.Bad);
		}
		else if (value >= 70)
		{
			text = text.TagColor(FontColor.Good);
		}
		string text2 = "";
		if (num > 0)
		{
			text2 = "↑".TagColor(FontColor.Good);
		}
		else if (num < 0)
		{
			text2 = "↓".TagColor(FontColor.Bad);
		}
		return text + " " + text2;
	}

	public int CountMembers()
	{
		int num = 0;
		foreach (Chara member in branch.members)
		{
			if (member.memberType == type)
			{
				num++;
			}
		}
		return num;
	}

	public void WriteNote(UINote n)
	{
		n.Clear();
		n.AddHeader(Name);
		n.Space();
		n.AddText("vTarget".lang() + GetTargetValue());
		n.AddText("vCurrent".lang() + value);
		n.AddText("vLast".lang() + lastValue);
		n.Space();
		n.AddText("policyHappiness".lang(modPolicy.ToString() ?? ""));
		n.Build();
	}
}
