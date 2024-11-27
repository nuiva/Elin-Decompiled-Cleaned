using System;
using UnityEngine;

public class AM_Cinema : AM_BaseTileSelect
{
	public override bool IsBuildMode
	{
		get
		{
			return true;
		}
	}

	public override BaseTileSelector.SelectType selectType
	{
		get
		{
			return BaseTileSelector.SelectType.None;
		}
	}

	public override bool ShowActionHint
	{
		get
		{
			return false;
		}
	}

	public override bool ShowMouseoverTarget
	{
		get
		{
			return false;
		}
	}

	public override bool enableMouseInfo
	{
		get
		{
			return false;
		}
	}

	public override bool ShowMaskedThings
	{
		get
		{
			return false;
		}
	}

	public CinemaConfig conf
	{
		get
		{
			return EClass.player.cinemaConfig;
		}
	}

	public override void OnActivate()
	{
		if (!this.profile)
		{
			this.profile = SceneProfile.Load("cinema");
		}
		EClass.ui.widgets.Activate("ArtTool");
		this.destPos = null;
	}

	public override void OnDeactivate()
	{
		EClass.ui.widgets.DeactivateWidget("ArtTool");
		EClass.scene.camSupport.grading.cinemaBrightness = 0f;
		EClass.core.config.ApplyGrading();
	}

	public unsafe override void OnUpdateInput()
	{
		if (EInput.leftMouse.clicked && !EClass.ui.isPointerOverUI)
		{
			SE.ClickGeneral();
			EClass.ui.canvas.enabled = !EClass.ui.canvas.enabled;
		}
		if (EInput.rightMouse.down || this.conf.speed == 0)
		{
			this.destPos = null;
			return;
		}
		Vector3Int vector3Int = EClass.screen.grid.WorldToCell(EClass.screen.position);
		this.center.Set(-vector3Int.y, vector3Int.x - 1);
		this.center.Clamp(false);
		if (this.destPos == null || this.destPos.Distance(this.center) <= 2)
		{
			this.destPos = new Point();
			for (int i = 0; i < 10000; i++)
			{
				this.destPos.x = EClass._map.bounds.x + EClass.rnd(EClass._map.bounds.Width);
				this.destPos.z = EClass._map.bounds.z + EClass.rnd(EClass._map.bounds.Height);
				if (this.center.Distance(this.destPos) > EClass._map.bounds.Width)
				{
					break;
				}
			}
		}
		Vector3 normalized = (*this.destPos.PositionCenter() - *this.center.PositionCenter()).normalized;
		EClass.screen.position += normalized * Core.delta * (float)this.conf.speed * 0.5f;
	}

	public SceneProfile profile;

	public Point destPos;

	public Point center = new Point();
}
