using System;

public class GridItemCard : GridItem
{
	public override void SetButton(ButtonGrid b)
	{
		b.mainText.text = (this.c.Num.ToString() ?? "");
		this.c.SetImage(b.icon);
		b.SetTooltip("note", delegate(UITooltip t)
		{
			this.c.WriteNote(t.note, null, IInspect.NoteMode.Default, null);
		}, true);
	}

	public override void OnClick(ButtonGrid b)
	{
		EClass.ui.AddLayer<LayerInfo>().Set(this.c, false);
	}

	public override void AutoAdd()
	{
		EClass.player.AddInventory(this.c);
	}

	public Card c;
}
