using UnityEngine;

public class SceneTemplate : EScriptable
{
	public Color colorShadowOutside;

	public Color colorScreen;

	public static SceneTemplate Load(string id)
	{
		return Resources.Load<SceneTemplate>("Scene/Template/" + id.IsEmpty("Default"));
	}
}
