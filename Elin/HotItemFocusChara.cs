using System;
using Newtonsoft.Json;
using UnityEngine;

public class HotItemFocusChara : HotItem
{
	public Chara chara
	{
		get
		{
			return RefChara.Get(this.uid) ?? EClass.pc;
		}
	}

	public override Color SpriteColor
	{
		get
		{
			return new Color(1f, 1f, 1f, 0.9f);
		}
	}

	public override Vector3 SpriteScale
	{
		get
		{
			return new Vector3(0.8f, 0.8f, 1f);
		}
	}

	public override string Name
	{
		get
		{
			string s = "focusTo";
			Chara chara = this.chara;
			return s.lang(((chara != null) ? chara.NameSimple : null) ?? "???", null, null, null, null);
		}
	}

	public override string pathSprite
	{
		get
		{
			return "icon_focus";
		}
	}

	public override Sprite GetSprite()
	{
		if (this.chara != null)
		{
			return this.chara.GetSprite(0);
		}
		return base.GetSprite();
	}

	public override void OnClick(ButtonHotItem b, Hotbar h)
	{
		if (this.chara == null || EClass.AdvMode)
		{
			SE.Beep();
			return;
		}
		EClass.screen.Focus(this.chara);
	}

	[JsonProperty]
	public int uid;
}
