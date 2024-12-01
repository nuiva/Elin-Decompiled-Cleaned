using UnityEngine;

public class HotItemToggleVisible : HotItemIcon
{
	public bool sticky;

	public override string Name => "s_visible".lang();

	public override string TextTip => null;

	public override bool KeepVisibleWhenHighlighted => true;

	public override bool UseIconForHighlight => true;

	public override bool ShouldHighlight()
	{
		return sticky;
	}

	public override Sprite GetSprite(bool highlight)
	{
		if (!highlight)
		{
			return GetSprite();
		}
		return SpriteSheet.Get("icon_visible_highlight");
	}

	public override void OnHover(UIButton b)
	{
		ButtonHotItem buttonHotItem = b as ButtonHotItem;
		if (!buttonHotItem.widget.Visible)
		{
			OnClick(buttonHotItem, buttonHotItem.widget.hotbar);
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
			sticky = !sticky;
		}
		else
		{
			SE.Play("pop_context");
			b.widget.ToggleVisible();
		}
		b.widget.RefreshHighlight();
	}
}
