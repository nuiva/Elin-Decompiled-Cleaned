using System;
using System.Collections.Generic;

public class TraitBed : Trait
{
	public override bool CanStack
	{
		get
		{
			return false;
		}
	}

	public int MaxHolders
	{
		get
		{
			if (this.owner.c_bedType != BedType.residentOne)
			{
				return ((base.GetParam(1, null) == null) ? 1 : base.GetParam(1, null).ToInt()) + this.owner.c_containerSize;
			}
			return 1;
		}
	}

	public override bool IsChangeFloorHeight
	{
		get
		{
			return true;
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		p.TrySetAct(new AI_Sleep
		{
			target = this.owner.Thing
		}, this.owner);
		p.TrySetAct(new AI_PassTime
		{
			target = this.owner.Thing
		}, this.owner);
		if (EClass._zone.IsPCFaction)
		{
			p.TrySetAct("bedConfig", delegate()
			{
				UIContextMenu uicontextMenu = EClass.ui.CreateContextMenuInteraction();
				if (this.HasHolder())
				{
					uicontextMenu.AddButton("unassignBed", delegate()
					{
						this.ClearHolders();
						SE.Trash();
					}, true);
				}
				else if (this.owner.c_bedType == BedType.resident || this.owner.c_bedType == BedType.residentOne)
				{
					uicontextMenu.AddButton("claimBed", delegate()
					{
						this.ClearHolders();
						this.AddHolder(EClass.pc);
						Msg.Say("claimBed", EClass.pc, null, null, null);
						SE.Play("jingle_embark");
					}, true);
				}
				using (List<BedType>.Enumerator enumerator = new List<BedType>
				{
					BedType.resident,
					BedType.residentOne,
					BedType.guest
				}.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BedType t = enumerator.Current;
						if (t != BedType.livestock && t != BedType.patient)
						{
							uicontextMenu.AddButton(((t == this.owner.c_bedType) ? "context_checker".lang() : "") + ("bed_" + t.ToString()).lang(), delegate()
							{
								this.SetBedType(t);
								SE.ClickOk();
							}, true);
						}
					}
				}
				CursorSystem.ignoreCount = 5;
				uicontextMenu.Show();
				return false;
			}, this.owner, null, 1, false, true, false);
		}
	}

	public void AddHolder(Chara c)
	{
		CharaList charaList = this.owner.c_charaList;
		if (charaList == null)
		{
			charaList = (this.owner.c_charaList = new CharaList());
		}
		charaList.Add(c);
	}

	public void RemoveHolder(Chara c)
	{
		CharaList c_charaList = this.owner.c_charaList;
		if (c_charaList == null)
		{
			return;
		}
		c_charaList.Remove(c);
		if (c_charaList.list.Count == 0)
		{
			this.owner.c_charaList = null;
		}
	}

	public void ClearHolders()
	{
		CharaList c_charaList = this.owner.c_charaList;
		if (c_charaList != null)
		{
			c_charaList.list.Clear();
		}
	}

	public bool IsHolder(Chara c)
	{
		CharaList c_charaList = this.owner.c_charaList;
		return c_charaList != null && c_charaList.list.Contains(c.uid);
	}

	public bool IsFull()
	{
		CharaList c_charaList = this.owner.c_charaList;
		return c_charaList != null && c_charaList.list.Count >= this.MaxHolders;
	}

	public bool HasHolder()
	{
		CharaList c_charaList = this.owner.c_charaList;
		return c_charaList != null && c_charaList.list.Count > 0;
	}

	public bool CanAssign(Chara c)
	{
		CharaList c_charaList = this.owner.c_charaList;
		if (c_charaList != null && c_charaList.list.Count >= this.MaxHolders)
		{
			return false;
		}
		BedType c_bedType = this.owner.c_bedType;
		FactionMemberType memberType = c.memberType;
		return (memberType == FactionMemberType.Default && (c_bedType == BedType.resident || c_bedType == BedType.residentOne)) || (memberType == FactionMemberType.Livestock && c_bedType == BedType.livestock) || (memberType == FactionMemberType.Guest && c_bedType == BedType.guest);
	}

	public void SetBedType(BedType bedType)
	{
		if (bedType == this.owner.c_bedType)
		{
			return;
		}
		this.owner.c_bedType = bedType;
		this.ClearHolders();
	}

	public override void SetName(ref string s)
	{
		CharaList c_charaList = this.owner.c_charaList;
		if (c_charaList != null)
		{
			List<Chara> list = c_charaList.Get();
			if (list.Count == 1)
			{
				s = "_bed".lang(list[0].NameSimple, s, null, null, null);
				return;
			}
			if (list.Count == 2)
			{
				s = "_bed".lang("_and".lang(list[0].NameSimple, list[1].NameSimple, null, null, null), s, null, null, null);
				return;
			}
		}
		if (this.owner.c_bedType == BedType.resident)
		{
			return;
		}
		string @ref = ("bed_" + this.owner.c_bedType.ToString()).lang().ToLower();
		s = "_of4".lang(@ref, s, null, null, null);
	}

	public override string GetHoverText()
	{
		CharaList c_charaList = this.owner.c_charaList;
		if (c_charaList == null || c_charaList.Get().Count < 3)
		{
			return null;
		}
		string text = "";
		foreach (Chara chara in c_charaList.Get())
		{
			if (text != "")
			{
				text += ", ";
			}
			text += chara.NameSimple;
		}
		return "(" + text + ")";
	}
}
