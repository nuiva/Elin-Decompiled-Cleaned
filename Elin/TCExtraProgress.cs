using System;
using UnityEngine;
using UnityEngine.UI;

public class TCExtraProgress : TCExtraUI
{
	public override void OnSetOwner()
	{
		this.Refresh();
	}

	private void Update()
	{
		this.timer -= Core.delta;
		if (this.timer < 0f)
		{
			this.timer += this.refreshInterval;
			this.Refresh();
		}
	}

	public void Refresh()
	{
		if (base.owner == null)
		{
			return;
		}
		TraitGeneMachine traitGeneMachine = base.owner.trait as TraitGeneMachine;
		bool flag = base.owner.isOn && base.owner.IsInstalled && traitGeneMachine.IsTargetUsingGene();
		this.goBar.SetActive(flag);
		if (flag)
		{
			float progress = traitGeneMachine.GetProgress();
			this.bar.rectTransform.sizeDelta = new Vector2(progress * this.bgBar.rectTransform.sizeDelta.x, this.bgBar.rectTransform.sizeDelta.y);
			if (this.textProgress)
			{
				this.textProgress.text = traitGeneMachine.GetProgressText();
			}
		}
	}

	public TCExtraProgress.ProgressType progressType;

	public GameObject goBar;

	public Image bar;

	public Image bgBar;

	public UIText textProgress;

	public float refreshInterval;

	private float timer;

	public enum ProgressType
	{
		Gene
	}
}
