using System;
using UnityEngine;

public class CharaActorPCC : CharaActor
{
	public override bool isPCC
	{
		get
		{
			return true;
		}
	}

	public override int currentDir
	{
		get
		{
			return this.provider.currentDir;
		}
	}

	public override void OnSetOwner()
	{
		base.SetOwnerAsChara();
		this.sr.flipX = false;
		this.pcc = PCC.Get((this.owner.renderer as CharaRenderer).pccData);
		this.provider.onSetSprite = delegate(Sprite a)
		{
			this.sr.sprite = a;
			this.mpb.SetTexture("_MainTex", this.sr.sprite.texture);
		};
		this.provider.angle = this.owner.angle;
		this.provider.SetPCC(this.pcc);
		this.mpb.SetTexture("_MainTex", this.sr.sprite.texture);
		this.IdleFrame();
	}

	public override void NextFrame()
	{
		if (this.lastNextFrame == Time.timeSinceLevelLoad)
		{
			return;
		}
		this.provider.NextFrame();
		this.lastNextFrame = Time.timeSinceLevelLoad;
	}

	public override void IdleFrame()
	{
		if (!this.owner.isDead)
		{
			this.provider.SetSpriteIdle();
		}
	}

	public override int GetFrame()
	{
		return this.provider.currentFrame % 2;
	}

	public override void NextDir()
	{
		this.provider.NextDir(15f);
		this.owner.angle = this.provider.angle;
		this.provider.SetDir();
		this.provider.SetSpriteMain();
		this.RefreshSprite();
	}

	public override void Kill()
	{
		base.Kill();
		this.pcc = null;
		this.provider.pcc = null;
	}

