public class GridItemCardSource : GridItem
{
	public CardRow source;

	public Thing thing;

	public override void SetButton(ButtonGrid b)
	{
		b.mainText.SetActive(enable: false);
		if (thing != null)
		{
			thing.SetImage(b.icon);
			b.SetTooltip("note", delegate(UITooltip t)
			{
				thing.WriteNote(t.note);
			});
		}
		else
		{
			source.SetImage(b.icon);
		}
	}
}
