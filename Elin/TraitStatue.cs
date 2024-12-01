public class TraitStatue : TraitFigure
{
	public override int GetMatColor()
	{
		return owner.colorInt * -1;
	}
}
