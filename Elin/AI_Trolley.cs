using System;
using System.Collections.Generic;
using UnityEngine;

public class AI_Trolley : AIAct
{
	public int dir
	{
		get
		{
			return this.trolley.owner.dir;
		}
	}

	public override bool CanManualCancel()
	{
		return true;
	}

	public override bool CancelWhenDamaged
	{
		get
		{
			return this.owner != null && (!this.owner.IsPC || this.owner.hp < this.owner.MaxHP / 3);
		}
	}

	public override bool CancelWhenMoved
	{
		get
		{
			return true;
		}
	}

	public override bool ShowCursor
	{
		get
		{
			return false;
		}
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		this.owner.Say("ride", this.owner, this.trolley.owner, null, null);
		if (!this.trolley.HideChara)
		{
			this.owner.Talk("ride2", null, null, false);
		}
		for (;;)
		{
			int nextDir = this.GetNextDir();
			if (!this.trolley.owner.ExistsOnMap || nextDir == -1)
			{
				yield return this.Stop(null);
			}
			this.trolley.owner.dir = nextDir;
			Point point = this.GetPoint(this.dir, true);
			this.owner.SetDir(nextDir);
			this.owner.PlayAnime(AnimeID.Truck, false);
			string idSound = this.trolley.GetIdSound();
			if (this.owner.IsPC)
			{
				this.owner.PlaySound(idSound, 1f, true);
			}
			else if (!(EClass.pc.ai is AI_Trolley))
			{
				this.owner.PlaySound(idSound, 1f, true);
				EClass.Sound.Stop(idSound, Mathf.Max(1f, this.trolley.FadeDuration));
			}
			foreach (Chara t in point.ListCharas())
			{
				this.owner.Kick(t, true, false);
			}
			EClass._map.MoveCard(point, this.owner);
			this.trolley.owner.MoveImmediate(point, true, true);
			this.trolley.owner.PlayAnime(AnimeID.Truck, false);
			this.running = true;
			yield return base.KeepRunning();
		}
		yield break;
	}

	public Point GetPoint(int d, bool onlyValid = true)
	{
		Point point = new Point();
		Point pos = this.trolley.owner.pos;
		point.Set(pos.x + AI_Trolley.VecList[d].x, pos.z + AI_Trolley.VecList[d].y);
		if (!point.IsValid || !point.IsInBounds || !point.HasRail || point.IsBlocked)
		{
			return null;
		}
		using (List<Thing>.Enumerator enumerator = point.Things.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.IsMultisize)
				{
					return null;
				}
			}
		}
		if (EClass.pc.pos.Equals(point))
		{
			return null;
		}
		return point;
	}

	public int GetNextDir()
	{
		int[] array = AI_Trolley.DirList[this.dir];
		for (int i = 0; i < 3 + (this.running ? 0 : 1); i++)
		{
			int num = array[i];
			if (this.GetPoint(num, true) != null)
			{
				return num;
			}
		}
		return -1;
	}

	public AIAct.Status Stop(Action action = null)
	{
		EClass.Sound.Stop(this.trolley.GetIdSound(), this.trolley.FadeDuration);
		if (this.owner != null)
		{
			this.owner.Say("ride_unride", this.owner, this.trolley.owner, null, null);
		}
		return base.Success(null);
	}

	public override void OnCancel()
	{
		this.Stop(null);
	}

	public static int[][] DirList = new int[][]
	{
		new int[]
		{
			0,
			1,
			3,
			2
		},
		new int[]
		{
			1,
			0,
			2,
			3
		},
		new int[]
		{
			2,
			1,
			3,
			0
		},
		new int[]
		{
			3,
			2,
			0,
			1
		}
	};

	public static Vector2Int[] VecList = new Vector2Int[]
	{
		new Vector2Int(0, -1),
		new Vector2Int(1, 0),
		new Vector2Int(0, 1),
		new Vector2Int(-1, 0)
	};

	public TraitTrolley trolley;

	public bool running;
}
