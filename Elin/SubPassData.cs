using System;
using UnityEngine;

[Serializable]
public class SubPassData
{
	public static SubPassData Default;

	public static SubPassData Current;

	public Vector3 offset;

	public Vector3 scale = Vector3.one;

	public Quaternion rotation = Quaternion.identity;

	public bool enable;

	public bool shadow;
}
