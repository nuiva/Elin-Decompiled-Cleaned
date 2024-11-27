using System;
using UnityEngine;

[Serializable]
public class SerializableDateTime : IComparable<SerializableDateTime>
{
	public DateTime DateTime
	{
		get
		{
			if (!this.initialized)
			{
				this.m_dateTime = new DateTime(this.m_ticks);
				this.initialized = true;
			}
			return this.m_dateTime;
		}
	}

	public SerializableDateTime(DateTime dateTime)
	{
		this.m_ticks = dateTime.Ticks;
		this.m_dateTime = dateTime;
		this.initialized = true;
	}

	public int CompareTo(SerializableDateTime other)
	{
		if (other == null)
		{
			return 1;
		}
		return this.m_ticks.CompareTo(other.m_ticks);
	}

	[SerializeField]
	private long m_ticks;

	private bool initialized;

	public DateTime m_dateTime;
}
