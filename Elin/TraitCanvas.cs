using System;
using UnityEngine;
using UnityEngine.UI;

public class TraitCanvas : Trait
{
	public virtual bool PointFilter
	{
		get
		{
			return false;
		}
	}

	public virtual float Scale
	{
		get
		{
			return 1f;
		}
	}

	public virtual TraitPainter.Type CanvasType
	{
		get
		{
			return TraitPainter.Type.Paint;
		}
	}

	public override void OnSetCardGrid(ButtonGrid b)
	{
		if (this.owner.c_textureData == null)
		{
			return;
		}
		Sprite paintSprite = this.owner.GetPaintSprite();
		b.Attach<Image>("canvas", false).sprite = paintSprite;
	}
}
