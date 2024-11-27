using System;
using UnityEngine;
using UnityEngine.UI;

public class HotItem : UIButton.Item
{
	public override Sprite SpriteHighlight
	{
		get
		{
			return EClass.core.refs.spritesHighlight[1];
		}
	}

	public override bool IsSelectable
	{
		get
		{
			return true;
		}
	}

	public override string TextTip
	{
		get
		{
			return base.TextTip + this.TextHotkey();
		}
	}

	public string TextHotkey()
	{
		if (!this.hotbar.ShowFunctionKey)
		{
			return "";
		}
		int num = this.hotbar.CurrentPage.items.IndexOf(this) + 1;
		if (num > 8)
		{
			return "";
		}
		return " (F" + num.ToString() + ")";
	}

	public virtual void OnClick(ButtonHotItem b, Hotbar h)
	{
		this.OnClick(b);
	}

	public virtual void OnRightClick(ButtonHotItem b)
	{
		WidgetHotbar widget = b.widget;
		if (widget == null)
		{
			return;
		}
		widget.ShowContextMenu();
	}

	public virtual Thing RenderThing
	{
		get
		{
			if (EClass.pc.ai.RenderThing != null)
			{
				return EClass.pc.ai.RenderThing;
			}
			if (EClass.player.renderThing != null)
			{
				return EClass.player.renderThing;
			}
			Thing thing = this.Thing;
			if (!(((thing != null) ? thing.trait : null) is TraitAbility))
			{
				return this.Thing;
			}
			return null;
		}
	}

	public virtual Thing Thing
	{
		get
		{
			return null;
		}
	}

	public virtual Thing Tool
	{
		get
		{
			return null;
		}
	}

	public virtual bool IsTool
	{
		get
		{
			return false;
		}
	}

	public virtual bool LookAtMouse
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsGameAction
	{
		get
		{
			return false;
		}
	}

	public virtual bool UseUIObjMaterial
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanAutoFire(Chara tg)
	{
		return false;
	}

	public virtual void OnSetCurrentItem()
	{
	}

	public virtual void OnUnsetCurrentItem()
	{
	}

	public virtual void OnUnselect()
	{
	}

	public virtual void SetImage(Image icon)
	{
		icon.sprite = (this.GetSprite() ?? EClass.core.refs.icons.defaultHotItem);
		icon.color = this.SpriteColor;
		icon.transform.localScale = this.SpriteScale;
		icon.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		if (this.AdjustImageSize)
		{
			icon.SetNativeSize();
			return;
		}
		icon.Rect().sizeDelta = new Vector2(48f, 48f);
	}

	public virtual bool TrySetAct(ActPlan p)
	{
		return false;
	}

	public virtual void OnMarkMapHighlights()
	{
	}

	public virtual void OnRenderTile(Point point, HitResult result, int dir)
	{
	}

	public ButtonHotItem button;

	public Hotbar hotbar;

	public bool disabled;
}
