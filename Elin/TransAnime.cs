using UnityEngine;

public class TransAnime
{
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

	public TransAnime Init()
	{
		if (data.randomFlipX && Rand.rnd(2) == 0)
		{
			flipX = true;
		}
		if (data.randomDelay != 0f)
		{
			timer -= Random.Range(0f, data.randomDelay);
		}
		return this;
	}

	public bool Update()
	{
		timer += RenderObject.gameDelta;
		if (timer > data.interval)
		{
			timer -= data.interval;
			frame++;
			if (frame >= data.TotalFrame)
			{
				if (!data.loop)
				{
					if (point != null && point.cell.detail != null)
					{
						point.cell.detail.anime = null;
						point.cell.TryDespawnDetail();
					}
					else if (renderer != null)
					{
						renderer.anime = null;
					}
					return true;
				}
				frame = 0;
			}
		}
		if (frame >= data.vectors.Length)
		{
			frame = data.vectors.Length - 1;
		}
		v = data.vectors[frame];
		if (data.randomMtp != 0f)
		{
			v *= Random.Range(1f - data.randomMtp, 1f + data.randomMtp);
		}
		if (flipX)
		{
			v.x *= -1f;
		}
		if (data.directional)
		{
			v.y = dest.y * v.x;
			v.x = dest.x * v.x;
			v.z = 0f;
		}
		if (renderer != null)
		{
			RenderObject.currentParam.x += v.x;
			RenderObject.currentParam.y += v.y;
			RenderObject.currentParam.z += v.z;
		}
		return false;
	}
}
