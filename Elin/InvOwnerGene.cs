public class InvOwnerGene : InvOwnerDraglet
{
	public Chara tg;

	public override ProcessType processType => ProcessType.None;

	public override string langTransfer => "invGene";

	public InvOwnerGene(Card owner = null, Chara _tg = null)
		: base(owner, null, CurrencyType.None)
	{
		tg = _tg;
		count = 1;
	}

	public override bool ShouldShowGuide(Thing t)
	{
		if (t.c_DNA != null)
		{
			return tg.feat >= t.c_DNA.cost;
		}
		return false;
	}

	public override void _OnProcess(Thing t)
	{
		DNA.Type type = t.c_DNA.type;
		if (type != 0 && tg.c_genes != null && tg.CurrentGeneSlot >= tg.MaxGeneSlot)
		{
			SE.Beep();
			Msg.Say("tooManyGene", tg);
			return;
		}
		if (type == DNA.Type.Brain)
		{
			if (tg.c_genes != null)
			{
				foreach (DNA item in tg.c_genes.items)
				{
					if (item.type == DNA.Type.Brain)
					{
						SE.Beep();
						Msg.Say("invalidGeneBrain", tg);
						return;
					}
				}
			}
		}
		else
		{
			Element invalidFeat = t.c_DNA.GetInvalidFeat(tg);
			if (invalidFeat != null)
			{
				SE.Beep();
				Msg.Say("invalidGeneFeat", tg, invalidFeat.Name.ToTitleCase());
				return;
			}
			Element invalidAction = t.c_DNA.GetInvalidAction(tg);
			if (invalidAction != null)
			{
				SE.Beep();
				Msg.Say("invalidGeneAction", tg, invalidAction.Name.ToTitleCase());
				return;
			}
		}
		SE.Play("mutation");
		tg.PlayEffect("identify");
		Msg.Say("gene_modify", tg, t);
		tg.AddCard(t);
		ConSuspend condition = tg.GetCondition<ConSuspend>();
		condition.gene = t;
		condition.duration = t.c_DNA.GetDurationHour();
		condition.dateFinish = EClass.world.date.GetRaw(condition.duration);
	}

	public override void OnWriteNote(Thing t, UINote n)
	{
		if (ShouldShowGuide(t))
		{
			n.AddHeader("HeaderAdditionalTrait", "gene_hint");
			_ = tg.c_genes;
			int num = tg.MaxGeneSlot - tg.CurrentGeneSlot;
			int num2 = num - t.c_DNA.slot;
			int maxGeneSlot = tg.MaxGeneSlot;
			n.AddText("gene_hint_slot".lang(num.ToString() ?? "", num2.ToString() ?? "", maxGeneSlot.ToString() ?? ""), (num2 >= 0) ? FontColor.Good : FontColor.Bad);
			int cost = t.c_DNA.cost;
			int num3 = tg.feat - cost;
			n.AddText("gene_hint_cost".lang(tg.feat.ToString() ?? "", cost + ((cost == t.c_DNA.cost) ? "" : ("(" + t.c_DNA.cost + ")")), num3.ToString() ?? ""), (num3 >= 0) ? FontColor.Good : FontColor.Bad);
		}
	}
}
