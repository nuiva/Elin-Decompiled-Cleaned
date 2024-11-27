using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class EmbarkActor : EMono
{
	private void Update()
	{
		if (this.isDestroying)
		{
			return;
		}
		if (!this.crystal)
		{
			this.show = !EMono.core.IsGameStarted;
		}
		if (this.show)
		{
			this.sr.DOFade(this.targetFade, this.fadeTime);
			this.skyLevel += Core.delta * this.speedSky;
			if (this.skyLevel > this.targetFade)
			{
				this.skyLevel = this.targetFade;
			}
			this.anim.DOPlay();
		}
		else
		{
			if (!this.crystal)
			{
				this.sr.DOFade(0f, this.fadeTime);
			}
			this.skyLevel -= Core.delta * this.speedSky;
			if (this.skyLevel < 0f)
			{
				this.skyLevel = 0f;
			}
			this.anim.DOPause();
		}
		this.grading.material.SetFloat("_SkyLevel", this.skyLevel);
		this.grading.material.SetFloat("_ViewHeight", 20f);
		this.grading.material.SetVector("_ScreenPos", Vector3.zero);
		this.grading.material.SetVector("_Position", this.cam.transform.position * this.speed);
		this.grading.material.SetVector("_Offset", Vector3.zero);
		this.grading.SetGrading();
	}

	public void Hide()
	{
		this.isDestroying = true;
		this.show = false;
		this.ps.emission.enabled = false;
		DOTween.To(() => this.tiltShift.blurArea, delegate(float x)
		{
			this.tiltShift.blurArea = x;
		}, 0f, this.fadeTime);
		this.sr2.DOFade(0f, this.fadeTime - 0.1f);
		this.sr.DOFade(0f, this.fadeTime).OnComplete(delegate
		{
			if (this.crystal)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		});
	}

	public Camera cam;

	public TiltShift tiltShift;

	public ScreenGrading grading;

	public SpriteRenderer sr;

	public SpriteRenderer sr2;

	public ParticleSystem ps;

	public float speed;

	public float speedSky;

	public float skyLevel = 1f;

	public float fadeTime;

	public float targetFade = 1f;

	public bool show;

	public bool crystal;

	public bool isDestroying;

	public DOTweenAnimation anim;
}