	public override void OnRender(RenderParam p)
	{
		base.OnRender(p);
		if (this.pcc.data.isUnique)
		{
			return;
		}
		CharaActorPCC.org.x = p.x;
		CharaActorPCC.org.y = p.y;
		CharaActorPCC.org.z = p.z + this.owner.renderer.data.offset.z;
		bool flag = this.owner.combatCount <= 0 && EMono.core.config.game.hideWeapons;
		Cell cell = this.owner.Cell;
		if ((this.owner.held == null || (this.owner.held.trait.ShowAsTool && !HotItemHeld.disableTool)) && !this.owner.IsDeadOrSleeping && this.pcc.data.state != PCCState.Naked && (this.owner.Cell.isFloating || !cell.sourceSurface.tileType.IsDeepWater || cell.IsIceTile))
		{
			int currentDir = this.currentDir;
			int tempLeft = this.pcc.data.tempLeft;
			int tempRight = this.pcc.data.tempRight;
			if (tempRight != 0)
			{
				if (tempRight != -1)
				{
					RenderData renderTempEQ = EMono.scene.screenElin.renderTempEQ;
					bool flag2 = currentDir == 0 || currentDir == 1;
					Vector3[] mainHandPos = EMono.setting.render.mainHandPos;
					Vector3[] mainHand = EMono.setting.render.animeWalk[this.provider.currentFrame].mainHand;
					p.tile = (float)(tempRight * (flag2 ? 1 : -1));
					p.matColor = 104025f;
					p.x = CharaActorPCC.org.x + mainHandPos[currentDir].x + mainHand[currentDir].x;
					p.y = CharaActorPCC.org.y + mainHandPos[currentDir].y + mainHand[currentDir].y;
					p.z = CharaActorPCC.org.z - renderTempEQ.offset.z + mainHandPos[currentDir].z + mainHand[currentDir].z;
					renderTempEQ.Draw(p);
				}
			}
			else
			{
				Thing thing;
				if (!this.owner.IsPC || EMono.player.currentHotItem.RenderThing == null)
				{
					if (!flag)
					{
						BodySlot slotMainHand = this.owner.body.slotMainHand;
						if (((slotMainHand != null) ? slotMainHand.thing : null) != null)
						{
							thing = this.owner.body.slotMainHand.thing;
							goto IL_2C9;
						}
					}
					thing = null;
				}
				else
				{
					thing = EMono.player.currentHotItem.RenderThing;
				}
				IL_2C9:
				Thing thing2 = thing;
				if (thing2 != null)
				{
					bool flag3 = currentDir == 0 || currentDir == 1;
					if (thing2.trait.InvertHeldSprite)
					{
						flag3 = !flag3;
					}
					Vector3[] mainHandPos2 = EMono.setting.render.mainHandPos;
					Vector3[] mainHand2 = EMono.setting.render.animeWalk[this.provider.currentFrame].mainHand;
					SourcePref pref = thing2.sourceCard.pref;
					thing2.dir = (flag3 ? 0 : 1);
					thing2.SetRenderParam(p);
					p.x = CharaActorPCC.org.x + mainHandPos2[currentDir].x + mainHand2[currentDir].x + (flag3 ? 0.01f : -0.01f) * (float)pref.equipX;
					p.y = CharaActorPCC.org.y + mainHandPos2[currentDir].y + mainHand2[currentDir].y + 0.01f * (float)pref.equipY;
					p.z = CharaActorPCC.org.z - thing2.renderer.data.offset.z + mainHandPos2[currentDir].z + mainHand2[currentDir].z;
					p.v.x = p.x;
					p.v.y = p.y;
					p.v.z = p.z;
					thing2.renderer.Draw(p, ref p.v, false);
				}
			}
			if (tempLeft != 0)
			{
				if (tempLeft != -1)
				{
					RenderData renderTempEQ2 = EMono.scene.screenElin.renderTempEQ;
					bool flag4 = currentDir == 1 || currentDir == 3;
					Vector3[] offHandPos = EMono.setting.render.offHandPos;
					Vector3[] offHand = EMono.setting.render.animeWalk[this.provider.currentFrame].offHand;
					p.tile = (float)(tempLeft * (flag4 ? 1 : -1));
					p.matColor = 104025f;
					p.x = CharaActorPCC.org.x + offHandPos[currentDir].x + offHand[currentDir].x;
					p.y = CharaActorPCC.org.y + offHandPos[currentDir].y + offHand[currentDir].y;
					p.z = CharaActorPCC.org.z - renderTempEQ2.offset.z + offHandPos[currentDir].z + offHand[currentDir].z;
					renderTempEQ2.Draw(p);
				}
			}
			else if (!flag && this.owner.body.slotOffHand != null && EMono.core.config.game.showOffhand)
			{
				Thing thing3 = this.owner.body.slotOffHand.thing;
				if (thing3 != null)
				{
					bool flag5 = currentDir == 1 || currentDir == 3;
					Vector3[] offHandPos2 = EMono.setting.render.offHandPos;
					Vector3[] offHand2 = EMono.setting.render.animeWalk[this.provider.currentFrame].offHand;
					SourcePref pref2 = thing3.source.pref;
					thing3.dir = (flag5 ? 0 : 1);
					thing3.SetRenderParam(p);
					p.x = CharaActorPCC.org.x + offHandPos2[currentDir].x + offHand2[currentDir].x + (flag5 ? 0.01f : -0.01f) * (float)pref2.equipX;
					p.y = CharaActorPCC.org.y + offHandPos2[currentDir].y + offHand2[currentDir].y + 0.01f * (float)pref2.equipY;
					p.z = CharaActorPCC.org.z - thing3.renderer.data.offset.z + offHandPos2[currentDir].z + offHand2[currentDir].z;
					thing3.renderer.Draw(p);
				}
			}
		}
		p.x = CharaActorPCC.org.x;
		p.y = CharaActorPCC.org.y;
		p.z = CharaActorPCC.org.z - this.owner.renderer.data.offset.z;
	}

	public override void RefreshSprite()
	{
		this.provider.angle = this.owner.angle;
		this.provider.SetDir();
		this.provider.SetSpriteMain();
		base.RefreshSprite();
	}

	private static Vector3 org;

	public PCC pcc;

	public SpriteProvider provider = new SpriteProvider();

	private float lastNextFrame;

	public static Vector3 V2 = new Vector3(2f, 2f, 1f);
}
