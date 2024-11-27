using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseWidgetNotice : Widget
{
	public sealed override void OnActivate()
	{
		if (!this.mold)
		{
			this.mold = this.layout.CreateMold(null);
		}
		this._OnActivate();
		this.RefreshLayout();
		this.layout.RebuildLayout(true);
		if (this.layout2)
		{
			this.layout2.RebuildLayout(true);
		}
		this.activating = false;
		this._RefreshAll();
	}

	public virtual void _OnActivate()
	{
	}

	private void OnEnable()
	{
		base.InvokeRepeating("_RefreshAll", 0.1f, 0.2f);
	}

	private void OnDisable()
	{
		base.CancelInvoke();
	}

	public void _RefreshAll()
	{
		this.OnRefresh();
		bool rebuild = false;
		using (List<BaseNotification>.Enumerator enumerator = this.list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				BaseNotification n = enumerator.Current;
				n.Refresh();
				n.item.SetActive(n.Visible, delegate(bool enabled)
				{
					if (enabled)
					{
						n.item.button.RebuildLayout(true);
					}
					rebuild = true;
				});
			}
		}
		if (rebuild)
		{
			this.layout.RebuildLayout(false);
			if (this.layout2)
			{
				this.layout2.RebuildLayout(true);
			}
		}
	}

	public virtual void OnRefresh()
	{
	}

	public ItemNotice Add(BaseNotification n, Transform parent = null)
	{
		this.list.Add(n);
		LayoutGroup layoutGroup = n.GetLayoutGroup() ?? this.layout;
		ItemNotice itemNotice = Util.Instantiate<ItemNotice>(n.GetMold() ?? this.mold, parent ?? layoutGroup.transform);
		itemNotice.button.onClick.AddListener(delegate()
		{
			if (EMono.scene.actionMode == ActionMode.NoMap)
			{
				BaseCore.Instance.WaitForEndOfFrame(new Action(n.OnClick));
				return;
			}
			n.OnClick();
		});
		Sprite sprite = n.Sprite;
		if (n.idSprite != -1)
		{
			sprite = this.sprites[n.idSprite];
		}
		if (sprite)
		{
			itemNotice.button.icon.sprite = sprite;
		}
		itemNotice.button.image.raycastTarget = n.Interactable;
		itemNotice.button.tooltip.enable = (n.onShowTooltip != null);
		itemNotice.button.tooltip.onShowTooltip = n.onShowTooltip;
		n.item = itemNotice;
		n.Refresh();
		n.OnInstantiate();
		if (!this.activating)
		{
			layoutGroup.RebuildLayout(true);
		}
		return itemNotice;
	}

	public void Remove(BaseNotification n)
	{
		this.list.Remove(n);
		if (n.item)
		{
			UnityEngine.Object.DestroyImmediate(n.item.gameObject);
		}
	}

	public override void OnFlip()
	{
		this.layout.childAlignment = (this.flip ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft);
		this.Rect().pivot = new Vector2((float)(this.flip ? 1 : 0), this.Rect().pivot.y);
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		m.AddChild("style").AddSlider("toggleButtonBG", (float a) => a.ToString() ?? "", (float)base.config.skin.button, delegate(float a)
		{
			base.config.skin.button = (int)a;
			this.ApplySkin();
		}, 0f, (float)(base.config.skin.Skin.buttons.Count - 1), true, true, false);
		base.SetBaseContextMenu(m);
	}

	public void RefreshLayout()
	{
		SkinAsset_Button button = base.GetComponent<SkinRoot>().Config.Button;
		int y = button.size.y;
		foreach (BaseNotification baseNotification in this.list)
		{
			RectTransform rectTransform = baseNotification.item.Rect();
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, (float)y);
		}
		this.layout.spacing = (float)button.spacing.y;
	}

	public override void OnApplySkin()
	{
		this.RefreshLayout();
	}

	public Sprite[] sprites;

	public List<BaseNotification> list = new List<BaseNotification>();

	public VerticalLayoutGroup layout;

	public VerticalLayoutGroup layout2;

	public ItemNotice mold;

	private bool activating = true;
}
