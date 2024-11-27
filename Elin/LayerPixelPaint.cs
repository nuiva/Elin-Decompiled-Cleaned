using System;
using Empyrean.ColorPicker;
using UnityEngine;
using UnityEngine.UI;

public class LayerPixelPaint : ELayer
{
	public override void OnInit()
	{
		UIItem t = this.layoutColors.CreateMold(null);
		for (int i = 0; i < 8; i++)
		{
			UIItem item = Util.Instantiate<UIItem>(t, this.layoutColors);
			int _i = i;
			item.button1.icon.color = IntColor.FromInt(ELayer.core.config.colors[_i]);
			item.button1.SetOnClick(delegate
			{
				this.picker.SelectColor(item.button1.icon.color);
			});
			item.button2.SetOnClick(delegate
			{
				item.button1.icon.color = this.picker.SelectedColor;
				ELayer.core.config.colors[_i] = IntColor.ToInt(this.picker.SelectedColor);
				SE.Tab();
			});
		}
		this.layoutColors.RebuildLayout(false);
		this.windows[0].AddBottomButton("apply", delegate
		{
			Action action = this.onApply;
			if (action == null)
			{
				return;
			}
			action();
		}, false);
		this.windows[0].AddBottomButton("cancel", delegate
		{
			this.Close();
		}, false);
		this.paint.Init();
	}

	public void SetCanvas(TraitCanvas c)
	{
		this.onApply = delegate()
		{
			c.owner.PlaySound(c.owner.material.GetSoundImpact(null), 1f, true);
			c.owner.renderer.PlayAnime(AnimeID.Shiver, default(Vector3), false);
			Thing thing = c.owner.Split(1);
			thing.c_textureData = this.paint.tex.EncodeToPNG();
			thing.isModified = true;
			thing.ClearPaintSprite();
			thing.GetPaintSprite();
			thing.renderer.RefreshSprite();
			this.Close();
		};
		if (c.owner.c_textureData != null)
		{
			this.paint.tex.LoadImage(c.owner.c_textureData);
		}
		this.paint.imageMask.texture = c.owner.GetSprite(0).texture;
		this.paint.imageMask.SetNativeSize();
		this.paint.imagePreview.SetNativeSize();
	}

	public PixelPaint paint;

	public GridLayoutGroup layoutColors;

	public ColorPicker picker;

	public Action onApply;
}
