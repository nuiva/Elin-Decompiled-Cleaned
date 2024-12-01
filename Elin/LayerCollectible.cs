using System;
using UnityEngine;
using UnityEngine.UI;

public class LayerCollectible : ELayer
{
	public UIList list;

	public UIText textName;

	public UIText textDetail;

	public UIButton selected;

	public UIButton lastShown;

	public Image imageItem;

	public Vector3 contextFix;

	public Func<Hoard.Item, bool> onClick;

	public Hoard hoard => ELayer.player.hoard;

	public override void OnAfterInit()
	{
		if (hoard.items.Count == 0)
		{
			hoard.Add("gold", 1);
		}
		Refresh();
	}

	public void Refresh()
	{
		ELayer.ui.GetLayer<LayerHoard>();
		list.callbacks = new UIList.Callback<Hoard.Item, UIButton>
		{
			onClick = delegate(Hoard.Item a, UIButton b)
			{
				selected = b;
				if (onClick != null)
				{
					if (onClick(a))
					{
						Close();
					}
				}
				else
				{
					UIContextMenu uIContextMenu = ELayer.ui.contextMenu.Create("ContextHoard");
					int max = a.num;
					if (max > 1 && a.IsUnique)
					{
						max = 1;
					}
					uIContextMenu.AddSlider("display", (float n) => n + " / " + max, a.show, delegate(float v)
					{
						a.show = (int)v;
						SetButton(a, b);
						RefreshInfo(b, force: true);
					}, 0f, max, isInt: true, hideOther: false);
					uIContextMenu.AddToggle("displayRandom", a.random, delegate(bool on)
					{
						a.random = on;
					});
					uIContextMenu.Show(EInput.uiMousePosition + contextFix);
				}
			},
			onInstantiate = delegate(Hoard.Item a, UIButton b)
			{
				SetButton(a, b);
			}
		};
		foreach (SourceCollectible.Row row in ELayer.sources.collectibles.rows)
		{
			if (hoard.items.ContainsKey(row.id))
			{
				list.Add(hoard.items[row.id]);
			}
		}
		list.Refresh();
		selected = list.buttons[0].component as UIButton;
		RefreshInfo(selected);
	}

	private void Update()
	{
		UIButton componentOf = InputModuleEX.GetComponentOf<UIButton>();
		if ((bool)componentOf && componentOf.refObj is Hoard.Item)
		{
			RefreshInfo(componentOf);
		}
		else
		{
			RefreshInfo(selected);
		}
	}

	public void SetButton(Hoard.Item a, UIButton b)
	{
		string text = a.num.ToString() ?? "";
		if (a.num != a.show)
		{
			text = text + " (" + a.show + ")";
		}
		b.mainText.text = text;
		b.icon.sprite = hoard.GetSprite(a.id);
		b.icon.SetNativeSize();
		b.refObj = a;
	}

	public void RefreshInfo(UIButton button, bool force = false)
	{
		if (force || (!(lastShown == button) && (bool)button))
		{
			Hoard.Item item = button.refObj as Hoard.Item;
			string text = "collectibleTitle".lang(item.Source.GetName(), item.num.ToString() ?? "", item.show.ToString() ?? "");
			if (item.IsUnique)
			{
				text += " UNIQUE";
			}
			textName.text = text;
			textDetail.text = item.Source.GetText("detail", returnNull: true).IsEmpty("none".lang());
			imageItem.sprite = hoard.GetSprite(item.id);
			imageItem.SetNativeSize();
			lastShown = button;
		}
	}
}
