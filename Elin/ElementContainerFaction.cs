using System;

public class ElementContainerFaction : ElementContainer
{
	public bool IsEffective(Thing t)
	{
		return t.c_idDeity.IsEmpty() || t.c_idDeity == EClass.pc.idFaith;
	}

	public void OnEquip(Chara c, Thing t)
	{
		if (!c.IsPCFaction)
		{
			return;
		}
		this.OnEquip(t);
	}

	public void OnUnequip(Chara c, Thing t)
	{
		if (!c.IsPCFaction)
		{
			return;
		}
		this.OnUnequip(t);
	}

	public void OnEquip(Thing t)
	{
		if (!this.IsEffective(t))
		{
			return;
		}
		foreach (Element element in t.elements.dict.Values)
		{
			if (element.IsGlobalElement)
			{
				base.ModBase(element.id, element.Value).vExp = element.vExp;
				this.isDirty = true;
			}
		}
		this.CheckDirty();
	}

	public void OnUnequip(Thing t)
	{
		if (!this.IsEffective(t))
		{
			return;
		}
		foreach (Element element in t.elements.dict.Values)
		{
			if (element.IsGlobalElement)
			{
				base.ModBase(element.id, -element.Value);
				this.isDirty = true;
			}
		}
		this.CheckDirty();
	}

	public void CheckDirty()
	{
		if (!this.isDirty)
		{
			return;
		}
		foreach (Chara chara in EClass.game.cards.globalCharas.Values)
		{
			if (chara.IsPCFaction)
			{
				chara.Refresh(false);
			}
		}
		if (EClass.core.IsGameStarted)
		{
			foreach (Chara chara2 in EClass._map.charas)
			{
				if (chara2.IsPCFactionMinion)
				{
					chara2.Refresh(false);
				}
			}
		}
		this.isDirty = false;
	}

	public void OnLeaveFaith()
	{
		foreach (Chara chara in EClass.game.cards.globalCharas.Values)
		{
			if (chara.IsPCFaction)
			{
				this.OnRemoveMember(chara);
			}
		}
	}

	public void OnJoinFaith()
	{
		foreach (Chara chara in EClass.game.cards.globalCharas.Values)
		{
			if (chara.IsPCFaction)
			{
				this.OnAddMemeber(chara);
			}
		}
	}

	public void OnAddMemeber(Chara c)
	{
		foreach (BodySlot bodySlot in c.body.slots)
		{
			if (bodySlot.thing != null)
			{
				this.OnEquip(bodySlot.thing);
			}
		}
	}

	public void OnRemoveMember(Chara c)
	{
		foreach (BodySlot bodySlot in c.body.slots)
		{
			if (bodySlot.thing != null)
			{
				this.OnUnequip(bodySlot.thing);
			}
		}
	}

	public bool isDirty;
}
