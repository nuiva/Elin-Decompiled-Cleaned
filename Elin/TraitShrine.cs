public class TraitShrine : TraitPowerStatue
{
	public SourceMaterial.Row mat;

	public ShrineData Shrine => EClass.gamedata.shrines[owner.refVal];

	public override bool RenderExtra
	{
		get
		{
			if (owner.isOn)
			{
				if (!owner.IsInstalled)
				{
					return owner.isRoofItem;
				}
				return true;
			}
			return false;
		}
	}

	public override void OnCreate(int lv)
	{
		base.OnCreate(lv);
		owner.refVal = EClass.gamedata.shrines.IndexOf(EClass.gamedata.shrines.RandomItemWeighted((ShrineData a) => a.chance));
		owner.idSkin = Shrine.skin;
	}

	public override void _OnUse(Chara c)
	{
		Point point = (owner.ExistsOnMap ? owner.pos : EClass.pc.pos);
		switch (Shrine.id)
		{
		case "material":
			ActEffect.Proc(EffectId.ChangeMaterial, EClass.pc, null, 100, new ActRef
			{
				n1 = GetMaterial().alias
			});
			break;
		case "armor":
		{
			bool flag = mat.alias == "gold";
			ActEffect.Proc(flag ? EffectId.EnchantArmorGreat : EffectId.EnchantArmor, 100, flag ? BlessedState.Blessed : BlessedState.Normal, EClass.pc);
			break;
		}
		case "replenish":
		{
			foreach (Chara member in EClass.pc.party.members)
			{
				member.hp = member.MaxHP;
				member.mana.value = member.mana.max;
				member.stamina.Mod(member.stamina.max / 3);
				if (!member.IsPC)
				{
					member.stamina.value = member.stamina.max;
				}
				member.PlayEffect("revive");
				member.Say("heal_light", member);
				foreach (Condition item in member.conditions.Copy())
				{
					if (item.Type == ConditionType.Debuff && !item.IsKilled)
					{
						member.Say("removeHex", member, item.Name.ToLower());
						item.Kill();
					}
				}
			}
			break;
		}
		case "strife":
		{
			for (int i = 0; i < 3 + EClass.rnd(2); i++)
			{
				EClass._zone.SpawnMob(point.GetNearestPoint(allowBlock: false, allowChara: false), (i == 0) ? SpawnSetting.Boss(owner.LV) : SpawnSetting.DefenseEnemy(owner.LV))?.PlayEffect("teleport");
			}
			break;
		}
		case "knowledge":
		{
			Thing t2 = ThingGen.Create((EClass.rnd(3) == 0) ? "book_skill" : "book_ancient", -1, owner.LV);
			EClass._zone.AddCard(t2, point);
			break;
		}
		case "invention":
			EClass.player.recipes.ComeUpWithRandomRecipe(null, 10);
			break;
		case "item":
		{
			Thing t = ThingGen.Create("water");
			EClass._zone.AddCard(t, point);
			break;
		}
		}
	}

	public override void OnRenderExtra(RenderParam p)
	{
		PaintPosition paintPosition = EClass.setting.render.paintPos.TryGetValue("shrine");
		int num = 1;
		p.x += paintPosition.pos.x * (float)num;
		p.y += paintPosition.pos.y;
		p.z += paintPosition.pos.z;
		p.tile = EClass.core.refs.renderers.objs_shrine.ConvertTile(Shrine.tile) * num;
		p.matColor = BaseTileMap.GetColorInt(ref GetMaterial().matColor, 100);
		EClass.core.refs.renderers.objs_shrine.Draw(p);
	}

	public SourceMaterial.Row GetMaterial()
	{
		if (mat != null)
		{
			return mat;
		}
		Rand.SetSeed(owner.c_seed);
		if (Shrine.id == "armor")
		{
			mat = EClass.sources.materials.alias[(EClass.rnd(5) == 0) ? "gold" : "granite"];
		}
		else
		{
			mat = MATERIAL.GetRandomMaterial(owner.LV / 3, (EClass.rnd(2) == 0) ? "metal" : "leather");
		}
		Rand.SetSeed();
		return mat;
	}

	public override string GetName()
	{
		string @ref = "";
		if (Shrine.id == "material")
		{
			@ref = GetMaterial().GetName().ToTitleCase();
		}
		return ("shrine_" + Shrine.id).lang(@ref);
	}
}
