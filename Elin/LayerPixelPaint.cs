using System;
using Empyrean.ColorPicker;
using UnityEngine;
using UnityEngine.UI;

public class LayerPixelPaint : ELayer
{
	public PixelPaint paint;

	public GridLayoutGroup layoutColors;

	public ColorPicker picker;

	public Action onApply;

	public override void OnInit()
	{
		UIItem t = layoutColors.CreateMold<UIItem>();
		for (int i = 0; i < 8; i++)
		{
			UIItem item = Util.Instantiate(t, layoutColors);
			int _i = i;
			item.button1.icon.color = IntColor.FromInt(ELayer.core.config.colors[_i]);
			item.button1.SetOnClick(delegate
			{
				picker.SelectColor(item.button1.icon.color);
			});
			item.button2.SetOnClick(delegate
			{
				item.button1.icon.color = picker.SelectedColor;
				ELayer.core.config.colors[_i] = IntColor.ToInt(picker.SelectedColor);
				SE.Tab();
			});
		}
		layoutColors.RebuildLayout();
		windows[0].AddBottomButton("apply", delegate
		{
			onApply?.Invoke();
		});
		windows[0].AddBottomButton("cancel", delegate
		{
			Close();
		});
		paint.Init();
	}

	public void SetCanvas(TraitCanvas c)
	{
		onApply = delegate
		{
			c.owner.PlaySound(c.owner.material.GetSoundImpact());
			c.owner.renderer.PlayAnime(AnimeID.Shiver);
			Thing thing = c.owner.Split(1);
			thing.c_textureData = paint.tex.EncodeToPNG();
			thing.isModified = true;
			thing.ClearPaintSprite();
			thing.GetPaintSprite();
			thing.renderer.RefreshSprite();
			Close();
		};
		if (c.owner.c_textureData != null)
		{
			paint.tex.LoadImage(c.owner.c_textureData);
		}
		paint.imageMask.texture = c.owner.GetSprite().texture;
		paint.imageMask.SetNativeSize();
		paint.imagePreview.SetNativeSize();
	}
}
