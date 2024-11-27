using System;
using UnityEngine;

public class NotificationCondition : BaseNotification
{
	public override bool Visible
	{
		get
		{
			return !this.text.IsEmpty();
		}
	}

	public override bool ShouldRemove()
	{
		return this.condition.IsKilled || (EClass.core.IsGameStarted && !EClass.pc.conditions.Contains(this.condition));
	}

	public override bool Interactable
	{
		get
		{
			return !this.condition.source.GetDetail().IsEmpty();
		}
	}

	public override Action<UITooltip> onShowTooltip
	{
		get
		{
			return delegate(UITooltip t)
			{
				this.condition.WriteNote(t.note, null);
			};
		}
	}

	public override Sprite Sprite
	{
		get
		{
			return this.condition.GetSprite();
		}
	}

	public override void OnClick()
	{
		if (this.condition.CanManualRemove)
		{
			SE.Trash();
			this.condition.Kill(false);
			WidgetStats.RefreshAll();
			if (WidgetStats.Instance)
			{
				WidgetStats.Instance.layout.RebuildLayout(false);
				return;
			}
		}
		else
		{
			SE.Beep();
		}
	}

	public override void OnRefresh()
	{
		this.text = this.condition.GetText() + (EClass.debug.showExtra ? (" " + this.condition.value.ToString()) : "");
		this.item.button.mainText.color = this.condition.GetColor(this.item.button.skinRoot.GetButton().colorProf);
	}

	public Condition condition;
}
