using System;
using System.Collections.Generic;

public class TraitPainter : TraitItem
{
	public virtual TraitPainter.Type PaintType
	{
		get
		{
			return TraitPainter.Type.Paint;
		}
	}

	public override bool CanUse(Chara c)
	{
		return this.GetCanvas() != null;
	}

	public override bool OnUse(Chara c)
	{
		ActionMode.Paint.SetPainter(this);
		return false;
	}

	public TraitCanvas GetCanvas()
	{
		List<Thing> list = EClass.pc.things.List((Thing t) => t.trait is TraitCanvas && (t.trait as TraitCanvas).CanvasType == this.PaintType && t.c_textureData == null, false);
		if (list.Count <= 0)
		{
			return null;
		}
		return list[0].trait as TraitCanvas;
	}

	public enum Type
	{
		Paint,
		Camera,
		Paper
	}
}
