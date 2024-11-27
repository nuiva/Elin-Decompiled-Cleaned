using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGridExt : MonoBehaviour
{
	public void Refresh(Card c)
	{
		float num = (float)c.hp / (float)c.MaxHP * 0.9f;
		this.imageBar.fillAmount = 0.1f + num;
		this.imageBar.color = EClass.Colors.Dark.gradientTool.Evaluate(num);
	}

	public Image imageBar;
}
