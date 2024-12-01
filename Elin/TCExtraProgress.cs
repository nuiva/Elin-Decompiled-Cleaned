using UnityEngine;
using UnityEngine.UI;

public class TCExtraProgress : TCExtraUI
{
	public enum ProgressType
	{
		Gene
	}

	public ProgressType progressType;

	public GameObject goBar;

	public Image bar;

	public Image bgBar;

	public UIText textProgress;

	public float refreshInterval;

	private float timer;

	public override void OnSetOwner()
	{
		Refresh();
	}

	private void Update()
	{
		timer -= Core.delta;
		if (timer < 0f)
		{
			timer += refreshInterval;
			Refresh();
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
		goBar.SetActive(flag);
		if (flag)
		{
			float progress = traitGeneMachine.GetProgress();
			bar.rectTransform.sizeDelta = new Vector2(progress * bgBar.rectTransform.sizeDelta.x, bgBar.rectTransform.sizeDelta.y);
			if ((bool)textProgress)
			{
				textProgress.text = traitGeneMachine.GetProgressText();
			}
		}
	}
}
