using UnityEngine;

public class PCOrbit : EMono
{
	public Transform crystal;

	public SpriteRenderer light;

	public int emitFoot;

	public int emitSmoke;

	public Vector3 footPos;

	public Vector3 smokePos;

	public float smokeWalkAlpha;

	public void OnChangeMin()
	{
		Color white = Color.white;
		white.a = EMono.scene.profile.light.orbitAlphaCurve.Evaluate(EMono.scene.timeRatio);
		light.color = white;
	}
}
