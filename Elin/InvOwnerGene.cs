using System;
using System.Collections.Generic;
using UnityEngine;

public class InvOwnerGene : InvOwnerDraglet
{
	public override InvOwnerDraglet.ProcessType processType
	{
		get
		{
			return InvOwnerDraglet.ProcessType.None;
		}
	}

	public override string langTransfer
	{
		get
		{
			return "invGene";
		}
	}

	public InvOwnerGene(Card owner = null, Chara _tg = null) : base(owner, null, CurrencyType.None)
	{
		this.tg = _tg;
		this.count = 1;
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return t.c_DNA != null && this.tg.feat >= t.c_DNA.cost;
	}

	public override void _OnProcess(Thing t)
	{
		DNA.Type type = t.c_DNA.type;
		if (type != DNA.Type.Inferior && this.tg.c_genes != null && this.tg.c_genes.items.Count >= this.tg.MaxGene)
		{
			SE.Beep();
			Msg.Say("tooManyGene", this.tg, null, null, null);
			return;
		}
		if (type == DNA.Type.Brain)
		{
			if (this.tg.c_genes == null)
			{
				goto IL_144;
			}
			using (List<DNA>.Enumerator enumerator = this.tg.c_genes.items.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.type == DNA.Type.Brain)
					{
						SE.Beep();
						Msg.Say("invalidGeneBrain", this.tg, null, null, null);
						return;
					}
				}
				goto IL_144;
			}
		}
		Element invalidFeat = t.c_DNA.GetInvalidFeat(this.tg);
		if (invalidFeat != null)
		{
			SE.Beep();
			Msg.Say("invalidGeneFeat", this.tg, invalidFeat.Name.ToTitleCase(false), null, null);
			return;
		}
		Element invalidAction = t.c_DNA.GetInvalidAction(this.tg);
		if (invalidAction != null)
		{
			SE.Beep();
			Msg.Say("invalidGeneAction", this.tg, invalidAction.Name.ToTitleCase(false), null, null);
			return;
		}
		IL_144:
		SE.Play("mutation");
		this.tg.PlayEffect("identify", true, 0f, default(Vector3));
		Msg.Say("gene_modify", this.tg, t, null, null);
		this.tg.AddCard(t);
		ConSuspend condition = this.tg.GetCondition<ConSuspend>();
		condition.gene = t;
		condition.duration = t.c_DNA.GetDurationHour();
		condition.dateFinish = EClass.world.date.GetRaw(condition.duration);
	}

	public override void OnWriteNote(Thing t, UINote n)
	{
		if (!this.ShouldShowGuide(t))
		{
			return;
		}
		n.AddHeader("HeaderAdditionalTrait", "gene_hint", null);
		CharaGenes c_genes = this.tg.c_genes;
		int num = (c_genes == null) ? 0 : c_genes.items.Count;
		int num2 = num + 1;
		int maxGene = this.tg.MaxGene;
		n.AddText("gene_hint_slot".lang(num.ToString() ?? "", num2.ToString() ?? "", maxGene.ToString() ?? "", null, null), (num2 <= maxGene) ? FontColor.Good : FontColor.Bad);
		int cost = t.c_DNA.cost;
		int num3 = this.tg.feat - cost;
		n.AddText("gene_hint_cost".lang(this.tg.feat.ToString() ?? "", cost.ToString() + ((cost == t.c_DNA.cost) ? "" : ("(" + t.c_DNA.cost.ToString() + ")")), num3.ToString() ?? "", null, null), (num3 >= 0) ? FontColor.Good : FontColor.Bad);
	}

	public Chara tg;
}
