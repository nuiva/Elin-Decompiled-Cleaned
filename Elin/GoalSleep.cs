using System;
using System.Collections.Generic;

public class GoalSleep : Goal
{
	public override IEnumerable<AIAct.Status> Run()
	{
		int i;
		int num;
		for (i = 0; i < 5; i = num + 1)
		{
			yield return AIAct.Status.Running;
			num = i;
		}
		TraitBed traitBed = this.owner.FindBed();
		if (traitBed == null)
		{
			traitBed = this.owner.TryAssignBed();
		}
		if (traitBed != null)
		{
			this.bed = traitBed.owner.Thing;
		}
		if (this.bed != null)
		{
			yield return base.DoGoto(this.bed, new Func<AIAct.Status>(base.KeepRunning));
		}
		else if (this.owner.memberType != FactionMemberType.Livestock)
		{
			BaseArea baseArea = EClass._map.FindPublicArea();
			if (baseArea != null)
			{
				yield return base.DoGoto(baseArea.GetRandomFreePos(), 0, false, new Func<AIAct.Status>(base.KeepRunning));
			}
		}
		i = 0;
		while (i < 5 && this.owner.pos.HasMultipleChara)
		{
			this.owner.MoveRandom();
			yield return AIAct.Status.Running;
			num = i;
			i = num + 1;
		}
		this.owner.AddCondition<ConSleep>(3000, true);
		yield return AIAct.Status.Running;
		yield break;
	}

	public override void OnSimulatePosition()
	{
		this.owner.AddCondition<ConSleep>(2000, true);
		TraitBed traitBed = this.owner.FindBed();
		this.bed = (((traitBed != null) ? traitBed.owner.Thing : null) ?? null);
		if (this.bed == null && EClass._zone.IsPCFaction)
		{
			this.owner.TryAssignBed();
		}
		if (this.bed != null && !this.bed.pos.HasChara)
		{
			this.owner.MoveImmediate(this.bed.pos, true, true);
			return;
		}
		BaseArea baseArea = EClass._map.FindPublicArea();
		if (baseArea != null)
		{
			this.owner.MoveImmediate(baseArea.GetRandomFreePos(), true, true);
		}
	}

	public int timer = 20;

	public Thing bed;
}
