using System;

public class ListPeopleBed : ListPeople
{
	public override void OnInstantiate(Chara c, ItemGeneral i)
	{
		UIButton uibutton = i.AddSubButton(EClass.core.refs.icons.bed, delegate
		{
		}, null, delegate(UITooltip t)
		{
		});
		uibutton.icon.SetAlpha((c.FindBed() != null) ? 0.9f : 0.4f);
		uibutton.tooltip.enable = false;
	}

	public override void OnClick(Chara c, ItemGeneral i)
	{
		if (this.main)
		{
			if (!this.bed.CanAssign(c))
			{
				SE.Beep();
				return;
			}
			TraitBed traitBed = c.FindBed();
			if (traitBed != null)
			{
				traitBed.RemoveHolder(c);
			}
			this.bed.AddHolder(c);
		}
		else
		{
			this.bed.RemoveHolder(c);
		}
		base.MoveToOther(c);
		base.Main.OnRefreshMenu();
	}

	public override void OnList()
	{
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.IsPCFaction && chara.memberType == FactionMemberType.Default)
			{
				if (this.main)
				{
					if (!this.bed.IsHolder(chara))
					{
						this.list.Add(chara);
					}
				}
				else if (this.bed.IsHolder(chara))
				{
					this.list.Add(chara);
				}
			}
		}
	}

	public override void OnRefreshMenu()
	{
	}

	public TraitBed bed;
}
