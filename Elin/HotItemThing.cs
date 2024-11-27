using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class HotItemThing : HotItemGameAction
{
	public override Act act
	{
		get
		{
			return this._act;
		}
	}

	public override string Name
	{
		get
		{
			return this.thing.Name;
		}
	}

	public override Sprite GetSprite()
	{
		return this.thing.GetImageSprite();
	}

	public override bool IsSelectable
	{
		get
		{
			return !this.lost;
		}
	}

	public override Thing Thing
	{
		get
		{
			return this.thing;
		}
	}

	public override bool UseUIObjMaterial
	{
		get
		{
			return true;
		}
	}

	public override void SetImage(Image icon)
	{
		this.thing.SetImage(icon);
	}

	public override void SetSubText(UIText t)
	{
		this.thing.trait.SetMainText(t, true);
	}

	[JsonProperty]
	public Thing thing;

	[JsonProperty]
	public bool lost;

	public Act _act;
}
