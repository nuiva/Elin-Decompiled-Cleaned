using System;

public class GridItemRecipe : GridItem
{
	public override void SetButton(ButtonGrid b)
	{
		b.mainText.text = "1";
		b.mainText.SetActive(false);
		this.r.renderRow.SetImage(b.icon, null, this.r.GetDefaultColor(), true, 0, 0);
		b.SetTooltip("note", delegate(UITooltip t)
		{
			UINote note = t.note;
			note.Clear();
			note.AddHeaderCard(this.r.Name, null);
			if (!this.r.GetDetail().IsEmpty())
			{
				note.AddText(this.r.GetDetail(), FontColor.DontChange);
				note.Space(0, 1);
			}
			if (this.r.source.NeedFactory)
			{
				note.AddText("reqFactory".lang(this.r.source.NameFactory, null, null, null, null), FontColor.DontChange);
			}
			else
			{
				note.AddText("reqNoFactory".lang(), FontColor.DontChange);
			}
			note.Build();
		}, true);
	}

	public override void OnClick(ButtonGrid b)
	{
	}

	public Recipe r;
}
