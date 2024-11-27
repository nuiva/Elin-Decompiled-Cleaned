using System;

public class TraitSpecialLantern : TraitLightSource
{
	public SourceBacker.Row source
	{
		get
		{
			return EClass.sources.backers.map.TryGetValue(this.owner.c_idBacker, null);
		}
	}

	public bool ShowBackerContent
	{
		get
		{
			return this.owner.isBackerContent && this.source != null && EClass.core.config.backer.Show(this.owner.c_idBacker);
		}
	}

	public override void OnCreate(int lv)
	{
		if (EClass.rnd(10) == 0 && this.owner != null && EClass._zone != null && EClass._zone.IsFestival && !this.owner.isBackerContent)
		{
			SourceBacker.Row row = EClass.sources.backers.listLantern.NextItem(ref BackerContent.indexLantern);
			if (row != null)
			{
				this.owner.ApplyBacker(row.id);
			}
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (this.ShowBackerContent)
		{
			p.TrySetAct("actRead", delegate()
			{
				SourceBacker.Row row = EClass.sources.backers.map[this.owner.c_idBacker];
				Msg.Say("backerLantern_read");
				Msg.Say("backerLantern", row.Text, null, null, null);
				return false;
			}, this.owner, null, 1, false, true, false);
		}
	}

	public override void SetName(ref string s)
	{
		if (this.ShowBackerContent)
		{
			s = "_of".lang(this.source.Name, s, null, null, null);
		}
	}
}
