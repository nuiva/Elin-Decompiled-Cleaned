using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class TaskMoveInstalled : TaskBaseBuild
{
	public override int W
	{
		get
		{
			if (this.dir % 2 != 0)
			{
				return this.target.sourceCard.H;
			}
			return this.target.sourceCard.W;
		}
	}

	public override int H
	{
		get
		{
			if (this.dir % 2 != 0)
			{
				return this.target.sourceCard.W;
			}
			return this.target.sourceCard.H;
		}
	}

	public override HitResult GetHitResult()
	{
		if (!base.CanPlaceCard(this.pos, this.target))
		{
			return HitResult.Invalid;
		}
		if (base.PointHasOtherDesignation())
		{
			return HitResult.Invalid;
		}
		if (EClass.scene.actionMode.IsRoofEditMode(null))
		{
			return HitResult.Valid;
		}
		return this.target.TileType._HitTest(this.pos, this.target, true);
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		if (!this.target.ExistsOnMap)
		{
			yield return base.Destroy();
		}
		this.target.TryReserve(this);
		yield return base.DoGrab(this.target, -1, false, null);
		for (;;)
		{
			TaskMoveInstalled.<>c__DisplayClass8_0 CS$<>8__locals1 = new TaskMoveInstalled.<>c__DisplayClass8_0();
			if (this.owner.held != this.target)
			{
				yield return this.Cancel();
			}
			CS$<>8__locals1.objPos = null;
			this.pos.ForeachMultiSize(this.target.W, this.target.H, delegate(Point p, bool main)
			{
				if (p.HasObj)
				{
					CS$<>8__locals1.objPos = p.Copy();
				}
			});
			if (CS$<>8__locals1.objPos == null)
			{
				break;
			}
			yield return base.Do(new TaskCut
			{
				pos = CS$<>8__locals1.objPos
			}, null);
			CS$<>8__locals1 = null;
		}
		yield return base.DoGoto(this.pos, 1, false, null);
		this.OnProgressComplete();
		yield break;
	}

	public override void OnProgressComplete()
	{
		bool flag = this.target.placeState == PlaceState.roaming && this.target.ExistsOnMap;
		if (this.owner == EClass.player.Agent)
		{
			EClass._zone.RemoveCard(this.target);
			this.target.dir = this.dir;
			EClass._zone.AddCard(this.target, this.pos);
		}
		else
		{
			this.target.dir = this.dir;
			this.owner.DropHeld(this.pos);
		}
		if (flag)
		{
			this.target.SetPlaceState(PlaceState.roaming, false);
		}
		else
		{
			this.target.SetPlaceState(PlaceState.installed, false);
		}
		this.target.altitude = this.altitude;
		this.target.ignoreStackHeight = Input.GetKey(KeyCode.LeftControl);
		this.target.PlayAnime(AnimeID.Place, false);
		this.pos.PlaySound("build", true, 1f, true);
		EClass._map.RefreshFOV(this.pos.x, this.pos.z, 6, false);
	}

	[JsonProperty]
	public Card target;

	[JsonProperty]
	public int dir;

	[JsonProperty]
	public int altitude;
}
