using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerRegisterHotbar : ELayer
{
	public void SetItem(SourceElement.Row act)
	{
		this.SetItem(new HotItemAct
		{
			id = act.id
		});
	}

	public LayerRegisterHotbar SetItem(Thing t)
	{
		return this.SetItem(t.trait.GetHotItem());
	}

	public void SetItem(Chara c)
	{
		this.SetItem(new HotItemChara
		{
			uid = c.uid
		});
	}

	public LayerRegisterHotbar SetItem(HotItem item)
	{
		WidgetHotbar.registeringItem = item;
		this.Show();
		return this;
	}

	public void Show()
	{
		WidgetHotbar.registering = true;
		foreach (Widget widget in ELayer.ui.widgets.list)
		{
			WidgetHotbar widgetHotbar = widget as WidgetHotbar;
			if (!(widgetHotbar == null) && widgetHotbar.CanRegisterItem && !widgetHotbar.hotbar.IsLocked)
			{
				if (!widgetHotbar.Visible)
				{
					widgetHotbar.ToggleVisible();
				}
				widgetHotbar.transform.SetParent(base.transform, false);
				this.hotbars.Add(widgetHotbar);
			}
		}
	}

	public ButtonHotItem GetButton()
	{
		foreach (WidgetHotbar widgetHotbar in this.hotbars)
		{
			foreach (ButtonHotItem buttonHotItem in widgetHotbar.buttons)
			{
				if (InputModuleEX.IsPointerOver(buttonHotItem))
				{
					return buttonHotItem;
				}
			}
		}
		return null;
	}

	public bool OnEndDrag()
	{
		ButtonHotItem button = this.GetButton();
		if (button)
		{
			button.onClick.Invoke();
			return true;
		}
		return false;
	}

	public override void OnKill()
	{
		foreach (WidgetHotbar widgetHotbar in this.hotbars)
		{
			widgetHotbar.transform.SetParent(ELayer.ui.widgets.transform, false);
		}
		foreach (Image image in this.covers)
		{
			UnityEngine.Object.DestroyImmediate(image.gameObject);
		}
		WidgetHotbar.registering = false;
		WidgetHotbar.registeringItem = null;
	}

	public Image moldCover;

	private List<WidgetHotbar> hotbars = new List<WidgetHotbar>();

	private List<Image> covers = new List<Image>();
}
