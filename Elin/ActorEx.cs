using System;
using UnityEngine;

public class ActorEx : Actor
{
	public void SetOwner(Card c)
	{
		this.owner = c;
		if (this.type == ActorEx.Type.JukeBox)
		{
			this.data = EMono.core.refs.dictBGM.TryGetValue(this.owner.refVal, null);
		}
		if (this.audioSource)
		{
			this.audioSource.clip = this.data.clip;
			this.audioSource.pitch = this.data.pitch * (1f + ((this.data.randomPitch == 0f) ? 0f : Rand.Range(-this.data.randomPitch, this.data.randomPitch)));
		}
		this.Refresh();
	}

	public float GetVolume()
	{
		if (!EMono._zone.isStarted)
		{
			return 0f;
		}
		Room room = EMono.pc.Cell.room;
		Room room2 = this.owner.Cell.room;
		bool flag = room2 == null || room2.data.atrium || !room2.HasRoof;
		bool flag2 = room == room2 || (room2 == null && (room.data.atrium || !room.HasRoof));
		bool flag3 = flag2 || ((room != null) ? room.lot : null) == ((room2 != null) ? room2.lot : null);
		float num = (this.alwaysOn || this.owner.isOn) ? 1f : 0f;
		if (!flag3)
		{
			num *= this.outsideLotVolume;
		}
		if (!flag2 && room2 != null)
		{
			num *= this.outsideRoomVolume;
		}
		if (num <= 0f)
		{
			return 0f;
		}
		if (this.type == ActorEx.Type.JukeBox)
		{
			num *= 0.5f + (flag ? 0.25f : 0f);
			if (flag2)
			{
				int num2 = EMono.pc.Dist(this.owner);
				if (num2 < 8)
				{
					float num3 = (float)(8 - num2) * SoundManager.current.jukeboxMod - 0.2f;
					if (num3 < SoundManager.bgmDumpMod)
					{
						SoundManager.bgmDumpMod = num3;
					}
				}
			}
		}
		return num;
	}

	public unsafe void Refresh()
	{
		if (this.owner.parent != EMono.pc.currentZone)
		{
			Debug.LogWarning(this.owner);
			return;
		}
		this.next--;
		if (this.next > 0)
		{
			return;
		}
		this.next = this.minInterval + Rand.rnd(this.randomInterval + 1);
		if (this.maxDistance != 0)
		{
			int num = EMono.pc.Dist(this.owner);
			if (base.gameObject.activeSelf)
			{
				if (num > this.maxDistance)
				{
					base.gameObject.SetActive(false);
					return;
				}
			}
			else
			{
				if (num > this.maxDistance)
				{
					return;
				}
				base.gameObject.SetActive(true);
			}
		}
		float volume = this.GetVolume();
		if (this.maxDistance != 0)
		{
			base.gameObject.SetActive(volume > 0f);
		}
		if (this.audioSource)
		{
			this.audioSource.volume = this.data.volume * volume;
			if (!this.audioSource.isPlaying && volume > 0f)
			{
				this.audioSource.Play();
			}
		}
		if (volume <= 0f)
		{
			return;
		}
		Vector3 vector = *this.owner.pos.PositionCenter();
		vector.z = 0f;
		if (this.audioSource)
		{
			base.transform.position = vector;
			return;
		}
		SoundManager.current.Play(this.data, vector, volume);
	}

	public void Kill()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public ActorEx.Type type;

	public Card owner;

	public bool alwaysOn;

	public float outsideRoomVolume = 0.5f;

	public float outsideLotVolume = 0.5f;

	public int maxDistance;

	public int minInterval = 1;

	public int randomInterval;

	private int next;

	public AudioSource audioSource;

	public SoundData data;

	public enum Type
	{
		Default,
		JukeBox
	}
}
