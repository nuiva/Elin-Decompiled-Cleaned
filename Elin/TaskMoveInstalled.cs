using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class TaskMoveInstalled : TaskBaseBuild
{
	[JsonProperty]
	public Card target;

	[JsonProperty]
	public int dir;

	[JsonProperty]
	public int altitude;

	public override int W
	{
		get
		{
			if (dir % 2 != 0)
			{
				return target.sourceCard.H;
			}
			return target.sourceCard.W;
		}
	}

	public override int H
	{
		get
		{
			if (dir % 2 != 0)
			{
				return target.sourceCard.W;
			}
			return target.sourceCard.H;
		}
	}

	public override HitResult GetHitResult()
	{
		if (!CanPlaceCard(pos, target))
		{
			return HitResult.Invalid;
		}
		if (PointHasOtherDesignation())
		{
			return HitResult.Invalid;
		}
		if (EClass.scene.actionMode.IsRoofEditMode())
		{
			return HitResult.Valid;
		}
		return target.TileType._HitTest(pos, target);
	}

	public override IEnumerable<Status> Run()
	{
		if (!target.ExistsOnMap)
		{
			yield return Destroy();
		}
		target.TryReserve(this);
		yield return DoGrab(target);
		while (true)
		{
			if (owner.held != target)
			{
				yield return Cancel();
			}
			Point objPos = null;
			pos.ForeachMultiSize(target.W, target.H, delegate(Point p, bool main)
			{
				if (p.HasObj)
				{
					objPos = p.Copy();
				}
			});
			if (objPos == null)
			{
				break;
			}
			yield return Do(new TaskCut
			{
				pos = objPos
			});
		}
		yield return DoGoto(pos, 1);
		OnProgressComplete();
	}

	public override void OnProgressComplete()
	{
		bool num = target.placeState == PlaceState.roaming && target.ExistsOnMap;
		if (owner == EClass.player.Agent)
		{
			EClass._zone.RemoveCard(target);
			target.dir = dir;
			EClass._zone.AddCard(target, pos);
		}
		else
		{
			target.dir = dir;
			owner.DropHeld(pos);
		}
		if (num)
		{
			target.SetPlaceState(PlaceState.roaming);
		}
		else
		{
			target.SetPlaceState(PlaceState.installed);
		}
		target.altitude = altitude;
		target.ignoreStackHeight = Input.GetKey(KeyCode.LeftControl);
		target.PlayAnime(AnimeID.Place);
		pos.PlaySound("build");
		EClass._map.RefreshFOV(pos.x, pos.z);
	}
}
