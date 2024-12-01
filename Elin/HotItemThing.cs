using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class HotItemThing : HotItemGameAction
{
	[JsonProperty]
	public Thing thing;

	[JsonProperty]
	public bool lost;

	public Act _act;

	public override Act act => _act;

	public override string Name => thing.Name;

	public override bool IsSelectable => !lost;

	public override Thing Thing => thing;

	public override bool UseUIObjMaterial => true;

	public override Sprite GetSprite()
	{
		return thing.GetImageSprite();
	}

	public override void SetImage(Image icon)
	{
		thing.SetImage(icon);
	}

	public override void SetSubText(UIText t)
	{
		thing.trait.SetMainText(t, hotitem: true);
	}
}
