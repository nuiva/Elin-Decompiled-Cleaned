using System;
using CreativeSpore.SuperTilemapEditor;
using UnityEngine;

public class TileSelectorElona : BaseTileSelector
{
	public EloMap elomap
	{
		get
		{
			return this.actor.elomap;
		}
	}

	public STETilemap fogmap
	{
		get
		{
			return this.elomap.fogmap;
		}
	}

	public unsafe override void OnRenderTile(Point point, HitResult result, bool shouldHide)
	{
		this.srHighlight.SetActive(!shouldHide);
		if (result == HitResult.NoTarget || !point.IsValid)
		{
			return;
		}
		Vector3 a = *point.PositionTopdown();
		this.srHighlight.transform.position = a + this.fixHighlight;
		int mouseGridX = TilemapUtils.GetMouseGridX(this.fogmap, EMono.scene.cam);
		int mouseGridY = TilemapUtils.GetMouseGridY(this.fogmap, EMono.scene.cam);
		if (EMono.debug.enable && Input.GetMouseButtonDown(2))
		{
			this.elomap.GetTileInfo(mouseGridX, mouseGridY);
		}
	}

	public void OnUpdateInput()
	{
		int mouseGridX = TilemapUtils.GetMouseGridX(this.fogmap, EMono.scene.cam);
		int mouseGridY = TilemapUtils.GetMouseGridY(this.fogmap, EMono.scene.cam);
		if (this.elomap.GetCell(mouseGridX, mouseGridY) != null && (mouseGridX != this.gx || mouseGridY != this.gy))
		{
			this.gx = mouseGridX;
			this.gy = mouseGridY;
			this.hasTargetChanged = true;
			this.textMouseOver = "";
			Zone zone = this.elomap.GetZone(this.gx, this.gy);
			if (zone != null)
			{
				this.textMouseOver = this.textMouseOver + " " + zone.Name;
			}
			this.srHighlight.transform.position = TilemapUtils.GetGridWorldPos(this.fogmap, this.gx, this.gy);
		}
		else
		{
			this.hasTargetChanged = false;
		}
		if (EMono.debug.enable)
		{
			this.elomap.GetTileInfo(this.gx, this.gy);
		}
		bool enable = !EMono.ui.isPointerOverUI;
		this.srHighlight.SetActive(enable);
		this.srHighlight2.SetActive(false);
		bool isActive = ActionMode.Title.IsActive;
		this.srEmbarkPoint.SetActive(isActive);
		if (isActive)
		{
			this.srEmbarkPoint.transform.position = TilemapUtils.GetGridWorldPos(this.fogmap, EMono.player.zone.x, EMono.player.zone.y);
		}
	}

	public EloMapActor actor;

	public SpriteRenderer srHighlight;

	public SpriteRenderer srHighlight2;

	public SpriteRenderer srEmbarkPoint;

	public Vector3 fixHighlight;

	[NonSerialized]
	public int gx;

	[NonSerialized]
	public int gy;

	[NonSerialized]
	public bool hasTargetChanged;

	[NonSerialized]
	public string textMouseOver;
}
