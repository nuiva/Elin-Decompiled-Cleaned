using DG.Tweening;
using UnityEngine;

public class DramaPropOnev : DramaProp
{
	public Material mat;

	public override void OnEnter()
	{
		sr.DOFade(1f, 2f).From(0f);
		mat.DOFade(1f, 2f).From(0f);
	}

	public override void OnLeave()
	{
		sr.DOFade(0f, 2f);
		mat.DOFade(0f, 2f);
	}
}
