using DG.Tweening;
using UnityEngine;

public class ScreenFlash : ScriptableObject
{
	private static Tween tween;

	public static Color currentColor;

	public static Color clearColor = new Color(0f, 0f, 0f, 0f);

	public Color color;

	public AnimationCurve ease;

	public float duration;

	public static void Reset()
	{
		TweenUtil.KillTween(ref tween);
		currentColor = clearColor;
	}

	public static void Play(string id, float mtp = 1f)
	{
		ResourceCache.Load<ScreenFlash>("Media/Effect/ScreenEffect/Flash/" + id).Play(mtp);
	}

	public void Play(float mtp = 1f)
	{
		TweenUtil.KillTween(ref tween);
		tween = DOTween.To(() => currentColor, delegate(Color x)
		{
			currentColor = x;
		}, color * mtp, duration).SetEase(ease).OnComplete(delegate
		{
			Reset();
		});
	}
}
