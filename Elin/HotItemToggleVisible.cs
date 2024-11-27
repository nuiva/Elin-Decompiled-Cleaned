using System;
using UnityEngine;

public class HotItemToggleVisible : HotItemIcon
{
	public override string Name
	{
		get
		{
			return "s_visible".lang();
		}
	}

	public override string TextTip
	{
		get
		{
			return null;
		}
	}

	public override bool KeepVisibleWhenHighlighted
	{
		get
		{
			return true;
		}
	}

	public override bool ShouldHighlight()
	{
		return this.sticky;
	}

	public override bool UseIconForHighlight
	{
		get
		{
			return true;
		}
	}

	public override Sprite GetSprite(bool highlight)
	{
		if (!highlight)
		{
			return base.GetSprite();
		}
		return SpriteSheet.Get("icon_visible_highlight");
	}

	public override void OnHover(UIButton b)
	{
		ButtonHotItem buttonHotItem = b as ButtonHotItem;
		if (!buttonHotItem.widget.Visible)
		{
			this.OnClick(buttonHotItem, buttonHotItem.widget.hotbar);
		}
	}

	public override void OnClick(ButtonHotItem b, Hotbar h)
	{
		if (!b)
		{
			SE.Beep();
			return;
		}
		if (b.widget.Visible)
		{
			SE.ClickGeneral();
			this.sticky = !this.sticky;
		}
		else
		{
			SE.Play("pop_context");
			b.widget.ToggleVisible();
		}
		b.widget.RefreshHighlight();
	}

	public bool sticky;
}
