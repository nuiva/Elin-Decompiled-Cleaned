using System;
using UnityEngine;
using UnityEngine.UI;

public class BaseNotification : EClass
{
	public virtual Sprite Sprite
	{
		get
		{
			return null;
		}
	}

	public virtual int idSprite
	{
		get
		{
			return -1;
		}
	}

	public virtual bool Visible
	{
		get
		{
			return true;
		}
	}

	public virtual bool Interactable
	{
		get
		{
			return true;
		}
	}

	public virtual Action<UITooltip> onShowTooltip
	{
		get
		{
			return null;
		}
	}

	public virtual ItemNotice GetMold()
	{
		return null;
	}

	public virtual LayoutGroup GetLayoutGroup()
	{
		return null;
	}

	public void Refresh()
	{
		this.OnRefresh();
		if (this.text != this.lastText)
		{
			this.item.button.mainText.text = this.text;
			this.lastText = this.text;
			if (this.item.gameObject.activeInHierarchy)
			{
				this.item.button.RebuildLayout(true);
			}
		}
	}

	public virtual void OnClick()
	{
		EClass.ui.AddLayer<LayerJournal>();
	}

	public virtual void OnRefresh()
	{
	}

	public virtual bool ShouldRemove()
	{
		return false;
	}

	public virtual void OnInstantiate()
	{
	}

	public ItemNotice item;

	public string text;

	public string lastText;
}
