using UnityEngine;
using UnityEngine.UI;

public class HotItem : UIButton.Item
{
	public ButtonHotItem button;

	public Hotbar hotbar;

	public bool disabled;

	public override Sprite SpriteHighlight => EClass.core.refs.spritesHighlight[1];

	public override bool IsSelectable => true;

	public override string TextTip => base.TextTip + TextHotkey();

	public virtual Thing RenderThing
	{
		get
		{
			if (EClass.pc.ai.RenderThing == null)
			{
				if (EClass.player.renderThing == null)
				{
					if (!(Thing?.trait is TraitAbility))
					{
						return Thing;
					}
					return null;
				}
				return EClass.player.renderThing;
			}
			return EClass.pc.ai.RenderThing;
		}
	}

	public virtual Thing Thing => null;

	public virtual Thing Tool => null;

	public virtual bool IsTool => false;

	public virtual bool LookAtMouse => false;

	public virtual bool IsGameAction => false;

	public virtual bool UseUIObjMaterial => false;

	public string TextHotkey()
	{
		if (!hotbar.ShowFunctionKey)
		{
			return "";
		}
		int num = hotbar.CurrentPage.items.IndexOf(this) + 1;
		if (num > 8)
		{
			return "";
		}
		return " (F" + num + ")";
	}

	public virtual void OnClick(ButtonHotItem b, Hotbar h)
	{
		OnClick(b);
	}

	public virtual void OnRightClick(ButtonHotItem b)
	{
		b.widget?.ShowContextMenu();
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
		icon.sprite = GetSprite() ?? EClass.core.refs.icons.defaultHotItem;
		icon.color = SpriteColor;
		icon.transform.localScale = SpriteScale;
		icon.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		if (AdjustImageSize)
		{
			icon.SetNativeSize();
		}
		else
		{
			icon.Rect().sizeDelta = new Vector2(48f, 48f);
		}
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
}
