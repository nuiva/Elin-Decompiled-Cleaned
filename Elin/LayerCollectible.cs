using System;
using UnityEngine;
using UnityEngine.UI;

public class LayerCollectible : ELayer
{
	public Hoard hoard
	{
		get
		{
			return ELayer.player.hoard;
		}
	}

	public override void OnAfterInit()
	{
		if (this.hoard.items.Count == 0)
		{
			this.hoard.Add("gold", 1, false);
		}
		this.Refresh();
	}

	public void Refresh()
	{
		ELayer.ui.GetLayer<LayerHoard>(false);
		this.list.callbacks = new UIList.Callback<Hoard.Item, UIButton>
		{
			onClick = delegate(Hoard.Item a, UIButton b)
			{
				this.selected = b;
				if (this.onClick != null)
				{
					if (this.onClick(a))
					{
						this.Close();
					}
					return;
				}
				UIContextMenu uicontextMenu = ELayer.ui.contextMenu.Create("ContextHoard", true);
				int max = a.num;
				if (max > 1 && a.IsUnique)
				{
					max = 1;
				}
				uicontextMenu.AddSlider("display", (float n) => n.ToString() + " / " + max.ToString(), (float)a.show, delegate(float v)
				{
					a.show = (int)v;
					this.SetButton(a, b);
					this.RefreshInfo(b, true);
				}, 0f, (float)max, true, false, false);
				uicontextMenu.AddToggle("displayRandom", a.random, delegate(bool on)
				{
					a.random = on;
				});
				uicontextMenu.Show(EInput.uiMousePosition + this.contextFix);
			},
			onInstantiate = delegate(Hoard.Item a, UIButton b)
			{
				this.SetButton(a, b);
			}
		};
		foreach (SourceCollectible.Row row in ELayer.sources.collectibles.rows)
		{
			if (this.hoard.items.ContainsKey(row.id))
			{
				this.list.Add(this.hoard.items[row.id]);
			}
		}
		this.list.Refresh(false);
		this.selected = (this.list.buttons[0].component as UIButton);
		this.RefreshInfo(this.selected, false);
	}

	private void Update()
	{
		UIButton componentOf = InputModuleEX.GetComponentOf<UIButton>();
		if (componentOf && componentOf.refObj is Hoard.Item)
		{
			this.RefreshInfo(componentOf, false);
			return;
		}
		this.RefreshInfo(this.selected, false);
	}

	public void SetButton(Hoard.Item a, UIButton b)
	{
		string text = a.num.ToString() ?? "";
		if (a.num != a.show)
		{
			text = text + " (" + a.show.ToString() + ")";
		}
		b.mainText.text = text;
		b.icon.sprite = this.hoard.GetSprite(a.id);
		b.icon.SetNativeSize();
		b.refObj = a;
	}

	public void RefreshInfo(UIButton button, bool force = false)
	{
		if (!force && (this.lastShown == button || !button))
		{
			return;
		}
		Hoard.Item item = button.refObj as Hoard.Item;
		string text = "collectibleTitle".lang(item.Source.GetName(), item.num.ToString() ?? "", item.show.ToString() ?? "", null, null);
		if (item.IsUnique)
		{
			text += " UNIQUE";
		}
		this.textName.text = text;
		this.textDetail.text = item.Source.GetText("detail", true).IsEmpty("none".lang());
		this.imageItem.sprite = this.hoard.GetSprite(item.id);
		this.imageItem.SetNativeSize();
		this.lastShown = button;
	}

	public UIList list;

	public UIText textName;

	public UIText textDetail;

	public UIButton selected;

	public UIButton lastShown;

	public Image imageItem;

	public Vector3 contextFix;

	public Func<Hoard.Item, bool> onClick;
}
