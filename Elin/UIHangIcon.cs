using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIHangIcon : EMono
{
	private void Awake()
	{
		if (!this.image && this.button)
		{
			this.image = this.button.image;
		}
		this.original = this.image.sprite;
		if (this.button && !this.rightClick)
		{
			this.button.onClick.AddListener(new UnityAction(this.OnClick));
		}
		this.Refresh();
	}

	public void OnClickCorner()
	{
		SE.Play("Ambience/Random/windchime1");
	}

	public void OnClick()
	{
		if (!EMono.core.IsGameStarted)
		{
			return;
		}
		ActionMode.DefaultMode.Activate(true, false);
		if (EMono.ui.contextMenu.isActive)
		{
			EMono.ui.contextMenu.currentMenu.Hide();
			EInput.rightMouse.Consume();
		}
		LayerCollectible layerCollectible = EMono.ui.ToggleLayer<LayerCollectible>(null);
		if (layerCollectible == null)
		{
			return;
		}
		EMono.ui.hud.hint.Show("h_hang", true);
		layerCollectible.onClick = delegate(Hoard.Item a)
		{
			EMono.player.hangIcons[this.id] = a.id;
			if (this != null && base.gameObject != null)
			{
				this.Refresh();
			}
			return true;
		};
	}

	public void Refresh()
	{
		if (!Core.Instance)
		{
			return;
		}
		if (!EMono.core.IsGameStarted || (this.windowCorner && !EMono.core.config.ui.cornerHoard))
		{
			this.image.sprite = (UIHangIcon.lastCorner = EMono.core.refs.spritesCorner.NextItem(UIHangIcon.lastCorner));
		}
		else
		{
			string text;
			if (!this.windowCorner)
			{
				text = EMono.player.hangIcons.TryGetValue(this.id, null);
			}
			else
			{
				Hoard.Item item = EMono.player.hoard.items.RandomItem<string, Hoard.Item>();
				text = ((item != null) ? item.id : null);
			}
			string str = text;
			if (!str.IsEmpty())
			{
				this.image.sprite = EMono.player.hoard.GetSprite(str);
			}
		}
		this.image.SetNativeSize();
	}

	public Image image;

	public UIButton button;

	public string id;

	public bool rightClick;

	public bool windowCorner;

	private Sprite original;

	public static Sprite lastCorner;
}
