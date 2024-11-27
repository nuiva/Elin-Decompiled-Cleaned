using System;
using UnityEngine;

public class TraitBaseContainer : Trait
{
	public override string IDInvStyle
	{
		get
		{
			if (base.GetParam(3, null) != null)
			{
				return base.GetParam(3, null);
			}
			return this.DefaultIdInvStyle;
		}
	}

	public override RefCardName RefCardName
	{
		get
		{
			return RefCardName.None;
		}
	}

	public string idContainer
	{
		get
		{
			if (base.GetParam(4, null) != null)
			{
				return base.GetParam(4, null);
			}
			return this.DefaultIdContainer;
		}
	}

	public int Width
	{
		get
		{
			if (this.owner.sourceCard.trait.Length <= 1)
			{
				return this.DefaultWidth;
			}
			return base.GetParam(1, null).ToInt();
		}
	}

	public int Height
	{
		get
		{
			if (this.owner.sourceCard.trait.Length <= 2)
			{
				return this.DefaultHeight;
			}
			return base.GetParam(2, null).ToInt();
		}
	}

	public virtual string DefaultIdInvStyle
	{
		get
		{
			return base.IDInvStyle;
		}
	}

	public virtual string DefaultIdContainer
	{
		get
		{
			return "default";
		}
	}

	public virtual int DefaultWidth
	{
		get
		{
			return 6;
		}
	}

	public virtual int DefaultHeight
	{
		get
		{
			return 2;
		}
	}

	public virtual int ChanceLock
	{
		get
		{
			return 0;
		}
	}

	public virtual int ChanceMedal
	{
		get
		{
			return 50;
		}
	}

	public override int DecaySpeedChild
	{
		get
		{
			return 50;
		}
	}

	public override bool IsContainer
	{
		get
		{
			return true;
		}
	}

	public virtual bool ShowOpenActAsCrime
	{
		get
		{
			return this.owner.isNPCProperty;
		}
	}

	public override bool UseAltTiles
	{
		get
		{
			return this.owner.things.Count == 0 && this.owner.c_lockLv == 0;
		}
	}

	public override void OnCreate(int lv)
	{
		this.owner.things.SetSize(this.Width, this.Height);
		if (this.ChanceLock > 0 && this.ChanceLock > EClass.rnd(100))
		{
			lv += 10;
			this.owner.c_lockLv = EClass.curve(5 + lv / 2 + EClass.rnd(lv / 2), 50, 10, 80);
		}
		this.Prespawn(lv);
	}

	public virtual void Prespawn(int lv)
	{
		if (!this.CanOpenContainer)
		{
			return;
		}
		int num = 1 + EClass.rnd(2);
		int num2 = EClass.curve(lv, 20, 15, 75);
		for (int i = 0; i < num; i++)
		{
			Thing thing;
			if (EClass.sources.spawnLists.map.ContainsKey(this.idContainer))
			{
				thing = ThingGen.CreateFromFilter(this.idContainer, -1);
			}
			else if (EClass.sources.categories.map.ContainsKey(this.idContainer))
			{
				thing = ThingGen.CreateFromCategory(this.idContainer, -1);
			}
			else
			{
				string idContainer = this.idContainer;
				if (!(idContainer == "money"))
				{
					if (!(idContainer == "provision"))
					{
						if (EClass.sources.things.map.ContainsKey(this.idContainer))
						{
							thing = ThingGen.Create(this.idContainer, -1, -1);
						}
						else
						{
							thing = ThingGen.CreateFromFilter("container_general", (lv + this.owner.c_lockLv > 0) ? 5 : 0);
						}
					}
					else
					{
						thing = ThingGen.CreateFromCategory((EClass.rnd(2) == 0) ? "preserved" : "drink", -1);
					}
				}
				else if (EClass.rnd(2) == 0)
				{
					thing = ThingGen.Create("money", -1, -1).SetNum(10 + EClass.rnd(50 + num2 * 25));
				}
				else if (EClass.rnd(2) == 0)
				{
					thing = ThingGen.Create("money2", -1, -1).SetNum(1 + EClass.rnd(Mathf.Min(2 + num2 / 50, 5)));
				}
				else
				{
					thing = ThingGen.Create("plat", -1, -1);
				}
			}
			if (thing != null)
			{
				this.owner.AddCard(thing);
			}
		}
		if (EClass.rnd(this.ChanceMedal) == 0)
		{
			this.owner.Add("medal", 1, 1);
		}
	}

	public bool HasChara
	{
		get
		{
			return this.owner.c_idRefCard != null;
		}
	}

	public void PutChara(string id)
	{
		this.owner.c_idRefCard = id;
		this.owner.AddCard(ThingGen.Create("junk", -1, -1));
	}

	public override void SetName(ref string s)
	{
		if (this.owner.Thing.IsSharedContainer)
		{
			s = "_shared".lang(s, null, null, null, null);
		}
		if (!this.owner.c_idRefName.IsEmpty() && this.owner.c_altName.IsEmpty())
		{
			s = "_written".lang(this.owner.c_idRefName, s, null, null, null);
		}
	}
}
