using UnityEngine;

public class TCState : TC
{
	public SpriteRenderer sr;

	public MultiSprite sprites;

	private int index;

	public override Vector3 FixPos => TC._setting.statePos;

	public void SetSprite(MultiSprite _sprites)
	{
		if (sprites != _sprites)
		{
			CancelInvoke();
			sprites = _sprites;
			sr.sprite = sprites.frames[0];
			if (sprites.frames.Length > 1)
			{
				index = 0;
				InvokeRepeating("UpdateFrame", sprites.interval, sprites.interval);
			}
		}
	}

	private void OnDisable()
	{
		CancelInvoke();
		sprites = null;
	}

	public void UpdateFrame()
	{
		index++;
		if (index >= sprites.frames.Length)
		{
			index = 0;
		}
		sr.sprite = sprites.frames[index];
	}
}
