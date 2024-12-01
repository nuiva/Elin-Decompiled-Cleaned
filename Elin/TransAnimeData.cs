using UnityEngine;

public class TransAnimeData : ScriptableObject
{
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

	public int TotalFrame => frames * count;

	public void Awake()
	{
		if (generate)
		{
			Generate();
		}
	}

	public void Generate()
	{
		vectors = new Vector3[TotalFrame];
		for (int i = 0; i < count; i++)
		{
			for (int j = 0; j < frames; j++)
			{
				float time = 1f / (float)frames * (float)j;
				Vector3 vector = (new Vector3(curveX.Evaluate(time), curveY.Evaluate(time), curveZ.Evaluate(time)) + offset) * mtp;
				vectors[i * frames + j] = vector;
			}
		}
	}

	public Vector3 GetVector(int frame)
	{
		return vectors[frame];
	}

	private void OnValidate()
	{
		if (realtimeGenerate && generate)
		{
			Generate();
		}
	}

	public void SwapXY()
	{
		AnimationCurve animationCurve = curveX;
		curveX = curveY;
		curveY = animationCurve;
	}
}
