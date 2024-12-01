using Newtonsoft.Json;
using UnityEngine;

public class HotItemSpeed : HotItem
{
	[JsonProperty]
	public int id;

	public override string Name => "changeSpeed".lang() + " " + id;

	public override string pathSprite => "icon_speed" + id;

	public override string TextTip => null;

	public override bool UseIconForHighlight => true;

	public int speedIndex => EClass.game.gameSpeedIndex;

	public override Sprite GetSprite(bool highlight)
	{
		if (!highlight)
		{
			return GetSprite();
		}
		return EClass.core.refs.spritesHighlightSpeed[id];
	}

	public override bool ShouldHighlight()
	{
		return id == EClass.game.gameSpeedIndex;
	}

	public override void OnClick(ButtonHotItem b, Hotbar h)
	{
		EClass.scene.actionMode.ChangeGameSpeed(id, sound: true);
	}
}
