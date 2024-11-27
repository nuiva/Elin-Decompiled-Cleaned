using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class AI_Torture : AIAct
{
	public override CursorInfo CursorIcon
	{
		get
		{
			return CursorSystem.IconMelee;
		}
	}

	public override bool HasProgress
	{
		get
		{
			return true;
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
			return false;
		}
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		this._owner = this.owner;
		this.isFail = (() => this.shackle == null || !this.shackle.owner.ExistsOnMap || !this.owner.isRestrained || !this.shackle.owner.pos.Equals(this.owner.pos) || this.owner.stamina.value <= 0);
		while (!this.owner.IsPC && this.owner.stamina.value < this.owner.stamina.max * 4 / 5)
		{
			yield return base.KeepRunning();
		}
		yield return base.DoProgress();
		yield break;
	}

	public override AIProgress CreateProgress()
	{
		Progress_Custom progress_Custom = new Progress_Custom();
		progress_Custom.canProgress = (() => !this.isFail());
		progress_Custom.onProgressBegin = delegate()
		{
		};
		progress_Custom.onProgress = delegate(Progress_Custom p)
		{
			if (p.progress % 8 == 0)
			{
				this.owner.PlayAnime(this.shackle.animeId, false);
			}
			if (EClass.rnd(20) == 0)
			{
				this.owner.Talk("restrained", null, null, false);
			}
			List<Chara> list = new List<Chara>();
			foreach (Chara chara in EClass._map.charas)
			{
				if (!chara.IsDisabled && chara.IsPCFaction && chara != this.owner && !chara.IsPC && !chara.isRestrained && chara.host == null && !chara.noMove && !chara.IsInCombat)
				{
					list.Add(chara);
				}
			}
			list.Sort((Chara a, Chara b) => this.<CreateProgress>g__SortVal|10_4(a) - this.<CreateProgress>g__SortVal|10_4(b));
			int num = 1 + EClass.Branch.members.Count / 5;
			int num2 = 0;
			foreach (Chara chara2 in list)
			{
				if (EClass.rnd(3) == 0 && chara2.HasAccess(this.owner.pos))
				{
					chara2.SetEnemy(this.owner);
				}
				num2++;
				if (num2 >= num)
				{
					break;
				}
			}
		};
		progress_Custom.onProgressComplete = delegate()
		{
		};
		progress_Custom.cancelWhenDamaged = false;
		return progress_Custom.SetDuration(10000, 2);
	}

	public override void OnCancelOrSuccess()
	{
		if (this._owner != null)
		{
			foreach (Chara chara in EClass._map.charas)
			{
				if (chara != this._owner && (chara.enemy == this._owner || chara.enemy == this._owner.parasite || chara.enemy == this._owner.ride))
				{
					chara.SetEnemy(null);
					if (!(chara.ai is AI_Torture))
					{
						chara.ai.Cancel();
					}
				}
			}
		}
	}

	[CompilerGenerated]
	private int <CreateProgress>g__SortVal|10_4(Chara c)
	{
		return this.owner.Dist(c);
	}

	public TraitShackle shackle;

	public Chara _owner;
}
