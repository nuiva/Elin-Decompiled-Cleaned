using System;
using UnityEngine;

public class CharaRenderer : CardRenderer
{
	public int currentDir
	{
		get
		{
			if (this.hasActor)
			{
				return this.actor.currentDir;
			}
			if (!this.owner.flipX)
			{
				return 1;
			}
			return 0;
		}
	}

	public override void SetOwner(Card c)
	{
		this.owner = (c as Chara);
		if (this.pccData != null)
		{
			this.data = ((this.pccData.GetBodySet2().id == "unique") ? EClass.core.refs.renderers.pcc_L : EClass.core.refs.renderers.pcc);
		}
		base.SetOwner(c);
	}

	public override void OnEnterScreen()
	{
		base.OnEnterScreen();
		if (!this.ignoreFirst)
		{
			this.first = true;
		}
		this.ignoreFirst = false;
		this.nextframeTimer = 0f;
		if (EClass.core.config.game.haltOnSpotEnemy && this.owner.ExistsOnMap && !EClass._zone.IsRegion && this.owner.IsHostile())
		{
			EClass.player.enemySpotted = true;
		}
	}

	public override void Draw(RenderParam p, ref Vector3 v, bool drawShadow)
	{
		base.Draw(p, ref v, drawShadow);
		if (Zone.sourceHat != null && this.owner.Pref.hatY != 0f && this.owner.host == null)
		{
			this.DrawHat();
		}
		if (this.owner.IsPC)
		{
			if (this.owner.held != null && !this.owner.IsDeadOrSleeping && (!this.owner.held.trait.ShowAsTool || HotItemHeld.disableTool))
			{
				this.DrawHeld();
				return;
			}
		}
		else if (this.owner.held != null && !this.owner.IsDeadOrSleeping && !this.owner.held.trait.ShowAsTool)
		{
			this.DrawHeld();
		}
	}

	public override void NextFrame()
	{
		base.NextFrame();
		this.owner.idleTimer = RenderObject.animeSetting.idleTime;
	}

	public override bool IsMoving
	{
		get
		{
			return this.isMoving;
		}
	}

