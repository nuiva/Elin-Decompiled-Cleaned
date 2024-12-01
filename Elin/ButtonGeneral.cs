using UnityEngine.UI;

public class ButtonGeneral : UIButton
{
	public Image imageFrame;

	public Portrait portrait;

	public void SetCard(Card c, FontColor color = FontColor.Ignore)
	{
		if ((bool)portrait)
		{
			portrait.SetChara(c.Chara);
		}
		else
		{
			c.SetImage(icon);
		}
		mainText.SetText(c.Name, color);
	}
}
