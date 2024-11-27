using System;
using UnityEngine;

[Serializable]
public class RoofStyle
{
	public string GetName(int i)
	{
		if (i != 0)
		{
			return "Roof" + i.ToString();
		}
		return "None";
	}

	public RoofStyle.Type type;

	public int w;

	public int h;

	public int flatW;

	public bool useDefBlock;

	public bool reverse;

	public bool wing;

	public bool maxHeight;

	public bool coverLot;

	public Vector3 posFix;

	public Vector3 posFixBlock;

	public Vector3 lowRoofFix;

	public Vector3 snowFix;

	public float snowZ = -0.01f;

	public enum Type
	{
		None,
		Default = 5,
		DefaultNoTop,
		Flat,
		FlatFloor,
		Triangle
	}
}
