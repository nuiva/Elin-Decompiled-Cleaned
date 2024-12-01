using DG.Tweening;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class EmbarkActor : EMono
{
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

	private void Update()
	{
		if (isDestroying)
		{
			return;
		}
		if (!crystal)
		{
			show = !EMono.core.IsGameStarted;
		}
		if (show)
		{
			sr.DOFade(targetFade, fadeTime);
			skyLevel += Core.delta * speedSky;
			if (skyLevel > targetFade)
			{
				skyLevel = targetFade;
			}
			anim.DOPlay();
		}
		else
		{
			if (!crystal)
			{
				sr.DOFade(0f, fadeTime);
			}
			skyLevel -= Core.delta * speedSky;
			if (skyLevel < 0f)
			{
				skyLevel = 0f;
			}
			anim.DOPause();
		}
		grading.material.SetFloat("_SkyLevel", skyLevel);
		grading.material.SetFloat("_ViewHeight", 20f);
		grading.material.SetVector("_ScreenPos", Vector3.zero);
		grading.material.SetVector("_Position", cam.transform.position * speed);
		grading.material.SetVector("_Offset", Vector3.zero);
		grading.SetGrading();
	}

	public void Hide()
	{
		isDestroying = true;
		show = false;
		ParticleSystem.EmissionModule emission = ps.emission;
		emission.enabled = false;
		DOTween.To(() => tiltShift.blurArea, delegate(float x)
		{
			tiltShift.blurArea = x;
		}, 0f, fadeTime);
		sr2.DOFade(0f, fadeTime - 0.1f);
		sr.DOFade(0f, fadeTime).OnComplete(delegate
		{
			if (crystal)
			{
				Object.Destroy(base.gameObject);
			}
		});
	}
}
