using System;
using UnityEngine;

public class CharaActor : CardActor
{
	public void SetOwnerAsChara()
	{
		this.owner = (this.owner as Chara);
	}

	public override void OnSetOwner()
	{
		this.SetOwnerAsChara();
		this.sr.sprite = this.owner.GetSprite(0);
		this.mpb.SetTexture("_MainTex", this.sr.sprite.texture);
		this.IdleFrame();
	}

	public override void NextFrame()
	{
		this.RefreshSprite();
	}

	public override void RefreshSprite()
	{
		Sprite sprite = this.sr.sprite;
		Texture2D texture = sprite.texture;
		Rect textureRect = sprite.textureRect;
		Vector4 value = new Vector4(textureRect.x / (float)texture.width, textureRect.min.y / (float)texture.height, textureRect.max.x / (float)texture.width, textureRect.max.y / (float)texture.height);
		this.mpb.SetVector("_Rect", value);
		this.mpb.SetFloat("_PixelHeight", sprite.rect.height);
	}

	public new Chara owner;
}
