using System;
using UnityEngine;

public class Resistance : Element
{
	public override bool CanLink(ElementContainer owner)
	{
		return !base.IsGlobalElement;
	}

	public override bool ShowPotential
	{
		get
		{
			return false;
		}
	}

	public override bool ShowXP
	{
		get
		{
			return false;
		}
	}

	public override string ShortName
	{
		get
		{
			return EClass.sources.elements.alias[base.source.aliasParent].GetName();
		}
	}

	public override Sprite GetIcon(string suffix = "")
	{
		return SpriteSheet.Get("Media/Graphics/Icon/Element/icon_elements", "ele_" + base.source.alias) ?? SpriteSheet.Get("Media/Graphics/Icon/Element/icon_elements", "ele_resist");
	}

	public override void SetTextValue(UIText text)
	{
		text.text = "なし";
	}
}
