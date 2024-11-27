using System;
using UnityEngine;

public class TraitScroll : Trait
{
	public virtual SourceElement.Row source
	{
		get
		{
			return null;
		}
	}

	public override SourceElement.Row GetRefElement()
	{
		return this.source;
	}

	public virtual int eleParent
	{
		get
		{
			if (this.source == null || this.source.aliasParent.IsEmpty())
			{
				return 76;
			}
			return EClass.sources.elements.alias[this.source.aliasParent].id;
		}
	}

	public virtual float MTPValue
	{
		get
		{
			return 1f;
		}
	}

	public virtual bool UseSourceValue
	{
		get
		{
			return true;
		}
	}

	public override bool CanRead(Chara c)
	{
		return c.IsPC && !c.isBlind && (!EClass._zone.IsUserZone || !this.owner.isNPCProperty);
	}

	public override int GetValue()
	{
		if (this.source != null && this.source.value != 0)
		{
			return (int)Mathf.Max(this.MTPValue * (float)this.source.value, 1f);
		}
		return base.GetValue();
	}
}
