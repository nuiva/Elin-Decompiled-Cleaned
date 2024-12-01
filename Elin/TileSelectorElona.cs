using System;
using CreativeSpore.SuperTilemapEditor;
using UnityEngine;

public class TileSelectorElona : BaseTileSelector
{
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

	public EloMap elomap => actor.elomap;

	public STETilemap fogmap => elomap.fogmap;

	public override void OnRenderTile(Point point, HitResult result, bool shouldHide)
	{
		srHighlight.SetActive(!shouldHide);
		if (result != HitResult.NoTarget && point.IsValid)
		{
			Vector3 vector = point.PositionTopdown();
			srHighlight.transform.position = vector + fixHighlight;
			int mouseGridX = TilemapUtils.GetMouseGridX(fogmap, EMono.scene.cam);
			int mouseGridY = TilemapUtils.GetMouseGridY(fogmap, EMono.scene.cam);
			if (EMono.debug.enable && Input.GetMouseButtonDown(2))
			{
				elomap.GetTileInfo(mouseGridX, mouseGridY);
			}
		}
	}

	public void OnUpdateInput()
	{
		int mouseGridX = TilemapUtils.GetMouseGridX(fogmap, EMono.scene.cam);
		int mouseGridY = TilemapUtils.GetMouseGridY(fogmap, EMono.scene.cam);
		if (elomap.GetCell(mouseGridX, mouseGridY) != null && (mouseGridX != gx || mouseGridY != gy))
		{
			gx = mouseGridX;
			gy = mouseGridY;
			hasTargetChanged = true;
			textMouseOver = "";
			Zone zone = elomap.GetZone(gx, gy);
			if (zone != null)
			{
				textMouseOver = textMouseOver + " " + zone.Name;
			}
			srHighlight.transform.position = TilemapUtils.GetGridWorldPos(fogmap, gx, gy);
		}
		else
		{
			hasTargetChanged = false;
		}
		if (EMono.debug.enable)
		{
			elomap.GetTileInfo(gx, gy);
		}
		bool enable = !EMono.ui.isPointerOverUI;
		srHighlight.SetActive(enable);
		srHighlight2.SetActive(enable: false);
		bool isActive = ActionMode.Title.IsActive;
		srEmbarkPoint.SetActive(isActive);
		if (isActive)
		{
			srEmbarkPoint.transform.position = TilemapUtils.GetGridWorldPos(fogmap, EMono.player.zone.x, EMono.player.zone.y);
		}
	}
}
