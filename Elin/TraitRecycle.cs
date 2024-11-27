using System;

public class TraitRecycle : TraitItem
{
	public override bool OnUse(Chara c)
	{
		LayerDragGrid.CreateRecycle(this);
		return false;
	}
}
