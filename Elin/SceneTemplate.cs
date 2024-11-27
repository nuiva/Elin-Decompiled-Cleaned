using System;
using UnityEngine;

public class SceneTemplate : EScriptable
{
	public static SceneTemplate Load(string id)
	{
		return Resources.Load<SceneTemplate>("Scene/Template/" + id.IsEmpty("Default"));
	}

	public Color colorShadowOutside;

	public Color colorScreen;
}
