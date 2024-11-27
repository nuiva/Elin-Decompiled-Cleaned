using System;
using DG.Tweening;
using UnityEngine;

public class DramaPropOnev : DramaProp
{
	public override void OnEnter()
	{
		this.sr.DOFade(1f, 2f).From(0f, true, false);
		this.mat.DOFade(1f, 2f).From(0f, true, false);
	}

	public override void OnLeave()
	{
		this.sr.DOFade(0f, 2f);
		this.mat.DOFade(0f, 2f);
	}

	public Material mat;
}
