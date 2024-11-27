using System;
using UnityEngine;

public class EUtil : EMono
{
	public void AddLayer(string s)
	{
		if (EMono.ui.contextMenu.currentMenu)
		{
			EMono.ui.contextMenu.currentMenu.Hide();
		}
		EMono.ui.AddLayer(s);
	}

	public void ExitGame()
	{
		if (EMono.ui.contextMenu.currentMenu)
		{
			EMono.ui.contextMenu.currentMenu.Hide();
		}
		EMono.game.Quit();
	}

	public void DestroyGameObject()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
