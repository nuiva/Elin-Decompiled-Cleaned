using System;
using UnityEngine;

public class CardActor : Actor
{
	public virtual bool isPCC
	{
		get
		{
			return false;
		}
	}

	public virtual int currentDir
	{
		get
		{
			return 0;
		}
	}

	private void Awake()
	{
		this.originalScale = base.transform.localScale;
	}

	public void SetOwner(Card t)
	{
		base.gameObject.SetActive(true);
		this.owner = t;
		if (this.mpb == null)
		{
			this.mpb = new MaterialPropertyBlock();
		}
		this.mpb.SetColor("_Mat", EMono.Colors.matColors.TryGetValue(t.material.alias, null).main);
		if (this.sr)
		{
			this.sr.SetPropertyBlock(this.mpb);
		}
		if (this.sr2)
		{
			if (this.mpb2 == null)
			{
				this.mpb2 = new MaterialPropertyBlock();
			}
			this.mpb2.SetColor("_Mat", Color.white);
			this.sr2.SetPropertyBlock(this.mpb2);
		}
		this.OnSetOwner();
		if (Application.isEditor)
		{
			base.gameObject.name = this.owner.Name;
		}
		if (this.type == CardActor.Type.Boat)
		{
			this.srAnime.SetData(this.owner.id + "_anime");
		}
		this.RefreshSprite();
	}

	public virtual void OnSetOwner()
	{
	}

	public virtual void IdleFrame()
	{
	}

	public virtual void NextFrame()
	{
	}

	public virtual void NextDir()
	{
	}

	public virtual int GetFrame()
	{
		return 0;
	}

	public virtual void OnRender(RenderParam p)
	{
		this.SetActive(true);
		SubPassData current = SubPassData.Current;
		RenderData data = this.owner.renderer.data;
		if (data.useOffsetBack && this.owner != EMono.pc.held && (data.symmetry ? (p.dir % 2 == 1) : (p.dir == 1 || p.dir == 2)))
		{
			CardActor.tempV.x = p.x + data.offsetBack.x + current.offset.x;
			CardActor.tempV.y = p.y + data.offsetBack.y + current.offset.y;
			CardActor.tempV.z = p.z + data.offsetBack.z + current.offset.z;
		}
		else
		{
			CardActor.tempV.x = p.x + data.offset.x + current.offset.x;
			CardActor.tempV.y = p.y + data.offset.y + current.offset.y;
			CardActor.tempV.z = p.z + data.offset.z + current.offset.z;
		}
		if (this.isPCC)
		{
			RenderDataPcc renderDataPcc = data as RenderDataPcc;
			CharaActorPCC charaActorPCC = this as CharaActorPCC;
			CardActor.tempV.x = (float)((int)(CardActor.tempV.x * 100f)) * 0.01f;
			CardActor.tempV.y = (float)((int)(CardActor.tempV.y * 100f)) * 0.01f;
			CardActor.tempV2.x = charaActorPCC.provider.vCurrent.source.size.x * current.scale.x;
			CardActor.tempV2.y = charaActorPCC.provider.vCurrent.source.size.y * current.scale.y;
			CardActor.tempV2.z = renderDataPcc._scale.z * current.scale.z;
			base.transform.localRotation = current.rotation;
		}
		else
		{
			CardActor.tempV2.x = this.originalScale.x * current.scale.x;
			CardActor.tempV2.y = this.originalScale.y * current.scale.y;
			CardActor.tempV2.z = this.originalScale.z * current.scale.z;
		}
		if (this.type == CardActor.Type.Boat)
		{
			float floatY = EMono.scene.screenElin.tileMap.floatY;
			int floatV = EMono.scene.screenElin.tileMap.floatV;
			this.sr2.transform.localPosition = new Vector3(0f, -0.005f * floatY, this.sr2.transform.localPosition.z);
			if (floatV == 1)
			{
				if (floatY < 2f)
				{
				}
			}
			else if (floatY < 3f)
			{
			}
			int index;
			if (floatV == 1)
			{
				if (floatY < 0f)
				{
					index = 0;
				}
				else if (floatY < 3f)
				{
					index = 1;
				}
				else
				{
					index = 2;
				}
			}
			else if (floatY < 2f)
			{
				index = 0;
			}
			else if (floatY < 5f)
			{
				index = 3;
			}
			else
			{
				index = 2;
			}
			this.srAnime.sr.sprite = this.srAnime.data.GetSprites().TryGet(index, -1);
		}
		base.transform.position = CardActor.tempV;
		base.transform.localScale = CardActor.tempV2;
		this.destLiquid = Mathf.Lerp(this.destLiquid, (float)p.liquidLv, Core.gameDelta * (float)((this.destLiquid > (float)p.liquidLv) ? 90 : 20));
		this.mpb.SetFloat("_MatColor", p.matColor);
		this.mpb.SetFloat("_Color", p.color);
		this.mpb.SetFloat("_Liquid", this.destLiquid);
		this.sr.SetPropertyBlock(this.mpb);
		if (this.sr2)
		{
			if (this.sr2.sprite)
			{
				this.mpb2.SetTexture("_MainTex", this.sr2.sprite.texture);
			}
			this.mpb2.SetTexture("_MaskTex", this.sr.sprite.texture);
			this.mpb2.SetFloat("_MatColor", p.matColor);
			this.mpb2.SetFloat("_Color", p.color);
			this.mpb2.SetFloat("_Liquid", this.destLiquid);
			this.sr2.SetPropertyBlock(this.mpb2);
		}
		if (this.owner.sourceCard.replacer.data != null)
		{
			SpriteData data2 = this.owner.sourceCard.replacer.data;
			if (data2.frame > 1)
			{
				this.spriteTimer += Core.delta;
				if (this.spriteTimer >= data2.time)
				{
					this.spriteTimer -= data2.time;
					this.spriteIndex++;
					if (this.spriteIndex >= data2.frame)
					{
						this.spriteIndex = 0;
					}
					this.sr.sprite = ((data2.spritesSnow != null && p.snow) ? data2.spritesSnow[this.spriteIndex] : data2.sprites[this.spriteIndex]);
				}
			}
		}
	}

