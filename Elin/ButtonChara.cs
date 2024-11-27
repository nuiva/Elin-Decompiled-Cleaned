using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonChara : UIButton
{
	public void SetChara(Chara c, ButtonChara.Mode m)
	{
		if (this.portrait)
		{
			this.portrait.SetChara(c, null);
		}
		if (m == ButtonChara.Mode.Ranking)
		{
			this.textBio.text = ((c.currentZone == null) ? "???" : c.currentZone.Name);
		}
		else
		{
			this.textAlias.text = c.Aka;
			if (this.textBio)
			{
				this.textBio.text = string.Concat(new string[]
				{
					c.race.GetText("name", false).ToTitleCase(true),
					" ",
					Lang.Parse("age", c.bio.TextAge(c), null, null, null, null),
					" ",
					Lang._gender(c.bio.gender)
				});
			}
		}
		this.textName.text = c.Name;
		if (this.layoutTag)
		{
			this.layoutTag.DestroyChildren(false, true);
			HintIcon hintIcon = Util.Instantiate<HintIcon>("UI/Element/Item/Tag General", this.layoutTag);
			hintIcon.text.SetText(c.job.GetName().ToTitleCase(true));
			hintIcon.RebuildLayout(false);
			this.layoutTag.RebuildLayout(false);
		}
	}

	public Portrait portrait;

	public UIText textName;

	public UIText textAlias;

	public UIText textBio;

	public UIText textWork;

	public UIText textHobby;

	public UIText textLifeStyle;

	public new UIItem item;

	public Transform transDefeated;

	public LayoutGroup layoutTag;

	public enum Mode
	{
		Default,
		Hire,
		Journal,
		Embark,
		Ranking
	}
}
