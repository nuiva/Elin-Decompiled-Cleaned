using System;
using UnityEngine;

public class VFX : EMono
{
	public void OnChangeHour()
	{
		int maxParticles = (int)(this.timeCurve.Evaluate(EMono.scene.timeRatio) * (float)this.baseParticleCount);
		ParticleSystem.MainModule main = this.ps.main;
		main.maxParticles = maxParticles;
		if (this.useSunColor)
		{
			main.startColor = EMono.scene.profile.color.sun.Evaluate(EMono.scene.timeRatio);
		}
		if (this.dbg)
		{
			Debug.Log(EMono.scene.timeRatio.ToString() + "/" + this.ps.main.maxParticles.ToString());
		}
	}

	public ParticleSystem ps;

	public AnimationCurve timeCurve;

	public int baseParticleCount;

	public bool useSunColor;

	public bool dbg;
}
