using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerRegisterHotbar : ELayer
{
	public Image moldCover;

	private List<WidgetHotbar> hotbars = new List<WidgetHotbar>();

	private List<Image> covers = new List<Image>();

	public void SetItem(SourceElement.Row act)
	{
		SetItem(new HotItemAct
		{
			id = act.id
		});
	}

	public LayerRegisterHotbar SetItem(Thing t)
	{
		return SetItem(t.trait.GetHotItem());
	}

	public void SetItem(Chara c)
	{
		SetItem(new HotItemChara
		{
			uid = c.uid
		});
	}

	public LayerRegisterHotbar SetItem(HotItem item)
	{
		WidgetHotbar.registeringItem = item;
		Show();
		return this;
	}

	public void Show()
	{
		WidgetHotbar.registering = true;
		foreach (Widget item in ELayer.ui.widgets.list)
		{
			WidgetHotbar widgetHotbar = item as WidgetHotbar;
			if (!(widgetHotbar == null) && widgetHotbar.CanRegisterItem && !widgetHotbar.hotbar.IsLocked)
			{
				if (!widgetHotbar.Visible)
				{
					widgetHotbar.ToggleVisible();
				}
				widgetHotbar.transform.SetParent(base.transform, worldPositionStays: false);
				hotbars.Add(widgetHotbar);
			}
		}
	}

	public ButtonHotItem GetButton()
	{
		foreach (WidgetHotbar hotbar in hotbars)
		{
			foreach (ButtonHotItem button in hotbar.buttons)
			{
				if (InputModuleEX.IsPointerOver(button))
				{
					return button;
				}
			}
		}
		return null;
	}

	public bool OnEndDrag()
	{
		ButtonHotItem button = GetButton();
		if ((bool)button)
		{
			button.onClick.Invoke();
			return true;
		}
		return false;
	}

	public override void OnKill()
	{
		foreach (WidgetHotbar hotbar in hotbars)
		{
			hotbar.transform.SetParent(ELayer.ui.widgets.transform, worldPositionStays: false);
		}
		foreach (Image cover in covers)
		{
			Object.DestroyImmediate(cover.gameObject);
		}
		WidgetHotbar.registering = false;
		WidgetHotbar.registeringItem = null;
	}
}
