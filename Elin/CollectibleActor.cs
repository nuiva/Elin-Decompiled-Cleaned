using System;
using UnityEngine;

public class CollectibleActor : EMono
{
	private void OnCollisionEnter2D(Collision2D collision)
	{
		Hoard hoard = EMono.player.hoard;
		hoard.score++;
		if (hoard.score > hoard.hiScore)
		{
			hoard.hiScore = hoard.score;
		}
		if (!this.hasSound || hoard.maxSounds == 0)
		{
			return;
		}
		if (this.rb.velocity.magnitude > 0.8f && this.rb.position.y < 3f)
		{
			EMono.Sound.Play(this.item.Source.sound, this.rb.position, 0.01f * (float)hoard.volume);
		}
	}

	public void PlaySound(string id = null)
	{
		if (this.rb.position.y < 10f)
		{
			return;
		}
		EMono.Sound.Play(id.IsEmpty(this.item.Source.sound), this.rb.position, 0.01f * (float)EMono.player.hoard.volume);
	}

	public void Deactivate()
	{
		base.gameObject.SetActive(false);
		this.active = false;
		RigidUpdate[] array = this.updates;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].active = false;
		}
	}

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
}
