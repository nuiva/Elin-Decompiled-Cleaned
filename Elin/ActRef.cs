using System;

public struct ActRef
{
	public int idEle
	{
		get
		{
			return EClass.sources.elements.alias[this.aliasEle].id;
		}
	}

	public Act act;

	public string aliasEle;

	public string n1;

	public Thing refThing;

	public Chara origin;

	public bool isPerfume;

	public bool noFriendlyFire;
}
