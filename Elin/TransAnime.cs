using System;
using UnityEngine;

public class TransAnime
{
	public TransAnime Init()
	{
		if (this.data.randomFlipX && Rand.rnd(2) == 0)
		{
			this.flipX = true;
		}
		if (this.data.randomDelay != 0f)
		{
			this.timer -= UnityEngine.Random.Range(0f, this.data.randomDelay);
		}
		return this;
	}

	public bool Update()
	{
		this.timer += RenderObject.gameDelta;
		if (this.timer > this.data.interval)
		{
			this.timer -= this.data.interval;
			this.frame++;
			if (this.frame >= this.data.TotalFrame)
			{
				if (!this.data.loop)
				{
					if (this.point != null && this.point.cell.detail != null)
					{
						this.point.cell.detail.anime = null;
						this.point.cell.TryDespawnDetail();
					}
					else if (this.renderer != null)
					{
						this.renderer.anime = null;
					}
					return true;
				}
				this.frame = 0;
			}
		}
		if (this.frame >= this.data.vectors.Length)
		{
			this.frame = this.data.vectors.Length - 1;
		}
		this.v = this.data.vectors[this.frame];
		if (this.data.randomMtp != 0f)
		{
			this.v *= UnityEngine.Random.Range(1f - this.data.randomMtp, 1f + this.data.randomMtp);
		}
		if (this.flipX)
		{
			this.v.x = this.v.x * -1f;
		}
		if (this.data.directional)
		{
			this.v.y = this.dest.y * this.v.x;
			this.v.x = this.dest.x * this.v.x;
			this.v.z = 0f;
		}
		if (this.renderer != null)
		{
			RenderObject.currentParam.x += this.v.x;
			RenderObject.currentParam.y += this.v.y;
			RenderObject.currentParam.z += this.v.z;
		}
		return false;
	}

	public Vector3 v;

	public Point point;

	public CardRenderer renderer;

	public TransAnimeData data;

	public int frame;

	public int startFrame;

	public float timer;

	public Vector3 dest;

	public bool animeBlock;

	public bool flipX;

	public bool drawBlock;
}
