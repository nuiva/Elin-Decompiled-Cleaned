using System;

public class GridItemCardSource : GridItem
{
	public override void SetButton(ButtonGrid b)
	{
		b.mainText.SetActive(false);
		if (this.thing != null)
		{
			this.thing.SetImage(b.icon);
			b.SetTooltip("note", delegate(UITooltip t)
			{
				this.thing.WriteNote(t.note, null, IInspect.NoteMode.Default, null);
			}, true);
			return;
		}
		this.source.SetImage(b.icon, null, 0, true, 0, 0);
	}

	public CardRow source;

	public Thing thing;
}
