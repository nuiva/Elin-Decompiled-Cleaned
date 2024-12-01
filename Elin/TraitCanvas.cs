using UnityEngine;
using UnityEngine.UI;

public class TraitCanvas : Trait
{
	public virtual bool PointFilter => false;

	public virtual float Scale => 1f;

	public virtual TraitPainter.Type CanvasType => TraitPainter.Type.Paint;

	public override void OnSetCardGrid(ButtonGrid b)
	{
		if (owner.c_textureData != null)
		{
			Sprite paintSprite = owner.GetPaintSprite();
			b.Attach<Image>("canvas", rightAttach: false).sprite = paintSprite;
		}
	}
}
