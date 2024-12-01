using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHotItem : ButtonGridDrag
{
	[NonSerialized]
	public new int index;

	public WidgetHotbar widget;

	public override float extFixY => 10f;

	public override void RefreshItem()
	{
		HotItem item = base.item as HotItem;
		bool flag = item != null;
		instantClick = !widget.hotbar.IsUserHotbar;
		base.onClick.RemoveAllListeners();
		dragParent = ((flag && widget.CanRegisterItem) ? widget : null);
		icon.enabled = flag;
		base.interactable = flag || widget.CanRegisterItem || !widget.IsSealed;
		if (flag && item.Hidden)
		{
			icon.enabled = false;
			base.interactable = false;
		}
		mainText.SetActive(widget.extra.showShortcut);
		base.image.sprite = Core.Instance.refs.spritesHighlight[0];
		base.transition = item?.Transition ?? Transition.SpriteSwap;
		if (flag)
		{
			item.button = this;
			item.hotbar = widget.hotbar;
			base.onClick.AddListener(delegate
			{
				if (EClass.ui.BlockInput)
				{
					SE.BeepSmall();
				}
				else if (WidgetHotbar.registering)
				{
					RegisterHotbar();
				}
				else
				{
					if (!widget.Visible)
					{
						widget.ToggleVisible();
					}
					UIButton.buttonPos = base.transform.position;
					item.OnClick(this, widget.hotbar);
					EInput.Consume(0);
				}
			});
			onRightClick = delegate
			{
				item.OnRightClick(this);
			};
			tooltip.enable = !item.TextTip.IsEmpty();
			tooltip.offset = new Vector3(0f, (widget.transform.position.y < 200f) ? 70 : (-20), 0f);
			tooltip.onShowTooltip = delegate(UITooltip t)
			{
				string textTip = item.TextTip;
				t.textMain.text = textTip;
			};
			icon.material = (item.UseUIObjMaterial ? EClass.core.refs.matUIObj : null);
			item.SetImage(icon);
			Refresh();
			return;
		}
		subText.SetActive(enable: false);
		tooltip.enable = false;
		base.onClick.AddListener(delegate
		{
			if (EClass.ui.BlockInput)
			{
				SE.BeepSmall();
			}
			else if (WidgetHotbar.registering)
			{
				RegisterHotbar();
			}
			else
			{
				widget.OnClickEmptyItem(this);
			}
		});
		onRightClick = null;
	}

	public void Refresh()
	{
		if (item != null)
		{
			HotItem hotItem = item as HotItem;
			if ((bool)subText)
			{
				hotItem.SetSubText(subText);
			}
			if (hotItem.Thing != null)
			{
				card = hotItem.Thing;
			}
		}
	}

	public void RegisterHotbar()
	{
		SE.SelectHotitem();
		widget.SetItem(this, WidgetHotbar.registeringItem);
		Core.Instance.ui.RemoveLayer<LayerRegisterHotbar>();
	}

	public override void OnHover()
	{
		if (!WidgetHotbar.registering)
		{
			base.OnHover();
		}
	}
}
