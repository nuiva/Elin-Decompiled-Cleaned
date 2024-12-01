using UnityEngine;

public class TraitBookPlan : TraitBookSkill
{
	public override bool IsPlan => true;

	public override int GetValue()
	{
		return base.GetValue() * (Mathf.Max(source.cost[0], 3) * 17) / 100;
	}

	public override void WriteNote(UINote n, bool identified)
	{
		base.WriteNote(n, identified);
		n.Space();
		foreach (FactionBranch child in EClass.pc.faction.GetChildren())
		{
			bool flag = child.elements.HasBase(base.idEle);
			n.AddText("_bullet".lang() + child.owner.Name + " " + (flag ? "alreadyLearned" : "notLearned").lang(), flag ? FontColor.Good : FontColor.Warning);
		}
	}
}
