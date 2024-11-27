using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class ScreenFlash : ScriptableObject
{
	public static void Reset()
	{
		TweenUtil.KillTween(ref ScreenFlash.tween, false);
		ScreenFlash.currentColor = ScreenFlash.clearColor;
	}

	public static void Play(string id, float mtp = 1f)
	{
		ResourceCache.Load<ScreenFlash>("Media/Effect/ScreenEffect/Flash/" + id).Play(mtp);
	}

	public void Play(float mtp = 1f)
	{
		TweenUtil.KillTween(ref ScreenFlash.tween, false);
		ScreenFlash.tween = DOTween.To(() => ScreenFlash.currentColor, delegate(Color x)
		{
			ScreenFlash.currentColor = x;
		}, this.color * mtp, this.duration).SetEase(this.ease).OnComplete(delegate
		{
			ScreenFlash.Reset();
		});
	}

	private static Tween tween;

	public static Color currentColor;

	public static Color clearColor = new Color(0f, 0f, 0f, 0f);

	public Color color;

	public AnimationCurve ease;

	public float duration;
}
