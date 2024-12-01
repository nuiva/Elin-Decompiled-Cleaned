using UnityEngine;

public class BaseSticky : EClass
{
	public UIItem item;

	public virtual string idSound => "sticky";

	public virtual int idIcon => 0;

	public virtual bool animate => true;

	public virtual bool bold => false;

	public virtual bool ShouldShow => true;

	public virtual bool Removable => false;

	public virtual bool ForceShowText => false;

	public virtual bool AllowMultiple => false;

	public virtual bool RemoveOnClick => false;

	public virtual string idLang => "";

	public WidgetSticky widget => WidgetSticky.Instance;

	public virtual string GetText()
	{
		return idLang.lang();
	}

	public virtual void Refresh()
	{
	}

	public virtual void OnClick()
	{
	}

	public virtual void RefreshButton()
	{
		UIButton button = item.button1;
		SetText();
		if (idIcon == -1)
		{
			button.icon.SetActive(enable: false);
		}
		else
		{
			button.icon.sprite = widget.icons[idIcon];
			button.icon.SetNativeSize();
		}
		button.onClick.AddListener(delegate
		{
			OnClick();
			if (RemoveOnClick)
			{
				WidgetSticky.Instance._Remove(this);
			}
		});
		button.onRightClick = delegate
		{
			if (Removable)
			{
				widget._Remove(this);
			}
			else
			{
				SE.Beep();
			}
			EInput.Consume();
		};
		button.mainText.SetActive(widget.extra.showText || ForceShowText);
		button.RebuildLayout();
	}

	public virtual void SetText()
	{
		UIButton button = item.button1;
		button.mainText.fontStyle = (bold ? FontStyle.Bold : FontStyle.Normal);
		button.mainText.SetText(GetText());
	}
}
