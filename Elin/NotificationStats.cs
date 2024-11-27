using System;
using UnityEngine;

public class NotificationStats : BaseNotification
{
	public override bool Visible
	{
		get
		{
			return !this.text.IsEmpty();
		}
	}

	public override bool Interactable
	{
		get
		{
			return !this.stats().source.GetDetail().IsEmpty();
		}
	}

	public override Action<UITooltip> onShowTooltip
	{
		get
		{
			return delegate(UITooltip t)
			{
				this.stats().WriteNote(t.note, null);
			};
		}
	}

	public override void OnClick()
	{
		EClass.ui.AddLayer<LayerChara>().SetChara(EClass.pc);
	}

	public override Sprite Sprite
	{
		get
		{
			return this.stats().GetSprite();
		}
	}

	public override void OnRefresh()
	{
		BaseStats baseStats = this.stats();
		this.text = baseStats.GetText();
		this.item.button.mainText.color = baseStats.GetColor(this.item.button.skinRoot.GetButton().colorProf);
	}

	public Func<BaseStats> stats;
}
