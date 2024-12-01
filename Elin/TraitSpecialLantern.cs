public class TraitSpecialLantern : TraitLightSource
{
	public SourceBacker.Row source => EClass.sources.backers.map.TryGetValue(owner.c_idBacker);

	public bool ShowBackerContent
	{
		get
		{
			if (owner.isBackerContent && source != null)
			{
				return EClass.core.config.backer.Show(owner.c_idBacker);
			}
			return false;
		}
	}

	public override void OnCreate(int lv)
	{
		if (EClass.rnd(10) == 0 && owner != null && EClass._zone != null && EClass._zone.IsFestival && !owner.isBackerContent)
		{
			SourceBacker.Row row = EClass.sources.backers.listLantern.NextItem(ref BackerContent.indexLantern);
			if (row != null)
			{
				owner.ApplyBacker(row.id);
			}
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (ShowBackerContent)
		{
			p.TrySetAct("actRead", delegate
			{
				SourceBacker.Row row = EClass.sources.backers.map[owner.c_idBacker];
				Msg.Say("backerLantern_read");
				Msg.Say("backerLantern", row.Text);
				return false;
			}, owner);
		}
	}

	public override void SetName(ref string s)
	{
		if (ShowBackerContent)
		{
			s = "_of".lang(source.Name, s);
		}
	}
}
