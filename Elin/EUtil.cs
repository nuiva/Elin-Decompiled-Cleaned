using UnityEngine;

public class EUtil : EMono
{
	public void AddLayer(string s)
	{
		if ((bool)EMono.ui.contextMenu.currentMenu)
		{
			EMono.ui.contextMenu.currentMenu.Hide();
		}
		EMono.ui.AddLayer(s);
	}

	public void ExitGame()
	{
		if ((bool)EMono.ui.contextMenu.currentMenu)
		{
			EMono.ui.contextMenu.currentMenu.Hide();
		}
		EMono.game.Quit();
	}

	public void DestroyGameObject()
	{
		Object.Destroy(base.gameObject);
	}
}
