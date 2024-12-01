using Newtonsoft.Json;
using UnityEngine;

public class HotItemToggle : HotItem
{
	public enum Type
	{
		ToggleRoof,
		showBalloon,
		muteBGM,
		instaComplete,
		ToggleSlope,
		ToggleWall,
		ToggleFreepos,
		SnapFreepos,
		ToggleBuildLight,
		ToggleRoofEdit,
		ToggleNoRoof
	}

	[JsonProperty]
	public Type type;

	public override string Name => type.ToString().lang();

	public override Sprite SpriteHighlight => SpriteSheet.Get(pathSprite + "_highlight") ?? EClass.core.refs.spritesHighlight[2];

	public override string pathSprite => "icon_" + type;

	public override bool Hidden
	{
		get
		{
			if (type == Type.instaComplete)
			{
				return !EClass.debug.enable;
			}
			return false;
		}
	}

	public override bool ShouldHighlight()
	{
		return type switch
		{
			Type.ToggleBuildLight => EClass.game.config.buildLight, 
			Type.SnapFreepos => EClass.game.config.snapFreePos, 
			Type.ToggleFreepos => EClass.game.config.freePos, 
			Type.ToggleWall => EClass.game.config.showWall, 
			Type.ToggleRoof => EClass.game.config.showRoof, 
			Type.ToggleNoRoof => EClass.game.config.noRoof, 
			Type.showBalloon => !EClass.scene.hideBalloon, 
			Type.muteBGM => EClass.Sound.muteBGM, 
			Type.instaComplete => EClass.player.instaComplete, 
			Type.ToggleSlope => EClass.game.config.slope, 
			_ => false, 
		};
	}

	public override void OnClick(ButtonHotItem b, Hotbar h)
	{
		switch (type)
		{
		case Type.ToggleBuildLight:
			EClass.scene.ToggleLight();
			break;
		case Type.SnapFreepos:
			EClass.scene.ToggleSnapFreePos();
			break;
		case Type.ToggleFreepos:
			EClass.scene.ToggleFreePos();
			break;
		case Type.ToggleWall:
			EClass.scene.ToggleShowWall();
			break;
		case Type.ToggleRoof:
			EClass.scene.ToggleShowRoof();
			break;
		case Type.ToggleNoRoof:
			EClass.scene.ToggleRoof();
			break;
		case Type.showBalloon:
			EClass.scene.ToggleBalloon();
			break;
		case Type.muteBGM:
			EClass.scene.ToggleMuteBGM();
			break;
		case Type.instaComplete:
			EClass.player.instaComplete = !EClass.player.instaComplete;
			SE.ClickGeneral();
			if ((bool)b)
			{
				b.widget.RefreshHighlight();
			}
			break;
		case Type.ToggleSlope:
			EClass.scene.ToggleSlope();
			break;
		}
		if ((bool)b)
		{
			b.widget.RefreshHighlight();
		}
	}
}
