using System;

public class AM_BaseGameMode : ActionMode
{
	public override AreaHighlightMode AreaHihlight
	{
		get
		{
			return AreaHighlightMode.None;
		}
	}

	public override bool ShowActionHint
	{
		get
		{
			return false;
		}
	}

	public override bool HideSubWidgets
	{
		get
		{
			return false;
		}
	}

	public override BaseTileSelector.HitType hitType
	{
		get
		{
			return BaseTileSelector.HitType.Default;
		}
	}

	public override BaseTileSelector.SelectType selectType
	{
		get
		{
			return BaseTileSelector.SelectType.Single;
		}
	}

	public override BaseGameScreen TargetGameScreen
	{
		get
		{
			if (!EClass._zone.IsRegion)
			{
				return EClass.scene.screenElin;
			}
			return EClass.scene.screenElona;
		}
	}

	protected Point hit
	{
		get
		{
			return Scene.HitPoint;
		}
	}

	public override void OnUpdateInput()
	{
		this.isMouseOnMap = (!EClass.ui.isPointerOverUI && this.hit.IsValid);
		if (EInput.leftMouse.down && EClass.ui.isPointerOverUI)
		{
			EInput.leftMouse.Consume();
			return;
		}
		EAction action = EInput.action;
		if (action != EAction.Help)
		{
			if (action != EAction.Log)
			{
				if (action == EAction.Cancel && !WidgetSearch.Instance)
				{
					HotItemContext.Show("system", EInput.uiMousePosition);
				}
			}
			else
			{
				WidgetMainText.ToggleLog();
			}
		}
		else
		{
			LayerHelp.Toggle("general", "1");
		}
		if (EClass.pc.currentZone.IsActiveZone)
		{
			this._OnUpdateInput();
		}
	}

	public override string GetHintText()
	{
		return null;
	}

	public virtual void _OnUpdateInput()
	{
	}

	public override void OnCancel()
	{
	}

	public override HitResult HitTest(Point point, Point start)
	{
		return HitResult.Default;
	}

	public bool isMouseOnMap;
}
