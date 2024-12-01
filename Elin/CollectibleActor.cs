using System;
using UnityEngine;

public class CollectibleActor : EMono
{
	public Hoard.Item item;

	public RigidUpdate[] updates;

	[NonSerialized]
	public float shadowY;

	[NonSerialized]
	public int shadow;

	[NonSerialized]
	public bool paired;

	[NonSerialized]
	public bool active = true;

	[NonSerialized]
	public bool hasSound;

	[NonSerialized]
	public Rigidbody2D rb;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Hoard hoard = EMono.player.hoard;
		hoard.score++;
		if (hoard.score > hoard.hiScore)
		{
			hoard.hiScore = hoard.score;
		}
		if (hasSound && hoard.maxSounds != 0 && rb.velocity.magnitude > 0.8f && rb.position.y < 3f)
		{
			EMono.Sound.Play(item.Source.sound, rb.position, 0.01f * (float)hoard.volume);
		}
	}

	public void PlaySound(string id = null)
	{
		if (!(rb.position.y < 10f))
		{
			EMono.Sound.Play(id.IsEmpty(item.Source.sound), rb.position, 0.01f * (float)EMono.player.hoard.volume);
		}
	}

	public void Deactivate()
	{
		base.gameObject.SetActive(value: false);
		active = false;
		RigidUpdate[] array = updates;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].active = false;
		}
	}
}
