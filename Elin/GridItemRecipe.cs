public class GridItemRecipe : GridItem
{
	public Recipe r;

	public override void SetButton(ButtonGrid b)
	{
		b.mainText.text = "1";
		b.mainText.SetActive(enable: false);
		r.renderRow.SetImage(b.icon, null, r.GetDefaultColor());
		b.SetTooltip("note", delegate(UITooltip t)
		{
			UINote note = t.note;
			note.Clear();
			note.AddHeaderCard(r.Name);
			if (!r.GetDetail().IsEmpty())
			{
				note.AddText(r.GetDetail());
				note.Space();
			}
			if (r.source.NeedFactory)
			{
				note.AddText("reqFactory".lang(r.source.NameFactory));
			}
			else
			{
				note.AddText("reqNoFactory".lang());
			}
			note.Build();
		});
	}

	public override void OnClick(ButtonGrid b)
	{
	}
}
