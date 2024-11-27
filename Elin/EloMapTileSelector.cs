using System;
using CreativeSpore.SuperTilemapEditor;
using UnityEngine;

public class EloMapTileSelector : EMono
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
		bool enable = !EMono.ui.isPointerOverUI;
		this.srHighlight.SetActive(enable);
		this.srHighlight2.SetActive(false);
		this.srEmbarkPoint.SetActive(false);
	}

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
}
