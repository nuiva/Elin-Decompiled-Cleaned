using System;
using Newtonsoft.Json;
using UnityEngine;

public class HotItemSpeed : HotItem
{
	public override string Name
	{
		get
		{
			return "changeSpeed".lang() + " " + this.id.ToString();
		}
	}

	public override string pathSprite
	{
		get
		{
			return "icon_speed" + this.id.ToString();
		}
	}

	public override Sprite GetSprite(bool highlight)
	{
		if (!highlight)
		{
			return this.GetSprite();
		}
		return EClass.core.refs.spritesHighlightSpeed[this.id];
	}

	public override string TextTip
	{
		get
		{
			return null;
		}
	}

	public override bool UseIconForHighlight
	{
		get
		{
			return true;
		}
	}

	public override bool ShouldHighlight()
	{
		return this.id == EClass.game.gameSpeedIndex;
	}

	public int speedIndex
	{
		get
		{
			return EClass.game.gameSpeedIndex;
		}
	}

	public override void OnClick(ButtonHotItem b, Hotbar h)
	{
		EClass.scene.actionMode.ChangeGameSpeed(this.id, true);
	}

	[JsonProperty]
	public int id;
}