	public virtual void RefreshSprite()
	{
		switch (this.type)
		{
		case CardActor.Type.Boat:
			this.sr2.SetActive(this.owner.Cell.IsFloorWater);
			break;
		case CardActor.Type.Canvas:
			if (this.owner != null && this.owner.c_textureData != null && this.sr2)
			{
				this.sr2.sprite = this.owner.GetPaintSprite();
				TraitCanvas traitCanvas = this.owner.trait as TraitCanvas;
				if (traitCanvas != null)
				{
					this.sr2.transform.SetLocalScale(traitCanvas.Scale, traitCanvas.Scale, 1f);
				}
			}
			break;
		case CardActor.Type.MultiSprite:
			this.sr2.sprite = this.owner.GetSprite(0);
			break;
		}
		int dir = this.owner.dir;
		Sprite sprite = this.sr.sprite = (this.sr.sprite = this.owner.GetSprite(dir));
		this.sr.flipX = ((dir == 1 || dir == 3) && (this.owner.Thing == null || !this.owner.Thing.isEquipped));
		this.mpb.SetTexture("_MainTex", sprite.texture);
		Vector4 value = new Vector4(sprite.textureRect.min.x / (float)sprite.texture.width, sprite.textureRect.min.y / (float)sprite.texture.height, sprite.textureRect.max.x / (float)sprite.texture.width, sprite.textureRect.max.y / (float)sprite.texture.height);
		this.mpb.SetVector("_Rect", value);
		this.mpb.SetFloat("_PixelHeight", sprite.rect.height);
		if (this.sr2)
		{
			if (this.sr2.sprite && this.sr2.sprite.texture)
			{
				this.sr2.flipX = (dir == 1 || dir == 3);
				sprite = this.sr2.sprite;
				if (sprite)
				{
					Rect textureRect = sprite.textureRect;
					value = new Vector4(sprite.textureRect.min.x / (float)sprite.texture.width, sprite.textureRect.min.y / (float)sprite.texture.height, sprite.textureRect.max.x / (float)sprite.texture.width, sprite.textureRect.max.y / (float)sprite.texture.height);
					this.mpb2.SetTexture("_MainTex", sprite.texture);
					this.mpb2.SetVector("_Rect", value);
					this.mpb2.SetFloat("_PixelHeight", sprite.rect.height);
					return;
				}
			}
			else
			{
				this.sr2.sprite = null;
			}
		}
	}

	public virtual void Kill()
	{
		PoolManager.Despawn(base.transform);
	}

	public Card owner;

	public SpriteRenderer sr;

	public SpriteRenderer sr2;

	public SpriteReplacerAnimation srAnime;

	public MaterialPropertyBlock mpb;

	public MaterialPropertyBlock mpb2;

	public CardActor.Type type;

	private float destLiquid;

	private float spriteTimer;

	private int spriteIndex;

	private Vector3 originalScale;

	protected static Vector3 tempV;

	protected static Vector3 tempV2;

	public enum Type
	{
		Default,
		Boat,
		Canvas,
		MultiSprite
	}
}
