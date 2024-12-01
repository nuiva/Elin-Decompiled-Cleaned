using UnityEngine;

public class TCSimpleText : EMono
{
	public TextMesh tm;

	public SpriteRenderer sr;

	public int num;

	public static TCSimpleText Spawn()
	{
		return PoolManager.Spawn(EMono.core.refs.tcs.simpleText, EMono.screen);
	}

	public static TCSimpleText SpawnIcon(Sprite sprite = null)
	{
		TCSimpleText tCSimpleText = PoolManager.Spawn(EMono.core.refs.tcs.simpleTextIcon, EMono.screen);
		if ((bool)tCSimpleText)
		{
			tCSimpleText.sr.sprite = sprite;
		}
		return tCSimpleText;
	}
}
