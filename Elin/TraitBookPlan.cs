using System;
using UnityEngine;

public class TraitBookPlan : TraitBookSkill
{
	public override bool IsPlan
	{
		get
		{
			return true;
		}
	}

	public override int GetValue()
	{
		return base.GetValue() * (Mathf.Max(this.source.cost[0], 3) * 17) / 100;
	}

	public override void WriteNote(UINote n, bool identified)
	{
		base.WriteNote(n, identified);
		n.Space(0, 1);
		foreach (FactionBranch factionBranch in EClass.pc.faction.GetChildren())
		{
			bool flag = factionBranch.elements.HasBase(base.idEle);
			n.AddText("_bullet".lang() + factionBranch.owner.Name + " " + (flag ? "alreadyLearned" : "notLearned").lang(), flag ? FontColor.Good : FontColor.Warning);
		}
	}
}
