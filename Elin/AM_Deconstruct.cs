using System;
using System.Collections.Generic;
using UnityEngine;

public class AM_Deconstruct : AM_BaseTileSelect
{
	public override bool IsRoofEditMode(Card c = null)
	{
		return Input.GetKey(KeyCode.LeftAlt);
	}

	public override bool IsBuildMode
	{
		get
		{
			return true;
		}
	}

	public override BaseTileMap.CardIconMode cardIconMode
	{
		get
		{
			return BaseTileMap.CardIconMode.Deconstruct;
		}
	}

	public override BaseTileSelector.SelectType selectType
	{
		get
		{
			if (!this.useRange)
			{
				return BaseTileSelector.SelectType.Single;
			}
			return BaseTileSelector.SelectType.Multiple;
		}
	}

	public override BaseTileSelector.HitType hitType
	{
		get
		{
			return BaseTileSelector.HitType.Default;
		}
	}

	public override MeshPass GetGuidePass(Point point)
	{
		if (this.Perform(point, false) <= 0)
		{
			return base.GetGuidePass(point);
		}
		return EClass.screen.guide.passGuideBlock;
	}

	public override bool ShowMouseoverTarget
	{
		get
		{
			return true;
		}
	}

	public override void OnUpdateCursor()
	{
		base.SetCursorOnMap(CursorSystem.Select);
	}

	public override bool UseSubMenu
	{
		get
		{
			return true;
		}
	}

	public override HitResult HitTest(Point point, Point start)
	{
		if (this.Perform(point, false) > 0)
		{
			return HitResult.Valid;
		}
		return base.HitTest(point, start);
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		if (this.Perform(point, false) == 0)
		{
			SE.Beep();
			return;
		}
		this.Perform(point, true);
	}

	public int Perform(Point point, bool perform = false)
	{
		List<Card> list = point.ListCards(false);
		int num = 0;
		list.Reverse();
		foreach (Card card in list.Copy<Card>())
		{
			if ((EClass.debug.ignoreBuildRule || (card.isThing && card.trait.CanPutAway)) && (!this.ignoreInstalled || !card.IsInstalled) && !card.IsPCParty)
			{
				if (perform)
				{
					card.PlaySound(card.material.GetSoundDead(card.sourceCard), 1f, true);
					if (card.isThing)
					{
						EClass._map.PutAway(card.Thing);
					}
					else
					{
						card.Destroy();
					}
					BuildMenu.dirtyCat = true;
				}
				num++;
			}
		}
		return num;
	}

	public override void OnClickSubMenu(int a)
	{
		if (a == 0)
		{
			this.useRange = !this.useRange;
			return;
		}
		if (a != 1)
		{
			return;
		}
		this.ignoreInstalled = !this.ignoreInstalled;
	}

	public override string OnSetSubMenuButton(int a, UIButton b)
	{
		if (a < 2)
		{
			if (a != 0)
			{
				if (a == 1)
				{
					b.SetCheck(this.ignoreInstalled);
				}
			}
			else
			{
				b.SetCheck(this.useRange);
			}
			return "deconstructMenu" + a.ToString();
		}
		return null;
	}

	public bool useRange;

	public bool ignoreInstalled;
}
