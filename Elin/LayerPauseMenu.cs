public class LayerPauseMenu : ELayer
{
	public void OnClickTitle()
	{
		ELayer.game.GotoTitle();
	}

	public void OnClickExit()
	{
		ELayer.game.Quit();
	}

	public void OnClickSave()
	{
		ELayer.game.Save();
	}
}
