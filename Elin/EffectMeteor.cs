using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class EffectMeteor : Effect
{
	public override void OnPlay()
	{
		this.sr.enabled = true;
		this.aniExplosion.SetActive(false);
		this.destV = this.fromV;
		this.fromV += this.startPos + this.startPos.Random() * 0.2f;
		base.transform.position = this.fromV;
		this.moveTween = base.transform.DOMove(this.destV, this.time, false).SetEase(Ease.Linear).SetDelay(this.startDelay).OnComplete(delegate
		{
			this.sr.enabled = false;
			this.aniExplosion.SetActive(true);
			this.destPos.Animate(AnimeID.Dig, true);
			Action onComplete = this.onComplete;
			if (onComplete != null)
			{
				onComplete();
			}
			EMono.Sound.Play("explode", this.destV, 1f);
			Shaker.ShakeCam("meteor", 1f);
		});
	}

	public static void Create(Point center, int radius, int count, Action<int, Point> onComplete)
	{
		List<Point> list = new List<Point>();
		for (int i = 0; i < count; i++)
		{
			Point p = center.Copy();
			Effect effect = Effect.Get("meteor");
			effect.startDelay = Rand.Range(0f, 0.5f);
			if (radius > 0)
			{
				int num = 0;
				if (num < 1000)
				{
					Point randomSurface = EMono._map.GetRandomSurface(center.x, center.z, radius, true, false);
					foreach (Point obj in list)
					{
						randomSurface.Equals(obj);
					}
					p.Set(randomSurface);
					list.Add(randomSurface);
				}
			}
			int _i = i;
			effect.onComplete = delegate()
			{
				onComplete(_i, p);
			};
			effect.Play(p, 0f, null, null);
		}
	}

	public Animator aniExplosion;

	public Vector3 startPos;

	public float time;
}
