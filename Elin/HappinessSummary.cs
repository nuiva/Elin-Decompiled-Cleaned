using System.Collections.Generic;

public class HappinessSummary : EClass
{
	public int happiness;

	public int hunger;

	public int fatigue;

	public int depression;

	public int bladder;

	public int hygine;

	public HappinessSummary(FactionBranch b)
	{
		List<Chara> members = b.members;
		foreach (Chara item in members)
		{
			happiness += item.happiness;
			hunger += item.hunger.value;
			fatigue += item.stamina.value;
			depression += item.depression.value;
			bladder += item.bladder.value;
			hygine += item.hygiene.value;
		}
		int count = members.Count;
		happiness /= count;
		hunger /= count;
		fatigue /= count;
		depression /= count;
		bladder /= count;
		hygine /= count;
	}

	public string GetText()
	{
		string text = "";
		text = text + "happiness".lang() + ": " + happiness + "\n";
		text = text + EClass.pc.hunger.name + ": " + hunger + "\n";
		text = text + EClass.pc.stamina.name + ": " + fatigue + "\n";
		text = text + EClass.pc.depression.name + ": " + depression + "\n";
		text = text + EClass.pc.bladder.name + ": " + bladder + "\n";
		return text + EClass.pc.hygiene.name + ": " + hygine;
	}
}
