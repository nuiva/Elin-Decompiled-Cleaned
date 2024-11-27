using System;
using UnityEngine;

public class TCOrbitChara : TCOrbit
{
	public override void OnSetOwner()
	{
		this.owner = (base.owner as Chara);
		RenderData data = this.owner.renderer.data;
		this.goIcon.transform.SetLocalPositionY(data.offset.y + data.size.y + 0.32f);
		this.RefreshAll();
	}

	public override void Refresh()
	{
		this.timer += Time.deltaTime;
		if (this.timer > 0.2f)
		{
			this.timer = 0f;
			this.RefreshAll();
		}
		if ((!this.owner.isHidden || EMono.pc.CanSee(this.owner)) && (this.showHP || this.showHP2 || this.showIcon))
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
			}
			base.transform.position = this.owner.renderer.position;
			return;
		}
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(false);
		}
	}

	public void RefreshAll()
	{
		this.showHP = (this.showHP2 = (this.showIcon = false));
		if (this.owner.isDead || this.owner.host != null)
		{
			return;
		}
		AIAct ai = this.owner.ai;
		CoreRef.OrbitIcons orbitIcons = EMono.core.refs.orbitIcons;
		Emo2 emo = this.owner.emoIcon;
		if (emo == Emo2.none)
		{
			if (this.owner.isRestocking && !this.owner.IsPCParty && this.owner.trait.GetRestockedIcon() != Emo2.none && !(EMono._zone is Zone_Dungeon))
			{
				emo = this.owner.trait.GetRestockedIcon();
			}
			else if (EMono.player.currentHotItem.Thing != null && EMono.player.currentHotItem.Thing.trait.GetHeldEmo(this.owner) != Emo2.none)
			{
				emo = EMono.player.currentHotItem.Thing.trait.GetHeldEmo(this.owner);
			}
			else
			{
				bool flag = false;
				if (this.owner.quest != null && EMono.game.quests.list.Contains(this.owner.quest))
				{
					flag = true;
				}
				else if (this.owner.IsUnique)
				{
					string id = this.owner.id;
					if (!(id == "ashland"))
					{
						if (id == "fiama")
						{
							if (EMono.game.quests.GetPhase<QuestMain>() >= 200 && EMono.player.dialogFlags.TryGetValue("fiama1", 0) == 0)
							{
								flag = true;
							}
							else if (EMono.pc.homeBranch != null)
							{
								foreach (Chara chara in EMono.pc.homeBranch.members)
								{
									if (chara.isDead && chara.GetInt(100, null) != 0)
									{
										flag = true;
										break;
									}
								}
							}
						}
					}
					else
					{
						int phase = EMono.game.quests.GetPhase<QuestMain>();
						if ((phase == 0 && EMono.player.dialogFlags.TryGetValue("ash1", 0) == 0) || phase == 200)
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					emo = Emo2.hint;
				}
			}
		}
		this.showIcon = (emo > Emo2.none);
		if (this.showIcon)
		{
			this.iconStatus.sprite = EMono.core.refs.spritesEmo[(int)emo];
		}
		this.iconStatus.SetActive(this.showIcon);
		float num = (float)this.owner.hp / (float)this.owner.MaxHP;
		this.showHP = (num < 0.9f && (this.owner.IsPCParty || this.owner.IsHostile() || (this.owner.enemy != null && this.owner.enemy.IsPCParty)));
		if (this.showHP)
		{
			this.barHP.transform.SetLocalScaleX(num);
			this.barHP.SetActive(true);
			this.bgHP.SetActive(true);
		}
		else
		{
			this.barHP.SetActive(false);
			this.bgHP.SetActive(false);
		}
		if (!this.barHP2)
		{
			return;
		}
		float num2 = Mathf.Min(new float[]
		{
			1f,
			(this.owner.parasite != null) ? ((float)this.owner.parasite.hp / (float)this.owner.parasite.MaxHP) : 1f,
			(this.owner.ride != null) ? ((float)this.owner.ride.hp / (float)this.owner.ride.MaxHP) : 1f
		});
		this.showHP2 = (num2 < 0.9f);
		if (this.showHP2)
		{
			this.barHP2.transform.SetLocalScaleX(num2);
			this.barHP2.SetActive(true);
			this.bgHP2.SetActive(true);
			return;
		}
		this.barHP2.SetActive(false);
		this.bgHP2.SetActive(false);
	}

	public GameObject goIcon;

	public SpriteRenderer iconRelation;

	public SpriteRenderer iconStatus;

	public SpriteRenderer barHP;

	public SpriteRenderer bgHP;

	public SpriteRenderer barHP2;

	public SpriteRenderer bgHP2;

	public new Chara owner;

	private float timer;

	private bool showHP;

	private bool showIcon;

	private bool showHP2;
}
