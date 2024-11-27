using System;
using System.Collections.Generic;

public class HappinessSummary : EClass
{
	public HappinessSummary(FactionBranch b)
	{
		List<Chara> members = b.members;
		foreach (Chara chara in members)
		{
			this.happiness += chara.happiness;
			this.hunger += chara.hunger.value;
			this.fatigue += chara.stamina.value;
			this.depression += chara.depression.value;
			this.bladder += chara.bladder.value;
			this.hygine += chara.hygiene.value;
		}
		int count = members.Count;
		this.happiness /= count;
		this.hunger /= count;
		this.fatigue /= count;
		this.depression /= count;
		this.bladder /= count;
		this.hygine /= count;
	}

	public string GetText()
	{
		string text = "";
		text = string.Concat(new string[]
		{
			text,
			"happiness".lang(),
			": ",
			this.happiness.ToString(),
			"\n"
		});
		text = string.Concat(new string[]
		{
			text,
			EClass.pc.hunger.name,
			": ",
			this.hunger.ToString(),
			"\n"
		});
		text = string.Concat(new string[]
		{
			text,
			EClass.pc.stamina.name,
			": ",
			this.fatigue.ToString(),
			"\n"
		});
		text = string.Concat(new string[]
		{
			text,
			EClass.pc.depression.name,
			": ",
			this.depression.ToString(),
			"\n"
		});
		text = string.Concat(new string[]
		{
			text,
			EClass.pc.bladder.name,
			": ",
			this.bladder.ToString(),
			"\n"
		});
		return text + EClass.pc.hygiene.name + ": " + this.hygine.ToString();
	}

	public int happiness;

	public int hunger;

	public int fatigue;

	public int depression;

	public int bladder;

	public int hygine;
}