	public override void UpdatePosition(ref Vector3 destPos, RenderParam p)
	{
		bool isPC = this.owner.IsPC;
		int num = isPC ? CharaRenderer._animeFramePC : (this.hasActor ? CharaRenderer._animeFramePCC : CharaRenderer._animeFrame);
		bool flag = isPC || num >= 10;
		if (num == 0 || Scene.skipAnime || this.first || !this.data.animate || (this.owner.IsDeadOrSleeping && this.pccData != null && !this.owner.IsPC))
		{
			this.first = false;
			this.position = destPos;
			if (isPC)
			{
				EClass.player.position = this.position;
			}
			this.movePoint.Set(this.owner.pos);
			this.step = 9999999;
			this.isMoving = false;
			this.moveTimer = 0f;
			base.RefreshSprite();
			p.x = this.position.x;
			p.y = this.position.y;
			p.z = this.position.z;
			this.lastShadowFix = (this.lastShadowFix2 = p.shadowFix);
			return;
		}
		if (!this.owner.pos.Equals(this.movePoint))
		{
			if (RenderObject.gameSpeed > 1f)
			{
				p.shadowFix = (this.lastShadowFix = 0f);
			}
			else
			{
				p.shadowFix = (this.lastShadowFix = this.lastShadowFix2);
			}
			this.movePoint.Set(this.owner.pos);
			this.orgPos = this.position;
			this.actTime = this.owner.actTime;
			base.RefreshSprite();
			this.step = 1;
			if (!this.isMoving)
			{
				this.isMoving = true;
				this.nextframeTimer = (RenderObject.animeSetting.nextFrameInterval + this.actTime / 4f) / 2f;
			}
			this.moveTimer = 0f;
		}
		if (num < 100)
		{
			num = (int)((float)num / 0.3f * this.actTime);
		}
		float num2 = (this.actTime + ((!isPC && this.owner.IsPCParty) ? RenderObject.animeSetting.animeExtraTimeParty : RenderObject.animeSetting.animeExtraTime) * RenderObject.gameSpeed) / (float)num;
		this.moveTimer += RenderObject.gameDelta;
		if (this.step >= num)
		{
			if (this.position == destPos)
			{
				if (this.isMoving && isPC)
				{
					EClass.player.position = this.position;
				}
				this.isMoving = false;
				this.moveTimer = 0f;
			}
			else if (!flag)
			{
				this.position = destPos;
			}
			else if (this.moveTimer >= num2)
			{
				int num3 = (int)(this.moveTimer / num2);
				this.moveTimer -= (float)num3 * num2;
				if (Vector3.Distance(this.position, destPos) < RenderObject.animeSetting.destRadius)
				{
					this.position = destPos;
					if (this.isMoving && isPC)
					{
						EClass.player.position = this.position;
					}
					this.isMoving = false;
				}
				else
				{
					this.position.z = destPos.z;
					Vector3 position = Vector3.MoveTowards(this.position, destPos, (float)num3 * RenderObject.animeSetting.slowSpeed / (float)this.step);
					this.position = position;
				}
			}
			if (this.owner.idleTimer > 0f)
			{
				this.owner.idleTimer -= RenderObject.gameDelta;
				if (flag && this.owner.idleTimer <= 0f)
				{
					this.IdleFrame();
					this.nextframeTimer = 0f;
				}
			}
			p.x = this.position.x;
			p.y = this.position.y;
			p.z = this.position.z;
			this.lastShadowFix = (this.lastShadowFix2 = p.shadowFix);
			if (isPC)
			{
				CellDetail detail = EClass.pc.Cell.detail;
				if (detail == null || detail.anime == null)
				{
					this.position.z = destPos.z;
					Vector3 position2 = Vector3.MoveTowards(this.position, destPos, 0.1f);
					this.position = position2;
					EClass.player.position = Vector3.Lerp(EClass.player.position, this.position, 0.6f);
				}
			}
			return;
		}
		if (this.moveTimer >= num2)
		{
			int num4 = (int)(this.moveTimer / num2);
			this.step += num4;
			this.moveTimer -= (float)num4 * num2;
			if (this.step >= num)
			{
				this.step = num;
			}
		}
		this.owner.idleTimer = RenderObject.animeSetting.idleTime;
		float num5 = (float)this.step / (float)num;
		p.shadowFix = p.shadowFix * num5 + this.lastShadowFix * (1f - num5);
		this.lastShadowFix2 = p.shadowFix;
		Vector3 position3 = this.orgPos + (destPos - this.orgPos) * num5 * ((flag && CharaRenderer.smoothmove) ? RenderObject.animeSetting.fixedMove : 1f);
		if (destPos.z < this.orgPos.z)
		{
			position3.z = this.orgPos.z + (destPos.z - this.orgPos.z) * RenderObject.animeSetting.gradientZForward.Evaluate(num5);
		}
		else
		{
			position3.z = this.orgPos.z + (destPos.z - this.orgPos.z) * RenderObject.animeSetting.gradientZBack.Evaluate(num5);
		}
		this.position = position3;
		if (this.hasActor)
		{
			if (EClass.core.config.graphic.spriteFrameMode == 0)
			{
				this.nextframeTimer += RenderObject.gameDelta * 0.5f;
				float nextFrameInterval = RenderObject.animeSetting.nextFrameInterval;
				if (this.nextframeTimer > nextFrameInterval)
				{
					if (this.owner.ai is AI_Trolley)
					{
						this.actor.IdleFrame();
					}
					else
					{
						this.actor.NextFrame();
					}
					this.nextframeTimer -= nextFrameInterval;
				}
			}
			else
			{
				this.nextframeTimer += RenderObject.gameDelta;
				float num6 = RenderObject.animeSetting.nextFrameInterval + this.actTime / 4f * (0.5f + RenderObject.gameSpeed / 2f);
				if (this.nextframeTimer > num6)
				{
					if (this.owner.ai is AI_Trolley)
					{
						this.actor.IdleFrame();
					}
					else
					{
						this.actor.NextFrame();
					}
					this.nextframeTimer -= num6;
				}
			}
		}
		p.x = this.position.x;
		p.y = this.position.y;
		p.z = this.position.z;
		if (isPC)
		{
			EClass.player.position = position3;
			return;
		}
		if (!this.hasActor && num >= 5 && this.hopCurve != null)
		{
			p.y += this.hopCurve.Evaluate(num5) * RenderObject.animeSetting.hopStrength;
		}
	}

	public void DrawHat()
	{
		if (this.pccData != null && this.owner.IsDeadOrSleeping)
		{
			return;
		}
		CardRow sourceHat = Zone.sourceHat;
		SourcePref pref = sourceHat.pref;
		bool flag = this.currentDir == 1 || this.currentDir == 3;
		int liquidLv = RenderObject.currentParam.liquidLv;
		float num = (this.replacer != null) ? this.replacer.pref.hatY : this.owner.Pref.hatY;
		if (this.pccData != null)
		{
			num += RenderObject.renderSetting.hatPos[this.actor.GetFrame()].y;
		}
		RenderObject.currentParam.liquidLv = 0;
		RenderObject.currentParam.y += num;
		RenderObject.currentParam.tile = (float)(sourceHat._tiles[this.owner.uid % sourceHat._tiles.Length] * (flag ? -1 : 1));
		sourceHat.renderData.Draw(RenderObject.currentParam);
		RenderObject.currentParam.y -= num;
		RenderObject.currentParam.liquidLv = liquidLv;
	}

