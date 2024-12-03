using System.Collections.Generic;

public class TraitPainter : TraitItem
{
	public enum Type
	{
		Paint,
		Camera,
		Paper
	}

	public virtual Type PaintType => Type.Paint;

	public override bool CanUse(Chara c)
	{
		return GetCanvas() != null;
	}

	public override bool OnUse(Chara c)
	{
		ActionMode.Paint.SetPainter(this);
		return false;
	}

	public TraitCanvas GetCanvas()
	{
		List<Thing> list = EClass.pc.things.List((Thing t) => t.trait is TraitCanvas && (t.trait as TraitCanvas).CanvasType == PaintType && t.c_textureData == null, onlyAccessible: true);
		if (list.Count <= 0)
		{
			return null;
		}
		return list[0].trait as TraitCanvas;
	}
}
