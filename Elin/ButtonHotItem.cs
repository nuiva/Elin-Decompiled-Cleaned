using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHotItem : ButtonGridDrag
{
	public override float extFixY
	{
		get
		{
			return 10f;
		}
	}

	public override void RefreshItem()
	{
		HotItem item = this.item as HotItem;
		bool flag = item != null;
		this.instantClick = !this.widget.hotbar.IsUserHotbar;
		base.onClick.RemoveAllListeners();
		this.dragParent = ((flag && this.widget.CanRegisterItem) ? this.widget : null);
		this.icon.enabled = flag;
		base.interactable = (flag || this.widget.CanRegisterItem || !this.widget.IsSealed);
		if (flag && item.Hidden)
		{
			this.icon.enabled = false;
			base.interactable = false;
		}
		this.mainText.SetActive(this.widget.extra.showShortcut);
		base.image.sprite = Core.Instance.refs.spritesHighlight[0];
		HotItem item2 = item;
		base.transition = ((item2 != null) ? item2.Transition : Selectable.Transition.SpriteSwap);
		if (flag)
		{
			item.button = this;
			item.hotbar = this.widget.hotbar;
			base.onClick.AddListener(delegate()
			{
				if (EClass.ui.BlockInput)
				{
					SE.BeepSmall();
					return;
				}
				if (WidgetHotbar.registering)
				{
					this.RegisterHotbar();
					return;
				}
				if (!this.widget.Visible)
				{
					this.widget.ToggleVisible();
				}
				UIButton.buttonPos = this.transform.position;
				item.OnClick(this, this.widget.hotbar);
				EInput.Consume(0);
			});
			this.onRightClick = delegate()
			{
				item.OnRightClick(this);
			};
			this.tooltip.enable = !item.TextTip.IsEmpty();
			this.tooltip.offset = new Vector3(0f, (float)((this.widget.transform.position.y < 200f) ? 70 : -20), 0f);
			this.tooltip.onShowTooltip = delegate(UITooltip t)
			{
				string textTip = item.TextTip;
				t.textMain.text = textTip;
			};
			this.icon.material = (item.UseUIObjMaterial ? EClass.core.refs.matUIObj : null);
			item.SetImage(this.icon);
			this.Refresh();
			return;
		}
		this.subText.SetActive(false);
		this.tooltip.enable = false;
		base.onClick.AddListener(delegate()
		{
			if (EClass.ui.BlockInput)
			{
				SE.BeepSmall();
				return;
			}
			if (WidgetHotbar.registering)
			{
				this.RegisterHotbar();
				return;
			}
			this.widget.OnClickEmptyItem(this);
		});
		this.onRightClick = null;
	}

	public void Refresh()
	{
		if (this.item == null)
		{
			return;
		}
		HotItem hotItem = this.item as HotItem;
		if (this.subText)
		{
			hotItem.SetSubText(this.subText);
		}
		if (hotItem.Thing != null)
		{
			this.card = hotItem.Thing;
		}
	}

	public void RegisterHotbar()
	{
		SE.SelectHotitem();
		this.widget.SetItem(this, WidgetHotbar.registeringItem);
		Core.Instance.ui.RemoveLayer<LayerRegisterHotbar>();
	}

	public override void OnHover()
	{
		if (WidgetHotbar.registering)
		{
			return;
		}
		base.OnHover();
	}

	[NonSerialized]
	public new int index;

	public WidgetHotbar widget;
}
