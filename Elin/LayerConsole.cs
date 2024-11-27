using System;
using UnityEngine;

public class LayerConsole : ELayer
{
	public ReflexConsole console
	{
		get
		{
			return ELayer.ui.console;
		}
	}

	public override void OnAfterInit()
	{
		this.console.Open();
	}

	public override void OnUpdateInput()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			this.Close();
		}
	}

	public override void OnKill()
	{
		this.console.Close();
	}
}
