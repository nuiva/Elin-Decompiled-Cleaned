using System;
using UnityEngine;

public class BaseSticky : EClass
{
	public virtual string idSound
	{
		get
		{
			return "sticky";
		}
	}

	public virtual int idIcon
	{
		get
		{
			return 0;
		}
	}

	public virtual bool animate
	{
		get
		{
			return true;
		}
	}

	public virtual bool bold
	{
		get
		{
			return false;
		}
	}

	public virtual bool ShouldShow
	{
		get
		{
			return true;
		}
	}

	public virtual bool Removable
	{
		get
		{
			return false;
		}
	}

	public virtual bool ForceShowText
	{
		get
		{
			return false;
		}
	}

	public virtual bool AllowMultiple
	{
		get
		{
			return false;
		}
	}

	public virtual bool RemoveOnClick
	{
		get
		{
			return false;
		}
	}

	public virtual string idLang
	{
		get
		{
			return "";
		}
	}

	public virtual string GetText()
	{
		return this.idLang.lang();
	}

	public virtual void Refresh()
	{
	}

	public virtual void OnClick()
	{
	}

	public virtual void RefreshButton()
	{
		UIButton button = this.item.button1;
		this.SetText();
		if (this.idIcon == -1)
		{
			button.icon.SetActive(false);
		}
		else
		{
			button.icon.sprite = this.widget.icons[this.idIcon];
			button.icon.SetNativeSize();
		}
		button.onClick.AddListener(delegate()
		{
			this.OnClick();
			if (this.RemoveOnClick)
			{
				WidgetSticky.Instance._Remove(this);
			}
		});
		button.onRightClick = delegate()
		{
			if (this.Removable)
			{
				this.widget._Remove(this);
			}
			else
			{
				SE.Beep();
			}
			EInput.Consume(false, 1);
		};
		button.mainText.SetActive(this.widget.extra.showText || this.ForceShowText);
		button.RebuildLayout(false);
	}

	public virtual void SetText()
	{
		UIButton button = this.item.button1;
		button.mainText.fontStyle = (this.bold ? FontStyle.Bold : FontStyle.Normal);
		button.mainText.SetText(this.GetText());
	}

	public WidgetSticky widget
	{
		get
		{
			return WidgetSticky.Instance;
		}
	}

	public UIItem item;
}
