using System;
using UnityEngine;

public class TCSimpleText : EMono
{
	public static TCSimpleText Spawn()
	{
		return PoolManager.Spawn<TCSimpleText>(EMono.core.refs.tcs.simpleText, EMono.screen);
	}

	public static TCSimpleText SpawnIcon(Sprite sprite = null)
	{
		TCSimpleText tcsimpleText = PoolManager.Spawn<TCSimpleText>(EMono.core.refs.tcs.simpleTextIcon, EMono.screen);
		if (tcsimpleText)
		{
			tcsimpleText.sr.sprite = sprite;
		}
		return tcsimpleText;
	}

	public TextMesh tm;

	public SpriteRenderer sr;

	public int num;
}
