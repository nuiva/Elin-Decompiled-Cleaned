using System;
using UnityEngine.UI;

public class NotificationBuff : NotificationCondition
{
	public override int idSprite
	{
		get
		{
			return 0;
		}
	}

	public override ItemNotice GetMold()
	{
		return WidgetStats.Instance.moldBuff;
	}

	public override LayoutGroup GetLayoutGroup()
	{
		return WidgetStats.Instance.layout2;
	}

	public override bool Interactable
	{
		get
		{
			return true;
		}
	}

	public override void OnRefresh()
	{
		if (this.item.button.icon.sprite == EClass.core.refs.spriteDefaultCondition)
		{
			this.OnInstantiate();
		}
		this.text = this.condition.GetText() + (EClass.debug.showExtra ? (" " + this.condition.value.ToString()) : "");
		this.item.textDuration.SetText(this.condition.TextDuration);
	}

	public override void OnInstantiate()
	{
		this.item.button.icon.sprite = this.condition.GetSprite();
		this.item.button.icon.color = this.condition.GetSpriteColor();
		this.item.button.icon.SetNativeSize();
	}
}
