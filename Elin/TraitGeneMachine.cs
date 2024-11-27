using System;
using UnityEngine;

public class TraitGeneMachine : TraitStasisChamber
{
	public override bool CanBeOnlyBuiltInHome
	{
		get
		{
			return true;
		}
	}

	public override bool IsHomeItem
	{
		get
		{
			return true;
		}
	}

	public Chara GetTarget()
	{
		foreach (Chara chara in this.owner.pos.Charas)
		{
			ConSuspend condition = chara.GetCondition<ConSuspend>();
			if (condition != null && condition.uidMachine == this.owner.uid)
			{
				return chara;
			}
		}
		return null;
	}

	public bool IsTargetUsingGene()
	{
		Chara target = this.GetTarget();
		return target != null && target.GetCondition<ConSuspend>().HasGene;
	}

	public float GetProgress()
	{
		ConSuspend condition = this.GetTarget().GetCondition<ConSuspend>();
		if (condition == null || !condition.HasGene)
		{
			return 0f;
		}
		int remainingHours = EClass.world.date.GetRemainingHours(condition.dateFinish);
		if (condition.duration == 0 || EClass.debug.enable)
		{
			return 1f;
		}
		return Mathf.Clamp((float)(condition.duration - remainingHours) / (float)condition.duration, 0f, 1f);
	}

	public string GetProgressText()
	{
		ConSuspend condition = this.GetTarget().GetCondition<ConSuspend>();
		int remainingHours = EClass.world.date.GetRemainingHours(condition.dateFinish);
		if (remainingHours > 0)
		{
			return remainingHours.ToString() + " h";
		}
		return "progress_finish".lang();
	}

	public override bool CanUse(Chara c)
	{
		return this.owner.IsInstalled && this.owner.isOn && (!this.IsTargetUsingGene() || this.GetProgress() >= 1f);
	}

	public override bool OnUse(Chara c)
	{
		Chara target = this.GetTarget();
		if (target == null)
		{
			LayerPeople.CreateSelect("", "", delegate(UIList l)
			{
				foreach (Chara chara in EClass.Branch.members)
				{
					if (chara.GetCondition<ConSuspend>() == null && chara.host == null && !chara.IsPC && chara.IsAliveInCurrentZone && chara.memberType == FactionMemberType.Default)
					{
						l.Add(chara);
					}
				}
			}, delegate(Chara c)
			{
				if (c.IsPCParty)
				{
					EClass.pc.party.RemoveMember(c);
				}
				if (!c.pos.Equals(this.owner.pos))
				{
					EClass.pc.Kick(this.owner.pos, false);
					c.Teleport(this.owner.pos, false, true);
					c.isRestrained = false;
				}
				if (EClass.debug.enable)
				{
					if (c.c_genes == null)
					{
						c.c_genes = new CharaGenes();
					}
					c.c_genes.inferior += 20;
					c.feat += 500;
				}
				c.RemoveCondition<ConSleep>();
				c.PlaySound("ride", 1f, true);
				(c.AddCondition<ConSuspend>(100, true) as ConSuspend).uidMachine = this.owner.uid;
			}, (Chara a) => "gene_note".lang(((a.c_genes != null) ? a.c_genes.items.Count : 0).ToString() ?? "", a.MaxGene.ToString() ?? "", a.feat.ToString() ?? "", a.GetTotalFeat().ToString() + " ", null));
		}
		else if (this.GetProgress() >= 1f)
		{
			ConSuspend condition = target.GetCondition<ConSuspend>();
			if (condition.gene.GetRootCard() != target)
			{
				Msg.SayNothingHappen();
			}
			else
			{
				target.Say("gene_finish", target, condition.gene, null, null);
				condition.gene.c_DNA.Apply(target);
				condition.gene.Destroy();
				condition.gene = null;
			}
			target.RemoveCondition<ConSuspend>();
			target.MoveNeighborDefinitely();
			target.PlaySound("ding_potential", 1f, true);
			target.pos.PlayEffect("mutation");
		}
		else
		{
			LayerDragGrid.Create(new InvOwnerGene(this.owner, target), false);
		}
		return true;
	}
}