	public override void DrawHeld()
	{
		int currentDir = this.currentDir;
		RenderData data = this.owner.held.renderer.data;
		SourcePref pref = this.owner.held.Pref;
		bool flag = currentDir == 1 || currentDir == 3;
		if (this.owner.held.isChara)
		{
			Vector3[] array = EClass.player.altHeldPos ? RenderObject.renderSetting.heldPosChara2 : RenderObject.renderSetting.heldPosChara;
			RenderObject.currentParam.x += array[currentDir].x;
			RenderObject.currentParam.y += array[currentDir].y;
			RenderObject.currentParam.z += array[currentDir].z + this.data.offset.z - data.offset.z;
			if (EClass.player.altHeldPos)
			{
				this.owner.held.SetDir((this.owner.dir == 0) ? 2 : ((this.owner.dir == 1) ? 1 : ((this.owner.dir == 2) ? 0 : 3)));
			}
			else
			{
				this.owner.held.SetDir((this.owner.dir == 0) ? 2 : this.owner.dir);
			}
		}
		else
		{
			Vector3[] heldPos = RenderObject.renderSetting.heldPos;
			if (this.hasActor)
			{
				RenderObject.currentParam.x += heldPos[currentDir].x;
				RenderObject.currentParam.y += heldPos[currentDir].y + 0.02f * (float)(this.actor.GetFrame() % 2);
				RenderObject.currentParam.z += heldPos[currentDir].z + this.data.offset.z - data.offset.z;
			}
			else
			{
				RenderObject.currentParam.x += heldPos[4].x;
				RenderObject.currentParam.y += heldPos[4].y;
				RenderObject.currentParam.z += heldPos[4].z + this.data.offset.z - data.offset.z;
			}
			if (this.owner.held.trait.NoHeldDir || !this.owner.held.sourceCard.ContainsTag("noHeldDir"))
			{
				this.owner.held.dir = (this.owner.flipX ? 1 : 0);
			}
		}
		if (this.owner.Cell.sourceSurface.tileType.IsDeepWater)
		{
			return;
		}
		int liquidLv = RenderObject.currentParam.liquidLv;
		if (this.owner.held.isChara)
		{
			RenderObject.currentParam.liquidLv = 0;
		}
		this.owner.held.SetRenderParam(RenderObject.currentParam);
		RenderObject.currentParam.x += data.heldPos.x - data.offset.x + (flag ? 0.01f : -0.01f) * (float)pref.equipX;
		RenderObject.currentParam.y += data.heldPos.y - data.offset.y + 0.01f * (float)pref.equipY;
		RenderObject.currentParam.z += data.heldPos.z;
		RenderObject.tempV.x = RenderObject.currentParam.x;
		RenderObject.tempV.y = RenderObject.currentParam.y;
		RenderObject.tempV.z = RenderObject.currentParam.z;
		this.owner.held.renderer.Draw(RenderObject.currentParam, ref RenderObject.tempV, false);
		RenderObject.currentParam.liquidLv = liquidLv;
	}

	public override void RefreshStateIcon()
	{
		if (!this.isSynced)
		{
			return;
		}
		TCState tcstate = base.GetTC<TCState>();
		MultiSprite stateIcon = this.owner.ai.GetStateIcon();
		if (stateIcon == null)
		{
			if (tcstate)
			{
				base.RemoveTC(tcstate);
			}
			return;
		}
		if (!tcstate)
		{
			tcstate = base.AddTC<TCState>(PoolManager.Spawn<TCState>(EClass.core.refs.tcs.state, null));
		}
		tcstate.SetSprite(stateIcon);
	}

	public override void SetFirst(bool first)
	{
		this.first = first;
		this.ignoreFirst = !first;
	}

	public override void SetFirst(bool first, Vector3 pos)
	{
		this.first = first;
		this.ignoreFirst = !first;
		this.position = pos;
		if (this.owner.IsPC)
		{
			EClass.player.position = this.position;
		}
		this.movePoint.Set(this.owner.pos);
		this.step = 9999999;
		this.isMoving = false;
		this.moveTimer = 0f;
	}

	public override void Refresh()
	{
	}

	public new Chara owner;

	public Vector3 orgPos;

	private float moveTimer;

	private float nextframeTimer;

	public bool first = true;

	public bool ignoreFirst;

	public Point movePoint = new Point();

	public AnimationCurve hopCurve;

	public PCCData pccData;

	private float actTime = 0.3f;

	private float lastShadowFix;

	private float lastShadowFix2;

	public int step;

	public bool isMoving;

	public static int _animeFramePC;

	public static int _animeFramePCC;

	public static int _animeFrame;

	public static bool smoothmove;
}
