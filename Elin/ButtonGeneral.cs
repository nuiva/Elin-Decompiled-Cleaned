using System;
using UnityEngine.UI;

public class ButtonGeneral : UIButton
{
	public void SetCard(Card c, FontColor color = FontColor.Ignore)
	{
		if (this.portrait)
		{
			this.portrait.SetChara(c.Chara, null);
		}
		else
		{
			c.SetImage(this.icon);
		}
		this.mainText.SetText(c.Name, color);
	}

	public Image imageFrame;

	public Portrait portrait;
}
