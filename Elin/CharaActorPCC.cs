using UnityEngine;

public class CharaActorPCC : CharaActor
{
	private static Vector3 org;

	public PCC pcc;

	public SpriteProvider provider = new SpriteProvider();

	private float lastNextFrame;

	public static Vector3 V2 = new Vector3(2f, 2f, 1f);

	public override bool isPCC => true;

	public override int currentDir => provider.currentDir;

	public override void OnSetOwner()
	{
		SetOwnerAsChara();
		sr.flipX = false;
		pcc = PCC.Get((owner.renderer as CharaRenderer).pccData);
		provider.onSetSprite = delegate(Sprite a)
		{
			sr.sprite = a;
			mpb.SetTexture("_MainTex", sr.sprite.texture);
		};
		provider.angle = owner.angle;
		provider.SetPCC(pcc);
		mpb.SetTexture("_MainTex", sr.sprite.texture);
		IdleFrame();
	}

	public override void NextFrame()
	{
		if (lastNextFrame != Time.timeSinceLevelLoad)
		{
			provider.NextFrame();
			lastNextFrame = Time.timeSinceLevelLoad;
		}
	}

	public override void IdleFrame()
	{
		if (!owner.isDead)
		{
			provider.SetSpriteIdle();
		}
	}

	public override int GetFrame()
	{
		return provider.currentFrame % 2;
	}

	public override void NextDir()
	{
		provider.NextDir();
		owner.angle = provider.angle;
		provider.SetDir();
		provider.SetSpriteMain();
		RefreshSprite();
	}

	public override void Kill()
	{
		base.Kill();
		pcc = null;
		provider.pcc = null;
	}

	public override void OnRender(RenderParam p)
	{
		base.OnRender(p);
		if (pcc.data.isUnique)
		{
			return;
		}
		org.x = p.x;
		org.y = p.y;
		org.z = p.z + owner.renderer.data.offset.z;
		bool flag = owner.combatCount <= 0 && EMono.core.config.game.hideWeapons;
		Cell cell = owner.Cell;
		if ((owner.held == null || (owner.held.trait.ShowAsTool && !HotItemHeld.disableTool)) && !owner.IsDeadOrSleeping && pcc.data.state != PCCState.Naked && (owner.Cell.isFloating || !cell.sourceSurface.tileType.IsDeepWater || cell.IsIceTile))
		{
			int num = currentDir;
			int tempLeft = pcc.data.tempLeft;
			int tempRight = pcc.data.tempRight;
			switch (tempRight)
			{
			default:
			{
				RenderData renderTempEQ = EMono.scene.screenElin.renderTempEQ;
				bool flag3 = num == 0 || num == 1;
				Vector3[] mainHandPos2 = EMono.setting.render.mainHandPos;
				Vector3[] mainHand2 = EMono.setting.render.animeWalk[provider.currentFrame].mainHand;
				p.tile = tempRight * (flag3 ? 1 : (-1));
				p.matColor = 104025f;
				p.x = org.x + mainHandPos2[num].x + mainHand2[num].x;
				p.y = org.y + mainHandPos2[num].y + mainHand2[num].y;
				p.z = org.z - renderTempEQ.offset.z + mainHandPos2[num].z + mainHand2[num].z;
				renderTempEQ.Draw(p);
				break;
			}
			case 0:
			{
				Thing thing = ((owner.IsPC && EMono.player.currentHotItem.RenderThing != null) ? EMono.player.currentHotItem.RenderThing : ((!flag && owner.body.slotMainHand?.thing != null) ? owner.body.slotMainHand.thing : null));
				if (thing != null)
				{
					bool flag2 = num == 0 || num == 1;
					if (thing.trait.InvertHeldSprite)
					{
						flag2 = !flag2;
					}
					Vector3[] mainHandPos = EMono.setting.render.mainHandPos;
					Vector3[] mainHand = EMono.setting.render.animeWalk[provider.currentFrame].mainHand;
					SourcePref pref = thing.sourceCard.pref;
					thing.dir = ((!flag2) ? 1 : 0);
					thing.SetRenderParam(p);
					p.x = org.x + mainHandPos[num].x + mainHand[num].x + (flag2 ? 0.01f : (-0.01f)) * (float)pref.equipX;
					p.y = org.y + mainHandPos[num].y + mainHand[num].y + 0.01f * (float)pref.equipY;
					p.z = org.z - thing.renderer.data.offset.z + mainHandPos[num].z + mainHand[num].z;
					p.v.x = p.x;
					p.v.y = p.y;
					p.v.z = p.z;
					thing.renderer.Draw(p, ref p.v, drawShadow: false);
				}
				break;
			}
			case -1:
				break;
			}
			switch (tempLeft)
			{
			default:
			{
				RenderData renderTempEQ2 = EMono.scene.screenElin.renderTempEQ;
				bool flag5 = num == 1 || num == 3;
				Vector3[] offHandPos2 = EMono.setting.render.offHandPos;
				Vector3[] offHand2 = EMono.setting.render.animeWalk[provider.currentFrame].offHand;
				p.tile = tempLeft * (flag5 ? 1 : (-1));
				p.matColor = 104025f;
				p.x = org.x + offHandPos2[num].x + offHand2[num].x;
				p.y = org.y + offHandPos2[num].y + offHand2[num].y;
				p.z = org.z - renderTempEQ2.offset.z + offHandPos2[num].z + offHand2[num].z;
				renderTempEQ2.Draw(p);
				break;
			}
			case 0:
				if (!flag && owner.body.slotOffHand != null && EMono.core.config.game.showOffhand)
				{
					Thing thing2 = owner.body.slotOffHand.thing;
					if (thing2 != null)
					{
						bool flag4 = num == 1 || num == 3;
						Vector3[] offHandPos = EMono.setting.render.offHandPos;
						Vector3[] offHand = EMono.setting.render.animeWalk[provider.currentFrame].offHand;
						SourcePref pref2 = thing2.source.pref;
						thing2.dir = ((!flag4) ? 1 : 0);
						thing2.SetRenderParam(p);
						p.x = org.x + offHandPos[num].x + offHand[num].x + (flag4 ? 0.01f : (-0.01f)) * (float)pref2.equipX;
						p.y = org.y + offHandPos[num].y + offHand[num].y + 0.01f * (float)pref2.equipY;
						p.z = org.z - thing2.renderer.data.offset.z + offHandPos[num].z + offHand[num].z;
						thing2.renderer.Draw(p);
					}
				}
				break;
			case -1:
				break;
			}
		}
		p.x = org.x;
		p.y = org.y;
		p.z = org.z - owner.renderer.data.offset.z;
	}

	public override void RefreshSprite()
	{
		provider.angle = owner.angle;
		provider.SetDir();
		provider.SetSpriteMain();
		base.RefreshSprite();
	}
}
