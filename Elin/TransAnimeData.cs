using System;
using UnityEngine;

public class TransAnimeData : ScriptableObject
{
	public int TotalFrame
	{
		get
		{
			return this.frames * this.count;
		}
	}

	public void Awake()
	{
		if (this.generate)
		{
			this.Generate();
		}
	}

	public void Generate()
	{
		this.vectors = new Vector3[this.TotalFrame];
		for (int i = 0; i < this.count; i++)
		{
			for (int j = 0; j < this.frames; j++)
			{
				float time = 1f / (float)this.frames * (float)j;
				Vector3 vector = (new Vector3(this.curveX.Evaluate(time), this.curveY.Evaluate(time), this.curveZ.Evaluate(time)) + this.offset) * this.mtp;
				this.vectors[i * this.frames + j] = vector;
			}
		}
	}

	public Vector3 GetVector(int frame)
	{
		return this.vectors[frame];
	}

	private void OnValidate()
	{
		if (this.realtimeGenerate && this.generate)
		{
			this.Generate();
		}
	}

	public void SwapXY()
	{
		AnimationCurve animationCurve = this.curveX;
		this.curveX = this.curveY;
		this.curveY = animationCurve;
	}

	public int frames;

	public int count = 1;

	public float interval;

	public float mtp;

	public float randomMtp;

	public float randomDelay;

	public bool loop;

	public bool generate;

	public bool directional;

	public bool realtimeGenerate;

	public bool randomFlipX;

	public Vector3 offset;

	public Vector3[] vectors;

	public AnimationCurve curveX;

	public AnimationCurve curveY;

	public AnimationCurve curveZ;
}
