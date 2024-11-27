using System;

public class TraitStatue : TraitFigure
{
	public override int GetMatColor()
	{
		return this.owner.colorInt * -1;
	}
}
