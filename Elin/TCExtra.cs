using System;
using System.Collections.Generic;
using UnityEngine;

public class TCExtra : TC
{
	public override Vector3 FixPos
	{
		get
		{
			return this.fixPos;
		}
	}

	private void Awake()
	{
		this.originalScale = base.transform.localScale;
		this.originalAngle = base.transform.localEulerAngles;
	}

	public override void OnDraw(ref Vector3 pos)
	{
		if (this._mat != base.owner.material)
		{
			this._mat = base.owner.material;
			this.RefreshColor();
		}
		RenderData data = base.owner.renderer.data;
		bool flag = base.owner.dir == 2 || base.owner.dir == 3;
		bool flag2 = base.owner.parent is Chara && (base.owner.parent as Chara).held == base.owner;
		bool flag3 = base.owner.dir % 2 == 1;
		bool flag4 = (this.alwaysFlip || !flag2) && flag3;
		if (flag && this.flipBack)
		{
			flag4 = !flag4;
		}
		bool flag5 = this.useOffsetBack && data.useOffsetBack && flag;
		if (this.dirPos.Length == 0)
		{
			this.v.x = base.owner.renderer.position.x + (flag5 ? data.offsetBack.x : data.offset.x) * (float)(flag4 ? -1 : 1) + this.FixPos.x + this.flipFixX * (float)(flag4 ? -1 : 1) + (flag2 ? (flag3 ? this.heldPosFlip.x : this.heldPos.x) : 0f);
			this.v.y = base.owner.renderer.position.y + (flag5 ? data.offsetBack.y : data.offset.y) + this.FixPos.y + (flag2 ? (flag3 ? this.heldPosFlip.y : this.heldPos.y) : 0f);
			this.v.z = base.owner.renderer.position.z + (flag5 ? data.offsetBack.z : data.offset.z) + this.FixPos.z + (flag2 ? -0.5f : 0f) + (flag2 ? (flag3 ? this.heldPosFlip.z : this.heldPos.z) : 0f);
		}
		else
		{
			this.v.x = base.owner.renderer.position.x + (flag5 ? data.offsetBack.x : data.offset.x) + this.dirPos[base.owner.dir % this.dirPos.Length].x + (flag2 ? (flag3 ? this.heldPosFlip.x : this.heldPos.x) : 0f);
			this.v.y = base.owner.renderer.position.y + (flag5 ? data.offsetBack.y : data.offset.y) + this.dirPos[base.owner.dir % this.dirPos.Length].y + (flag2 ? (flag3 ? this.heldPosFlip.y : this.heldPos.y) : 0f);
			this.v.z = base.owner.renderer.position.z + (flag5 ? data.offsetBack.z : data.offset.z) + this.dirPos[base.owner.dir % this.dirPos.Length].z + (flag2 ? -0.5f : 0f) + (flag2 ? (flag3 ? this.heldPosFlip.z : this.heldPos.z) : 0f);
		}
		base.transform.position = this.v;
		if (this.flipSR && this.sr.flipX != flag4)
		{
			this.sr.flipX = flag4;
		}
		if (this.flipSelf)
		{
			base.transform.localScale = (flag4 ? new Vector3(this.originalScale.x * -1f, this.originalScale.y, this.originalScale.z) : this.originalScale);
			base.transform.localEulerAngles = (flag4 ? new Vector3(this.originalAngle.x * -1f, this.originalAngle.y, this.originalAngle.z) : this.originalAngle);
		}
		bool flag6 = (!this.onlyInstalled || base.owner.IsInstalled || base.owner.isRoofItem) && base.owner.trait.UseExtra;
		TCExtra.Type type = this.type;
		if (type != TCExtra.Type.Effect)
		{
			if (type != TCExtra.Type.QuestBoard)
			{
				goto IL_5CE;
			}
			flag6 = false;
			using (List<Quest>.Enumerator enumerator = EMono.game.quests.globalList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Quest quest = enumerator.Current;
					if (quest.IsVisibleOnQuestBoard() && quest.isNew)
					{
						flag6 = true;
					}
				}
				goto IL_5CE;
			}
		}
		if (flag6)
		{
			TraitEffect traitEffect = base.owner.trait as TraitEffect;
			if (traitEffect.timer < Time.realtimeSinceStartup)
			{
				traitEffect.timer = Time.realtimeSinceStartup - Time.realtimeSinceStartup % traitEffect.Interval + traitEffect.Interval + traitEffect.data.delay;
				traitEffect.Proc(this.v);
			}
		}
		IL_5CE:
		if (this.hack == TCExtra.Hack.Candle)
		{
			foreach (Transform transform in this.listTrans)
			{
				if (flag4)
				{
					if (transform.localPosition.z > 0f)
					{
						transform.localPosition = transform.localPosition.SetZ(-transform.localPosition.z);
					}
				}
				else if (transform.localPosition.z < 0f)
				{
					transform.localPosition = transform.localPosition.SetZ(-transform.localPosition.z);
				}
			}
		}
		if (base.owner.isHidden && base.owner.isChara && !EMono.pc.CanSee(base.owner.Chara))
		{
			flag6 = false;
		}
		if (base.gameObject.activeSelf != flag6)
		{
			base.gameObject.SetActive(flag6);
		}
	}

	public override void OnSetOwner()
	{
		base.OnSetOwner();
		this.RefreshColor();
	}

	public void RefreshColor()
	{
		if (this.sr)
		{
			Color color = base.owner.trait.ColorExtra ?? Color.white;
			color.a = this.sr.color.a;
			this.sr.color = color;
		}
		if (this.colorParticle)
		{
			ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				ParticleSystem.MainModule main = componentsInChildren[i].main;
				main.startColor = (base.owner.isDyed ? base.owner.DyeMat : base.owner.material).GetColor().SetAlpha(main.startColor.color.a);
			}
		}
	}

	public TCExtra.Type type;

	public TCExtra.Hack hack;

	public Vector3 fixPos;

	public Vector3 heldPos;

	public Vector3 heldPosFlip;

	public float flipFixX;

	public bool flipSR;

	public bool flipSelf;

	public bool onlyInstalled = true;

	public bool useOffsetBack = true;

	public bool alwaysFlip;

	public bool flipBack;

	public bool colorParticle;

	public SpriteRenderer sr;

	public Transform[] listTrans;

	public Vector3[] dirPos;

	private SourceMaterial.Row _mat;

	private Vector3 v;

	private Vector3 originalScale;

	private Vector3 originalAngle;

	public enum Type
	{
		None,
		Light,
		Effect,
		QuestBoard
	}

	public enum Hack
	{
		None,
		Candle
	}
}
