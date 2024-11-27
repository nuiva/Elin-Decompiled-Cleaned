using System;

public class LayerPauseMenu : ELayer
{
	public void OnClickTitle()
	{
		ELayer.game.GotoTitle(true);
	}

	public void OnClickExit()
	{
		ELayer.game.Quit();
	}

	public void OnClickSave()
	{
		ELayer.game.Save(false, null, false);
	}
}
