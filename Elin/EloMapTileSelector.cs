using System;
using CreativeSpore.SuperTilemapEditor;
using UnityEngine;

public class EloMapTileSelector : EMono
{
	public EloMapActor actor;

	public SpriteRenderer srHighlight;

	public SpriteRenderer srHighlight2;

	public SpriteRenderer srEmbarkPoint;

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
		bool enable = !EMono.ui.isPointerOverUI;
		srHighlight.SetActive(enable);
		srHighlight2.SetActive(enable: false);
		srEmbarkPoint.SetActive(enable: false);
	}
}
