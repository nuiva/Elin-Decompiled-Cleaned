using UnityEngine;
using UnityEngine.UI;

public class ButtonGridExt : MonoBehaviour
{
	public Image imageBar;

	public void Refresh(Card c)
	{
		float num = (float)c.hp / (float)c.MaxHP * 0.9f;
		imageBar.fillAmount = 0.1f + num;
		imageBar.color = EClass.Colors.Dark.gradientTool.Evaluate(num);
	}
}
