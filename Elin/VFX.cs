using UnityEngine;

public class VFX : EMono
{
	public ParticleSystem ps;

	public AnimationCurve timeCurve;

	public int baseParticleCount;

	public bool useSunColor;

	public bool dbg;

	public void OnChangeHour()
	{
		int maxParticles = (int)(timeCurve.Evaluate(EMono.scene.timeRatio) * (float)baseParticleCount);
		ParticleSystem.MainModule main = ps.main;
		main.maxParticles = maxParticles;
		if (useSunColor)
		{
			main.startColor = EMono.scene.profile.color.sun.Evaluate(EMono.scene.timeRatio);
		}
		if (dbg)
		{
			Debug.Log(EMono.scene.timeRatio + "/" + ps.main.maxParticles);
		}
	}
}
