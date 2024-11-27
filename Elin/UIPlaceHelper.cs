using System;
using UnityEngine;

public class UIPlaceHelper : EMono
{
	public void Refresh()
	{
		Recipe recipe = HotItemHeld.recipe;
		bool flag = recipe != null;
		if (EMono.player.currentHotItem.Thing == null || (EMono.player.currentHotItem.Thing.trait.IsTool && !HotItemHeld.disableTool))
		{
			flag = false;
		}
		base.gameObject.SetActive(flag);
		if (!flag)
		{
			return;
		}
		this.buttonAutoWall.SetActive(false);
		this.buttonAutoWall.mainText.text = (EMono.game.config.autoWall ? "On" : "Off");
		this.buttonAutoWall.icon.SetAlpha(EMono.game.config.autoWall ? 1f : 0.4f);
		this.buttonAutoWall.SetOnClick(delegate
		{
			SE.Tab();
			EMono.game.config.autoWall = !EMono.game.config.autoWall;
			this.Refresh();
		});
		this.buttonFreePlace.SetActive(false);
		this.buttonFreePlace.mainText.text = (EMono.game.config.freePos ? "On" : "Off");
		this.buttonFreePlace.icon.SetAlpha(EMono.game.config.freePos ? 1f : 0.4f);
		this.buttonFreePlace.SetOnClick(delegate
		{
			SE.Tab();
			EMono.game.config.freePos = !EMono.game.config.freePos;
			this.Refresh();
		});
		this.buttonHeight.SetActive(recipe.MaxAltitude != 0);
		this.buttonHeight.mainText.text = ((ActionMode.Build.altitude >= 0) ? "+" : "") + ActionMode.Build.altitude.ToString();
		this.buttonHeight.SetOnClick(delegate
		{
			SE.Tab();
			ActionMode.Build.ModAltitude(1);
			this.Refresh();
		});
		this.buttonHeight.onRightClick = delegate()
		{
			SE.Tab();
			ActionMode.Build.ModAltitude(-1);
			this.Refresh();
		};
		this.buttonHeight.onInputWheel = delegate(int a)
		{
			SE.Tab();
			ActionMode.Build.ModAltitude(a);
			this.Refresh();
		};
		this.bgGrid.uvRect = new Rect(1f, 1f, (float)this.GetComponentsInDirectChildren(false).Count, 1f);
		this.RebuildLayout(false);
	}

	public UIRawImage bgGrid;

	public ButtonGrid buttonHeight;

	public ButtonGrid buttonFreePlace;

	public ButtonGrid buttonAutoWall;
}
