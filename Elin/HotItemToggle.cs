using System;
using Newtonsoft.Json;
using UnityEngine;

public class HotItemToggle : HotItem
{
	public override string Name
	{
		get
		{
			return this.type.ToString().lang();
		}
	}

	public override Sprite SpriteHighlight
	{
		get
		{
			return SpriteSheet.Get(this.pathSprite + "_highlight") ?? EClass.core.refs.spritesHighlight[2];
		}
	}

	public override string pathSprite
	{
		get
		{
			return "icon_" + this.type.ToString();
		}
	}

	public override bool Hidden
	{
		get
		{
			return this.type == HotItemToggle.Type.instaComplete && !EClass.debug.enable;
		}
	}

	public override bool ShouldHighlight()
	{
		switch (this.type)
		{
		case HotItemToggle.Type.ToggleRoof:
			return EClass.game.config.showRoof;
		case HotItemToggle.Type.showBalloon:
			return !EClass.scene.hideBalloon;
		case HotItemToggle.Type.muteBGM:
			return EClass.Sound.muteBGM;
		case HotItemToggle.Type.instaComplete:
			return EClass.player.instaComplete;
		case HotItemToggle.Type.ToggleSlope:
			return EClass.game.config.slope;
		case HotItemToggle.Type.ToggleWall:
			return EClass.game.config.showWall;
		case HotItemToggle.Type.ToggleFreepos:
			return EClass.game.config.freePos;
		case HotItemToggle.Type.SnapFreepos:
			return EClass.game.config.snapFreePos;
		case HotItemToggle.Type.ToggleBuildLight:
			return EClass.game.config.buildLight;
		case HotItemToggle.Type.ToggleNoRoof:
			return EClass.game.config.noRoof;
		}
		return false;
	}

	public override void OnClick(ButtonHotItem b, Hotbar h)
	{
		switch (this.type)
		{
		case HotItemToggle.Type.ToggleRoof:
			EClass.scene.ToggleShowRoof();
			break;
		case HotItemToggle.Type.showBalloon:
			EClass.scene.ToggleBalloon();
			break;
		case HotItemToggle.Type.muteBGM:
			EClass.scene.ToggleMuteBGM();
			break;
		case HotItemToggle.Type.instaComplete:
			EClass.player.instaComplete = !EClass.player.instaComplete;
			SE.ClickGeneral();
			if (b)
			{
				b.widget.RefreshHighlight();
			}
			break;
		case HotItemToggle.Type.ToggleSlope:
			EClass.scene.ToggleSlope();
			break;
		case HotItemToggle.Type.ToggleWall:
			EClass.scene.ToggleShowWall();
			break;
		case HotItemToggle.Type.ToggleFreepos:
			EClass.scene.ToggleFreePos();
			break;
		case HotItemToggle.Type.SnapFreepos:
			EClass.scene.ToggleSnapFreePos();
			break;
		case HotItemToggle.Type.ToggleBuildLight:
			EClass.scene.ToggleLight();
			break;
		case HotItemToggle.Type.ToggleNoRoof:
			EClass.scene.ToggleRoof();
			break;
		}
		if (b)
		{
			b.widget.RefreshHighlight();
		}
	}

	[JsonProperty]
	public HotItemToggle.Type type;

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
}
