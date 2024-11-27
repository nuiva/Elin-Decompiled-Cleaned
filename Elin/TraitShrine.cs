using System;
using System.Collections.Generic;
using UnityEngine;

public class TraitShrine : TraitPowerStatue
{
	public ShrineData Shrine
	{
		get
		{
			return EClass.gamedata.shrines[this.owner.refVal];
		}
	}

	public override bool RenderExtra
	{
		get
		{
			return this.owner.isOn && this.owner.IsInstalled;
		}
	}

	public override void OnCreate(int lv)
	{
		base.OnCreate(lv);
		this.owner.refVal = EClass.gamedata.shrines.IndexOf(EClass.gamedata.shrines.RandomItemWeighted((ShrineData a) => a.chance));
		this.owner.idSkin = this.Shrine.skin;
	}

	public override void _OnUse(Chara c)
	{
		Point point = this.owner.ExistsOnMap ? this.owner.pos : EClass.pc.pos;
		string id = this.Shrine.id;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(id);
		if (num <= 1052114667U)
		{
			if (num != 164943590U)
			{
				if (num != 413259977U)
				{
					if (num != 1052114667U)
					{
						return;
					}
					if (!(id == "knowledge"))
					{
						return;
					}
					Thing t = ThingGen.Create((EClass.rnd(3) == 0) ? "book_skill" : "book_ancient", -1, this.owner.LV);
					EClass._zone.AddCard(t, point);
					return;
				}
				else
				{
					if (!(id == "replenish"))
					{
						return;
					}
					using (List<Chara>.Enumerator enumerator = EClass.pc.party.members.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Chara chara = enumerator.Current;
							chara.hp = chara.MaxHP;
							chara.mana.value = chara.mana.max;
							chara.stamina.Mod(chara.stamina.max / 3);
							if (!chara.IsPC)
							{
								chara.stamina.value = chara.stamina.max;
							}
							chara.PlayEffect("revive", true, 0f, default(Vector3));
							chara.Say("heal_light", chara, null, null);
							foreach (Condition condition in chara.conditions.Copy<Condition>())
							{
								if (condition.Type == ConditionType.Debuff && !condition.IsKilled)
								{
									chara.Say("removeHex", chara, condition.Name.ToLower(), null);
									condition.Kill(false);
								}
							}
						}
						return;
					}
				}
			}
			else if (!(id == "strife"))
			{
				return;
			}
			for (int i = 0; i < 3 + EClass.rnd(2); i++)
			{
				Chara chara2 = EClass._zone.SpawnMob(point.GetNearestPoint(false, false, true, false), (i == 0) ? SpawnSetting.Boss(this.owner.LV, -1) : SpawnSetting.DefenseEnemy(this.owner.LV));
				if (chara2 != null)
				{
					chara2.PlayEffect("teleport", true, 0f, default(Vector3));
				}
			}
			return;
		}
		if (num <= 3538210912U)
		{
			if (num != 2671260646U)
			{
				if (num != 3538210912U)
				{
					return;
				}
				if (!(id == "material"))
				{
					return;
				}
				ActEffect.Proc(EffectId.ChangeMaterial, EClass.pc, null, 100, new ActRef
				{
					n1 = this.GetMaterial().alias
				});
				return;
			}
			else
			{
				if (!(id == "item"))
				{
					return;
				}
				Thing t2 = ThingGen.Create("water", -1, -1);
				EClass._zone.AddCard(t2, point);
			}
		}
		else if (num != 3841201909U)
		{
			if (num != 4165567700U)
			{
				return;
			}
			if (!(id == "armor"))
			{
				return;
			}
			bool flag = this.mat.alias == "gold";
			ActEffect.Proc(flag ? EffectId.EnchantArmorGreat : EffectId.EnchantArmor, 100, flag ? BlessedState.Blessed : BlessedState.Normal, EClass.pc, null, default(ActRef));
			return;
		}
		else
		{
			if (!(id == "invention"))
			{
				return;
			}
			EClass.player.recipes.ComeUpWithRandomRecipe(null, 10);
			return;
		}
	}

	public override void OnRenderExtra(RenderParam p)
	{
		PaintPosition paintPosition = EClass.setting.render.paintPos.TryGetValue("shrine", null);
		int num = 1;
		p.x += paintPosition.pos.x * (float)num;
		p.y += paintPosition.pos.y;
		p.z += paintPosition.pos.z;
		p.tile = (float)(EClass.core.refs.renderers.objs_shrine.ConvertTile(this.Shrine.tile) * num);
		p.matColor = (float)BaseTileMap.GetColorInt(ref this.GetMaterial().matColor, 100);
		EClass.core.refs.renderers.objs_shrine.Draw(p);
	}

	public SourceMaterial.Row GetMaterial()
	{
		if (this.mat != null)
		{
			return this.mat;
		}
		Rand.SetSeed(this.owner.c_seed);
		if (this.Shrine.id == "armor")
		{
			this.mat = EClass.sources.materials.alias[(EClass.rnd(5) == 0) ? "gold" : "granite"];
		}
		else
		{
			this.mat = MATERIAL.GetRandomMaterial(this.owner.LV / 3, (EClass.rnd(2) == 0) ? "metal" : "leather", false);
		}
		Rand.SetSeed(-1);
		return this.mat;
	}

	public override string GetName()
	{
		string @ref = "";
		if (this.Shrine.id == "material")
		{
			@ref = this.GetMaterial().GetName().ToTitleCase(false);
		}
		return ("shrine_" + this.Shrine.id).lang(@ref, null, null, null, null);
	}

	public SourceMaterial.Row mat;
}
