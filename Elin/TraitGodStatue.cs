using System;
using UnityEngine;

public class TraitGodStatue : TraitPowerStatue
{
	public Religion Religion
	{
		get
		{
			return EClass.game.religions.dictAll[base.GetParam(1, null)];
		}
	}

	public override bool IsImplemented()
	{
		return true;
	}

	public override void OnCreate(int lv)
	{
		base.OnCreate(lv);
		this.OnChangeMaterial();
	}

	public void OnChangeMaterial()
	{
		this.owner.isOn = (this.owner.material.alias == "gold");
		this.owner.rarity = (this.owner.isOn ? Rarity.Artifact : Rarity.Normal);
		if (this.Religion.id == "machine")
		{
			this.owner.AddCard(this.GetManiGene());
		}
		if (this.owner.placeState == PlaceState.installed)
		{
			this.owner.renderer.RefreshExtra();
		}
	}

	public Thing GetManiGene()
	{
		this.owner.things.DestroyAll(null);
		Debug.Log("Mani:" + this.owner.c_seed.ToString());
		Rand.SetSeed(this.owner.c_seed);
		CardRow r = SpawnList.Get("chara", null, null).Select(100, -1);
		Rand.SetSeed(this.owner.c_seed);
		Thing thing = DNA.GenerateGene(r, new DNA.Type?(DNA.Type.Superior), this.owner.LV, this.owner.c_seed);
		thing.c_DNA.cost = thing.c_DNA.cost / 2;
		thing.MakeRefFrom("mani");
		Rand.SetSeed(-1);
		Card owner = this.owner;
		int c_seed = owner.c_seed;
		owner.c_seed = c_seed + 1;
		return thing;
	}

	public override void _OnUse(Chara c)
	{
		this.Religion.Talk("shrine", null, null);
		string id = this.Religion.id;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(id);
		if (num <= 1330461687U)
		{
			if (num != 336916743U)
			{
				if (num != 636753111U)
				{
					if (num != 1330461687U)
					{
						return;
					}
					if (!(id == "element"))
					{
						return;
					}
				}
				else if (!(id == "earth"))
				{
					return;
				}
				Rand.SetSeed(this.owner.c_seed);
				SourceMaterial.Row randomMaterial = MATERIAL.GetRandomMaterial(this.owner.LV / 2 + 20, (this.Religion.id == "earth") ? "metal" : "leather", true);
				Thing thing = ThingGen.Create("mathammer", -1, -1);
				thing.ChangeMaterial(randomMaterial);
				thing.noSell = true;
				Rand.SetSeed(-1);
				EClass.pc.Pick(thing, true, true);
				return;
			}
			if (!(id == "wind"))
			{
				return;
			}
			EClass.pc.Pick(ThingGen.Create("blood_angel", -1, -1), true, true);
			return;
		}
		else if (num <= 3290931474U)
		{
			if (num != 2445848765U)
			{
				if (num != 3290931474U)
				{
					return;
				}
				if (!(id == "harvest"))
				{
					return;
				}
				Thing t = ThingGen.Create("book_kumiromi", -1, -1);
				EClass.pc.Pick(t, true, true);
				return;
			}
			else
			{
				if (!(id == "healing"))
				{
					return;
				}
				Msg.Say("jure_hug");
				EClass.player.ModKeyItem("jure_feather", 1, true);
				Msg.Say("jure_hug2");
				return;
			}
		}
		else if (num != 3775092334U)
		{
			if (num != 4145017712U)
			{
				return;
			}
			if (!(id == "luck"))
			{
				return;
			}
			EClass.player.ModKeyItem((!EClass.player.wellWished && EClass.player.CountKeyItem("well_wish") == 0) ? "well_wish" : "well_enhance", 1, true);
			return;
		}
		else
		{
			if (!(id == "machine"))
			{
				return;
			}
			Thing t2 = this.owner.things.Find("gene", -1, -1) ?? this.GetManiGene();
			EClass.pc.Pick(t2, true, true);
			return;
		}
	}
}
