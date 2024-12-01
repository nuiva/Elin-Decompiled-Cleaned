using Assets.Resources.Scene.Profile.Global;
using UnityEngine;

public class SceneProfile : ScriptableObject
{
	public SceneGlobalProfile global;

	public OverlayProfile overlay;

	public SceneLightProfile light;

	public SceneColorProfile color;

	public Material matOverlay;

	public Material matGrading;

	public static SceneProfile Load(string id)
	{
		return ResourceCache.Load<SceneProfile>("Scene/Profile/SceneProfile_" + id);
	}
}
