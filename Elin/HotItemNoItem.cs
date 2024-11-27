using System;
using UnityEngine;

public class HotItemNoItem : HotItemGameAction
{
	public override Sprite GetSprite()
	{
		return EClass.core.refs.icons.noHotItem;
	}

	public static bool _TrySetAct(ActPlan p)
	{
		if (p.IsNeighborBlocked || !p.IsSelfOrNeighbor)
		{
			return false;
		}
		Card tg = null;
		p.pos.ListVisibleCards().ForeachReverse(delegate(Card a)
		{
			if (a.TileType.CanBeHeld && a.trait.CanBeHeld && a.isThing && !a.isNPCProperty && a.isThing && a.TileType.CanBeHeld)
			{
				tg = a;
				return true;
			}
			return false;
		});
		if (tg != null)
		{
			return p.TrySetAct("actHold", delegate()
			{
				if (tg.ExistsOnMap)
				{
					if (!EClass.pc.CanLift(tg))
					{
						EClass.pc.Say("tooHeavy", tg, null, null);
						return false;
					}
					if (tg.HasEditorTag(EditorTag.TreasureMelilith))
					{
						if (EClass.player.flags.pickedMelilithTreasure)
						{
							EClass.pc.PlaySound("curse3", 1f, true);
							EClass.pc.PlayEffect("curse", true, 0f, default(Vector3));
							EClass.pc.SetFeat(1206, 1, true);
							EClass.player.flags.gotMelilithCurse = true;
						}
						else
						{
							Msg.Say("pickedMelilithTreasure");
							EClass.player.flags.pickedMelilithTreasure = true;
							QuestCursedManor questCursedManor = EClass.game.quests.Get<QuestCursedManor>();
							if (questCursedManor != null)
							{
								questCursedManor.NextPhase();
							}
						}
						tg.c_editorTags = null;
					}
					EClass.pc.HoldCard(tg, -1);
					if (EClass.pc.held != null)
					{
						tg.PlaySoundHold(false);
						EClass.player.RefreshCurrentHotItem();
						ActionMode.Adv.planRight.Update(ActionMode.Adv.mouseTarget);
						EClass.pc.renderer.Refresh();
					}
				}
				return true;
			}, tg, null, 1, false, false, false);
		}
		TaskHarvest taskHarvest = TaskHarvest.TryGetAct(EClass.pc, p.pos);
		if (taskHarvest != null)
		{
			p.TrySetAct(taskHarvest, null);
		}
		return false;
	}

	public override bool TrySetAct(ActPlan p)
	{
		return HotItemNoItem._TrySetAct(p);
	}
}
