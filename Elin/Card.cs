using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class Card : BaseCard, IReservable, ICardParent, IRenderSource, IGlobalValue, IInspect
{
	public override string ToString()
	{
		string[] array = new string[5];
		array[0] = this.Name;
		array[1] = "/";
		int num = 2;
		Point point = this.pos;
		array[num] = ((point != null) ? point.ToString() : null);
		array[3] = "/";
		int num2 = 4;
		ICardParent cardParent = this.parent;
		array[num2] = ((cardParent != null) ? cardParent.ToString() : null);
		return string.Concat(array);
	}

	public bool CanReserve(AIAct act)
	{
		return this.reservedAct == null || this.reservedAct == act || !this.reservedAct.IsRunning;
	}

	public bool TryReserve(AIAct act)
	{
		if (this.CanReserve(act))
		{
			this.reservedAct = act;
			return true;
		}
		return false;
	}

	public Card parentCard
	{
		get
		{
			return this.parent as Card;
		}
	}

	public Thing parentThing
	{
		get
		{
			return this.parent as Thing;
		}
	}

	public int colorInt
	{
		get
		{
			if (this._colorInt == 0)
			{
				this.RefreshColor();
			}
			return this._colorInt;
		}
	}

	public bool IsHotItem
	{
		get
		{
			return this.invY == 1;
		}
	}

	public int uid
	{
		get
		{
			return this._ints[1];
		}
		set
		{
			this._ints[1] = value;
		}
	}

	public int idMaterial
	{
		get
		{
			return this._ints[4];
		}
		set
		{
			this._ints[4] = value;
		}
	}

	public int dir
	{
		get
		{
			return this._ints[5];
		}
		set
		{
			this._ints[5] = value;
		}
	}

	public int Num
	{
		get
		{
			return this._ints[6];
		}
		set
		{
			this._ints[6] = value;
		}
	}

	public int _x
	{
		get
		{
			return this._ints[7];
		}
		set
		{
			this._ints[7] = value;
		}
	}

	public int _z
	{
		get
		{
			return this._ints[9];
		}
		set
		{
			this._ints[9] = value;
		}
	}

	public int refVal
	{
		get
		{
			return this._ints[11];
		}
		set
		{
			this._ints[11] = value;
		}
	}

	public int decay
	{
		get
		{
			return this._ints[12];
		}
		set
		{
			this._ints[12] = value;
		}
	}

	public int altitude
	{
		get
		{
			return this._ints[13];
		}
		set
		{
			this._ints[13] = value;
		}
	}

	public int hp
	{
		get
		{
			return this._ints[14];
		}
		set
		{
			this._ints[14] = value;
		}
	}

	public float fx
	{
		get
		{
			return 0.001f * (float)this._ints[15];
		}
		set
		{
			this._ints[15] = (int)(value * 1000f);
		}
	}

	public float fy
	{
		get
		{
			return 0.001f * (float)this._ints[16];
		}
		set
		{
			this._ints[16] = (int)(value * 1000f);
		}
	}

	public BlessedState blessedState
	{
		get
		{
			return this._ints[17].ToEnum<BlessedState>();
		}
		set
		{
			this._ints[17] = (int)value;
		}
	}

	public PlaceState _placeState
	{
		get
		{
			return this._ints[18].ToEnum<PlaceState>();
		}
		set
		{
			this._ints[18] = (int)value;
		}
	}

	public int rarityLv
	{
		get
		{
			return this._ints[19];
		}
		set
		{
			this._ints[19] = value;
		}
	}

	public Rarity rarity
	{
		get
		{
			return (this._ints[19] / 100).ToEnum<Rarity>();
		}
		set
		{
			this._ints[19] = (int)(value * (Rarity)100);
		}
	}

	public int encLV
	{
		get
		{
			return this._ints[20];
		}
		set
		{
			this._ints[20] = value;
		}
	}

	public int posInvX
	{
		get
		{
			return this._ints[21];
		}
		set
		{
			this._ints[21] = value;
		}
	}

	public int posInvY
	{
		get
		{
			return this._ints[22];
		}
		set
		{
			this._ints[22] = value;
		}
	}

	public int idSkin
	{
		get
		{
			return this._ints[23];
		}
		set
		{
			this._ints[23] = value;
		}
	}

	public int feat
	{
		get
		{
			return this._ints[24];
		}
		set
		{
			this._ints[24] = value;
		}
	}

	public int LV
	{
		get
		{
			return this._ints[25];
		}
		set
		{
			this._ints[25] = value;
		}
	}

	public int exp
	{
		get
		{
			return this._ints[26];
		}
		set
		{
			this._ints[26] = value;
		}
	}

	public int qualityTier
	{
		get
		{
			return this._ints[27];
		}
		set
		{
			this._ints[27] = value;
		}
	}

	public bool isCensored
	{
		get
		{
			return this._bits1[1];
		}
		set
		{
			this._bits1[1] = value;
		}
	}

	public bool isDeconstructing
	{
		get
		{
			return this._bits1[2];
		}
		set
		{
			this._bits1[2] = value;
		}
	}

	public bool isDyed
	{
		get
		{
			return this._bits1[3];
		}
		set
		{
			this._bits1[3] = value;
		}
	}

	public bool isModified
	{
		get
		{
			return this._bits1[4];
		}
		set
		{
			this._bits1[4] = value;
		}
	}

	public bool isNew
	{
		get
		{
			return this._bits1[5];
		}
		set
		{
			this._bits1[5] = value;
		}
	}

	public bool isPlayerCreation
	{
		get
		{
			return this._bits1[6];
		}
		set
		{
			this._bits1[6] = value;
		}
	}

	public bool ignoreAutoPick
	{
		get
		{
			return this._bits1[7];
		}
		set
		{
			this._bits1[7] = value;
		}
	}

	public bool freePos
	{
		get
		{
			return this._bits1[8];
		}
		set
		{
			this._bits1[8] = value;
		}
	}

	public bool isHidden
	{
		get
		{
			return this._bits1[9];
		}
		set
		{
			this._bits1[9] = value;
		}
	}

	public bool isOn
	{
		get
		{
			return this._bits1[10];
		}
		set
		{
			this._bits1[10] = value;
		}
	}

	public bool isNPCProperty
	{
		get
		{
			return this._bits1[11];
		}
		set
		{
			this._bits1[11] = value;
		}
	}

	public bool isRestrained
	{
		get
		{
			return this._bits1[12];
		}
		set
		{
			this._bits1[12] = value;
		}
	}

	public bool isRoofItem
	{
		get
		{
			return this._bits1[13];
		}
		set
		{
			this._bits1[13] = value;
		}
	}

	public bool isMasked
	{
		get
		{
			return this._bits1[14];
		}
		set
		{
			this._bits1[14] = value;
		}
	}

	public bool disableAutoToggle
	{
		get
		{
			return this._bits1[15];
		}
		set
		{
			this._bits1[15] = value;
		}
	}

	public bool isImported
	{
		get
		{
			return this._bits1[16];
		}
		set
		{
			this._bits1[16] = value;
		}
	}

	public bool autoRefuel
	{
		get
		{
			return this._bits1[17];
		}
		set
		{
			this._bits1[17] = value;
		}
	}

	public bool ignoreStackHeight
	{
		get
		{
			return this._bits1[18];
		}
		set
		{
			this._bits1[18] = value;
		}
	}

	public bool isFloating
	{
		get
		{
			return this._bits1[19];
		}
		set
		{
			this._bits1[19] = value;
		}
	}

	public bool isWeightChanged
	{
		get
		{
			return this._bits1[20];
		}
		set
		{
			this._bits1[20] = value;
		}
	}

	public bool isFireproof
	{
		get
		{
			return this._bits1[21];
		}
		set
		{
			this._bits1[21] = value;
		}
	}

	public bool isAcidproof
	{
		get
		{
			return this._bits1[22];
		}
		set
		{
			this._bits1[22] = value;
		}
	}

	public bool isReplica
	{
		get
		{
			return this._bits1[23];
		}
		set
		{
			this._bits1[23] = value;
		}
	}

	public bool isSummon
	{
		get
		{
			return this._bits1[24];
		}
		set
		{
			this._bits1[24] = value;
		}
	}

	public bool isElemental
	{
		get
		{
			return this._bits1[25];
		}
		set
		{
			this._bits1[25] = value;
		}
	}

	public bool isBroken
	{
		get
		{
			return this._bits1[26];
		}
		set
		{
			this._bits1[26] = value;
		}
	}

	public bool isSubsetCard
	{
		get
		{
			return this._bits1[27];
		}
		set
		{
			this._bits1[27] = value;
		}
	}

	public bool noSnow
	{
		get
		{
			return this._bits1[28];
		}
		set
		{
			this._bits1[28] = value;
		}
	}

	public bool noMove
	{
		get
		{
			return this._bits1[29];
		}
		set
		{
			this._bits1[29] = value;
		}
	}

	public bool isGifted
	{
		get
		{
			return this._bits1[30];
		}
		set
		{
			this._bits1[30] = value;
		}
	}

	public bool isCrafted
	{
		get
		{
			return this._bits1[31];
		}
		set
		{
			this._bits1[31] = value;
		}
	}

	public bool isLostProperty
	{
		get
		{
			return this._bits2[0];
		}
		set
		{
			this._bits2[0] = value;
		}
	}

	public bool noShadow
	{
		get
		{
			return this._bits2[1];
		}
		set
		{
			this._bits2[1] = value;
		}
	}

	public bool noSell
	{
		get
		{
			return this._bits2[2];
		}
		set
		{
			this._bits2[2] = value;
		}
	}

	public bool isLeashed
	{
		get
		{
			return this._bits2[3];
		}
		set
		{
			this._bits2[3] = value;
		}
	}

	public bool isStolen
	{
		get
		{
			return this._bits2[4];
		}
		set
		{
			this._bits2[4] = value;
		}
	}

	public bool isSale
	{
		get
		{
			return this._bits2[5];
		}
		set
		{
			this._bits2[5] = value;
		}
	}

	public bool isCopy
	{
		get
		{
			return this._bits2[6];
		}
		set
		{
			this._bits2[6] = value;
		}
	}

	public bool isRestocking
	{
		get
		{
			return this._bits2[7];
		}
		set
		{
			this._bits2[7] = value;
		}
	}

	public bool isBackerContent
	{
		get
		{
			return this.c_idBacker != 0;
		}
	}

	public SourceBacker.Row sourceBacker
	{
		get
		{
			if (!this.isBackerContent)
			{
				return null;
			}
			return EClass.sources.backers.map.TryGetValue(this.c_idBacker, null);
		}
	}

	public void Mod()
	{
		this.isModified = true;
	}

	public BedType c_bedType
	{
		get
		{
			return base.GetInt(9, null).ToEnum<BedType>();
		}
		set
		{
			base.SetInt(9, (int)value);
		}
	}

	public int c_equippedSlot
	{
		get
		{
			return base.GetInt(6, null);
		}
		set
		{
			base.SetInt(6, value);
		}
	}

	public int c_lockLv
	{
		get
		{
			return base.GetInt(50, null);
		}
		set
		{
			base.SetInt(50, value);
		}
	}

	public Hostility c_originalHostility
	{
		get
		{
			return base.GetInt(12, null).ToEnum<Hostility>();
		}
		set
		{
			base.SetInt(12, (int)value);
		}
	}

	public MinionType c_minionType
	{
		get
		{
			return base.GetInt(61, null).ToEnum<MinionType>();
		}
		set
		{
			base.SetInt(61, (int)value);
		}
	}

	public int c_vomit
	{
		get
		{
			return base.GetInt(13, null);
		}
		set
		{
			base.SetInt(13, value);
		}
	}

	public bool c_isImportant
	{
		get
		{
			return base.GetInt(109, null) != 0;
		}
		set
		{
			base.SetInt(109, value ? 1 : 0);
		}
	}

	public bool c_lockedHard
	{
		get
		{
			return base.GetInt(63, null) != 0;
		}
		set
		{
			base.SetInt(63, value ? 1 : 0);
		}
	}

	public bool c_revealLock
	{
		get
		{
			return base.GetInt(64, null) != 0;
		}
		set
		{
			base.SetInt(64, value ? 1 : 0);
		}
	}

	public bool c_isTrained
	{
		get
		{
			return base.GetInt(120, null) != 0;
		}
		set
		{
			base.SetInt(120, value ? 1 : 0);
		}
	}

	public bool c_isPrayed
	{
		get
		{
			return base.GetInt(121, null) != 0;
		}
		set
		{
			base.SetInt(121, value ? 1 : 0);
		}
	}

	public bool c_isDisableStockUse
	{
		get
		{
			return base.GetInt(122, null) != 0;
		}
		set
		{
			base.SetInt(122, value ? 1 : 0);
		}
	}

	public int c_lightColor
	{
		get
		{
			return base.GetInt(5, null);
		}
		set
		{
			this.Mod();
			base.SetInt(5, value);
		}
	}

	public Color LightColor
	{
		get
		{
			return new Color((float)(this.c_lightColor / 1024 * 8) / 256f, (float)(this.c_lightColor % 1024 / 32 * 8) / 256f, (float)(this.c_lightColor % 32 * 8) / 256f, 1f);
		}
	}

	public int c_uidZone
	{
		get
		{
			return base.GetInt(10, null);
		}
		set
		{
			this.Mod();
			base.SetInt(10, value);
		}
	}

	public int c_uidRefCard
	{
		get
		{
			return base.GetInt(11, null);
		}
		set
		{
			this.Mod();
			base.SetInt(11, value);
		}
	}

	public int c_priceFix
	{
		get
		{
			return base.GetInt(21, null);
		}
		set
		{
			base.SetInt(21, value);
		}
	}

	public int c_priceAdd
	{
		get
		{
			return base.GetInt(29, null);
		}
		set
		{
			base.SetInt(29, value);
		}
	}

	public int c_dyeMat
	{
		get
		{
			return base.GetInt(3, null);
		}
		set
		{
			base.SetInt(3, value);
		}
	}

	public VisitorState visitorState
	{
		get
		{
			return base.GetInt(4, null).ToEnum<VisitorState>();
		}
		set
		{
			base.SetInt(4, (int)value);
		}
	}

	public RescueState c_rescueState
	{
		get
		{
			return base.GetInt(53, null).ToEnum<RescueState>();
		}
		set
		{
			base.SetInt(53, (int)value);
		}
	}

	public BossType c_bossType
	{
		get
		{
			return base.GetInt(65, null).ToEnum<BossType>();
		}
		set
		{
			base.SetInt(65, (int)value);
		}
	}

	public int c_dateStockExpire
	{
		get
		{
			return base.GetInt(22, null);
		}
		set
		{
			base.SetInt(22, value);
		}
	}

	public int c_IDTState
	{
		get
		{
			return base.GetInt(2, null);
		}
		set
		{
			base.SetInt(2, value);
		}
	}

	public int c_charges
	{
		get
		{
			return base.GetInt(7, null);
		}
		set
		{
			base.SetInt(7, value);
		}
	}

	public int c_bill
	{
		get
		{
			return base.GetInt(23, null);
		}
		set
		{
			base.SetInt(23, value);
		}
	}

	public int c_invest
	{
		get
		{
			return base.GetInt(28, null);
		}
		set
		{
			base.SetInt(28, value);
		}
	}

	public int c_seed
	{
		get
		{
			return base.GetInt(33, null);
		}
		set
		{
			base.SetInt(33, value);
		}
	}

	public int c_allowance
	{
		get
		{
			return base.GetInt(114, null);
		}
		set
		{
			base.SetInt(114, value);
		}
	}

	public int c_fur
	{
		get
		{
			return base.GetInt(62, null);
		}
		set
		{
			base.SetInt(62, value);
		}
	}

	public int c_containerSize
	{
		get
		{
			return base.GetInt(8, null);
		}
		set
		{
			base.SetInt(8, value);
		}
	}

	public int c_weight
	{
		get
		{
			return base.GetInt(1, null);
		}
		set
		{
			base.SetInt(1, value);
		}
	}

	public int c_diceDim
	{
		get
		{
			return base.GetInt(14, null);
		}
		set
		{
			base.SetInt(14, value);
		}
	}

	public int c_indexContainerIcon
	{
		get
		{
			return base.GetInt(15, null);
		}
		set
		{
			base.SetInt(15, value);
		}
	}

	public int c_idMainElement
	{
		get
		{
			return base.GetInt(16, null);
		}
		set
		{
			base.SetInt(16, value);
		}
	}

	public int c_summonDuration
	{
		get
		{
			return base.GetInt(17, null);
		}
		set
		{
			base.SetInt(17, value);
		}
	}

	public int c_idBacker
	{
		get
		{
			return base.GetInt(52, null);
		}
		set
		{
			base.SetInt(52, value);
		}
	}

	public int c_uidMaster
	{
		get
		{
			return base.GetInt(54, null);
		}
		set
		{
			base.SetInt(54, value);
		}
	}

	public int c_ammo
	{
		get
		{
			return base.GetInt(27, null);
		}
		set
		{
			base.SetInt(27, value);
		}
	}

	public int c_daysWithGod
	{
		get
		{
			return base.GetInt(57, null);
		}
		set
		{
			base.SetInt(57, value);
		}
	}

	public string c_idPortrait
	{
		get
		{
			return base.GetStr(9, null);
		}
		set
		{
			base.SetStr(9, value);
		}
	}

	public string c_idRace
	{
		get
		{
			return base.GetStr(3, null);
		}
		set
		{
			base.SetStr(3, value);
		}
	}

	public string c_idJob
	{
		get
		{
			return base.GetStr(4, null);
		}
		set
		{
			base.SetStr(4, value);
		}
	}

	public string c_idTone
	{
		get
		{
			return base.GetStr(22, null);
		}
		set
		{
			base.SetStr(22, value);
		}
	}

	public string c_color
	{
		get
		{
			return base.GetStr(8, null);
		}
		set
		{
			base.SetStr(8, value);
		}
	}

	public string c_idTalk
	{
		get
		{
			return base.GetStr(21, null);
		}
		set
		{
			base.SetStr(21, value);
		}
	}

	public string c_idDeity
	{
		get
		{
			return base.GetStr(26, null);
		}
		set
		{
			base.SetStr(26, value);
		}
	}

	public string c_altName
	{
		get
		{
			return base.GetStr(1, null);
		}
		set
		{
			base.SetStr(1, value);
		}
	}

	public string c_altName2
	{
		get
		{
			return base.GetStr(2, null);
		}
		set
		{
			base.SetStr(2, value);
		}
	}

	public string c_refText
	{
		get
		{
			return base.GetStr(10, null);
		}
		set
		{
			base.SetStr(10, value);
		}
	}

	public string c_idRefName
	{
		get
		{
			return base.GetStr(7, null);
		}
		set
		{
			base.SetStr(7, value);
		}
	}

	public string c_idRidePCC
	{
		get
		{
			return base.GetStr(55, null);
		}
		set
		{
			base.SetStr(55, value);
		}
	}

	public string c_idAbility
	{
		get
		{
			return base.GetStr(50, null);
		}
		set
		{
			base.SetStr(50, value);
		}
	}

	public string c_context
	{
		get
		{
			return base.GetStr(20, null);
		}
		set
		{
			base.SetStr(20, value);
		}
	}

	public string c_idEditor
	{
		get
		{
			return base.GetStr(27, null);
		}
		set
		{
			base.SetStr(27, value);
		}
	}

	public string c_editorTags
	{
		get
		{
			return base.GetStr(28, null);
		}
		set
		{
			base.SetStr(28, value);
		}
	}

	public string c_editorTraitVal
	{
		get
		{
			return base.GetStr(25, null);
		}
		set
		{
			base.SetStr(25, value);
		}
	}

	public string c_idTrait
	{
		get
		{
			return base.GetStr(29, null);
		}
		set
		{
			base.SetStr(29, value);
		}
	}

	public string c_idRefCard
	{
		get
		{
			return base.GetStr(5, null);
		}
		set
		{
			base.SetStr(5, value);
		}
	}

	public string c_idRefCard2
	{
		get
		{
			return base.GetStr(6, null);
		}
		set
		{
			base.SetStr(6, value);
		}
	}

	public string c_note
	{
		get
		{
			return base.GetStr(51, null);
		}
		set
		{
			base.SetStr(51, value);
		}
	}

	public UniqueData c_uniqueData
	{
		get
		{
			return base.GetObj<UniqueData>(6);
		}
		set
		{
			base.SetObj(6, value);
		}
	}

	public Thing ammoData
	{
		get
		{
			return base.GetObj<Thing>(9);
		}
		set
		{
			base.SetObj(9, value);
		}
	}

	public Thing c_copyContainer
	{
		get
		{
			return base.GetObj<Thing>(10);
		}
		set
		{
			base.SetObj(10, value);
		}
	}

	public Window.SaveData c_windowSaveData
	{
		get
		{
			return base.GetObj<Window.SaveData>(2);
		}
		set
		{
			base.SetObj(2, value);
		}
	}

	public CharaUpgrade c_upgrades
	{
		get
		{
			return base.GetObj<CharaUpgrade>(11);
		}
		set
		{
			base.SetObj(11, value);
		}
	}

	public CharaGenes c_genes
	{
		get
		{
			return base.GetObj<CharaGenes>(15);
		}
		set
		{
			base.SetObj(15, value);
		}
	}

	public List<int> c_corruptionHistory
	{
		get
		{
			return base.GetObj<List<int>>(16);
		}
		set
		{
			base.SetObj(16, value);
		}
	}

	public ContainerUpgrade c_containerUpgrade
	{
		get
		{
			ContainerUpgrade result;
			if ((result = base.GetObj<ContainerUpgrade>(12)) == null)
			{
				result = (this.c_containerUpgrade = new ContainerUpgrade());
			}
			return result;
		}
		set
		{
			base.SetObj(12, value);
		}
	}

	public DNA c_DNA
	{
		get
		{
			return base.GetObj<DNA>(14);
		}
		set
		{
			base.SetObj(14, value);
		}
	}

	public CharaList c_charaList
	{
		get
		{
			return base.GetObj<CharaList>(13);
		}
		set
		{
			base.SetObj(13, value);
		}
	}

	public Window.SaveData GetWindowSaveData()
	{
		if (this.IsPC)
		{
			return Window.dictData.TryGetValue("LayerInventoryFloatMain0", null);
		}
		if (this.trait is TraitChestMerchant)
		{
			return Window.dictData.TryGetValue("ChestMerchant", null);
		}
		return this.c_windowSaveData;
	}

	public bool IsExcludeFromCraft()
	{
		if (this.IsUnique || this.c_isImportant)
		{
			return true;
		}
		Card card = this.parent as Card;
		if (card != null)
		{
			if (card.trait is TraitChestMerchant)
			{
				return true;
			}
			if (card.isSale)
			{
				return true;
			}
			Window.SaveData windowSaveData = card.GetWindowSaveData();
			if (windowSaveData != null)
			{
				return windowSaveData.excludeCraft;
			}
		}
		return false;
	}

	public byte[] c_textureData
	{
		get
		{
			return base.GetObj<byte[]>(4);
		}
		set
		{
			base.SetObj(4, value);
		}
	}

	public SourceMaterial.Row DyeMat
	{
		get
		{
			return EClass.sources.materials.rows[this.c_dyeMat];
		}
	}

	public int invX
	{
		get
		{
			return this.pos.x;
		}
		set
		{
			this.pos.x = value;
		}
	}

	public int invY
	{
		get
		{
			return this.pos.z;
		}
		set
		{
			this.pos.z = value;
		}
	}

	public CardRow refCard
	{
		get
		{
			CardRow result;
			if (!this.c_idRefCard.IsEmpty())
			{
				if ((result = EClass.sources.cards.map.TryGetValue(this.c_idRefCard, null)) == null)
				{
					return EClass.sources.cards.map["ash3"];
				}
			}
			else
			{
				result = null;
			}
			return result;
		}
	}

	public CardRow refCard2
	{
		get
		{
			CardRow result;
			if (!this.c_idRefCard2.IsEmpty())
			{
				if ((result = EClass.sources.cards.map.TryGetValue(this.c_idRefCard2, null)) == null)
				{
					return EClass.sources.cards.map["ash3"];
				}
			}
			else
			{
				result = null;
			}
			return result;
		}
	}

	public int ExpToNext
	{
		get
		{
			return (50 + this.LV * 30) * (100 - this.Evalue(403)) / 100;
		}
	}

	public int DefaultLV
	{
		get
		{
			return this.sourceCard.LV;
		}
	}

	public int ChildrenWeight
	{
		get
		{
			if (this.dirtyWeight)
			{
				this._childrenWeight = 0;
				if (!(this.trait is TraitMagicChest))
				{
					foreach (Thing thing in this.things)
					{
						this._childrenWeight += thing.ChildrenAndSelfWeight;
					}
					this.dirtyWeight = false;
					Card card = this.parent as Card;
					if (card != null)
					{
						card.SetDirtyWeight();
					}
					if (this.isChara && this.IsPCFaction)
					{
						this.Chara.CalcBurden();
					}
					if (this._childrenWeight < 0 || this._childrenWeight >= 10000000)
					{
						this._childrenWeight = 9999999;
					}
				}
			}
			return this._childrenWeight * Mathf.Max(100 - this.Evalue(404), 0) / 100;
		}
	}

	public void SetDirtyWeight()
	{
		if (EClass.core.IsGameStarted && this.IsPC)
		{
			EClass.player.wasDirtyWeight = true;
		}
		this.dirtyWeight = true;
		Card card = this.parent as Card;
		if (card == null)
		{
			return;
		}
		card.SetDirtyWeight();
	}

	public void ChangeWeight(int a)
	{
		this.c_weight = a;
		this.isWeightChanged = true;
		this.SetDirtyWeight();
	}

	public int ChildrenAndSelfWeight
	{
		get
		{
			return this.ChildrenWeight + this.SelfWeight * this.Num;
		}
	}

	public int ChildrenAndSelfWeightSingle
	{
		get
		{
			return this.ChildrenWeight + this.SelfWeight;
		}
	}

	public virtual int SelfWeight
	{
		get
		{
			return 1000;
		}
	}

	public virtual int WeightLimit
	{
		get
		{
			return 500000;
		}
	}

	public SourceCategory.Row category
	{
		get
		{
			SourceCategory.Row result;
			if ((result = this._category) == null)
			{
				result = (this._category = EClass.sources.categories.map[this.sourceCard.category]);
			}
			return result;
		}
	}

	public SourceMaterial.Row material
	{
		get
		{
			SourceMaterial.Row result;
			if ((result = this._material) == null)
			{
				result = (this._material = EClass.sources.materials.map[this.idMaterial]);
			}
			return result;
		}
	}

	public virtual string AliasMaterialOnCreate
	{
		get
		{
			return this.DefaultMaterial.alias;
		}
	}

	public int Evalue(int ele)
	{
		return this.elements.Value(ele);
	}

	public int Evalue(string alias)
	{
		return this.elements.Value(EClass.sources.elements.alias[alias].id);
	}

	public bool HasTag(CTAG tag)
	{
		return this.sourceCard.tag.Contains(tag.ToString());
	}

	public bool HasEditorTag(EditorTag tag)
	{
		string c_editorTags = this.c_editorTags;
		return c_editorTags != null && c_editorTags.Contains(tag.ToString());
	}

	public void AddEditorTag(EditorTag tag)
	{
		this.c_editorTags = (this.c_editorTags.IsEmpty() ? tag.ToString() : (this.c_editorTags + "," + tag.ToString()));
	}

	public void RemoveEditorTag(EditorTag tag)
	{
		this.c_editorTags = (this.c_editorTags.IsEmpty() ? null : this.c_editorTags.Replace(tag.ToString(), "").Replace(",,", ","));
	}

	public Cell Cell
	{
		get
		{
			return this.pos.cell;
		}
	}

	public virtual Thing Thing
	{
		get
		{
			if (!this.isThing)
			{
				return null;
			}
			return (Thing)this;
		}
	}

	public virtual Chara Chara
	{
		get
		{
			if (!this.isChara)
			{
				return null;
			}
			return (Chara)this;
		}
	}

	public virtual bool isThing
	{
		get
		{
			return false;
		}
	}

	public virtual bool isChara
	{
		get
		{
			return false;
		}
	}

	public bool ExistsOnMap
	{
		get
		{
			return this.parent == EClass._zone;
		}
	}

	public virtual bool isSynced
	{
		get
		{
			return this.renderer.isSynced;
		}
	}

	public bool IsContainer
	{
		get
		{
			return this.trait.IsContainer;
		}
	}

	public bool IsUnique
	{
		get
		{
			return this.rarity == Rarity.Artifact;
		}
	}

	public bool IsPowerful
	{
		get
		{
			return (this.rarity >= Rarity.Legendary || this.trait is TraitAdventurer) && !this.IsPCFaction;
		}
	}

	public bool IsImportant
	{
		get
		{
			return this.sourceCard.HasTag(CTAG.important);
		}
	}

	public virtual SourcePref Pref
	{
		get
		{
			return this.sourceCard.pref;
		}
	}

	public virtual bool IsDeadOrSleeping
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsDisabled
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsMoving
	{
		get
		{
			return false;
		}
	}

	public virtual bool flipX
	{
		get
		{
			return this.Tiles.Length == 1 && this.dir % 2 == 1;
		}
	}

	public virtual bool IsAliveInCurrentZone
	{
		get
		{
			return this.ExistsOnMap;
		}
	}

	public virtual string actorPrefab
	{
		get
		{
			return "ThingActor";
		}
	}

	public virtual CardRow sourceCard
	{
		get
		{
			return null;
		}
	}

	public virtual CardRow sourceRenderCard
	{
		get
		{
			return this.sourceCard;
		}
	}

	public TileType TileType
	{
		get
		{
			return this.sourceCard.tileType;
		}
	}

	public string Name
	{
		get
		{
			return this.GetName(NameStyle.Full, -1);
		}
	}

	public string NameSimple
	{
		get
		{
			return this.GetName(NameStyle.Simple, -1);
		}
	}

	public string NameOne
	{
		get
		{
			return this.GetName(NameStyle.Full, 1);
		}
	}

	public virtual string GetName(NameStyle style, int num = -1)
	{
		return "Card";
	}

	public virtual string GetExtraName()
	{
		return "";
	}

	public virtual string GetDetail()
	{
		return this.sourceCard.GetText("detail", true);
	}

	public virtual bool IsPC
	{
		get
		{
			return false;
		}
	}

	public bool _IsPC
	{
		get
		{
			return base.GetInt(56, null) != 0;
		}
	}

	public virtual bool IsPCC
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsPCParty
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsMinion
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsPCPartyMinion
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsPCFactionMinion
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsMultisize
	{
		get
		{
			return this.sourceCard.multisize && this.IsInstalled;
		}
	}

	public bool IsToolbelt
	{
		get
		{
			return this.category.slot == 44;
		}
	}

	public bool IsLightsource
	{
		get
		{
			return this.category.slot == 45;
		}
	}

	public bool IsEquipment
	{
		get
		{
			return this.category.slot != 0;
		}
	}

	public bool IsFood
	{
		get
		{
			return this.category.IsChildOf("food");
		}
	}

	public bool ShowFoodEnc
	{
		get
		{
			return this.IsFood || this.category.IsChildOf("seed") || this.id == "pasture" || this.category.IsChildOf("drug");
		}
	}

	public bool IsWeapon
	{
		get
		{
			return this.IsMeleeWeapon || this.IsRangedWeapon;
		}
	}

	public bool IsEquipmentOrRanged
	{
		get
		{
			return this.category.slot != 0 || this.IsRangedWeapon;
		}
	}

	public bool IsMeleeWeapon
	{
		get
		{
			return this.category.IsChildOf("melee");
		}
	}

	public bool IsRangedWeapon
	{
		get
		{
			return this.trait is TraitToolRange;
		}
	}

	public bool IsThrownWeapon
	{
		get
		{
			return this.sourceCard.HasTag(CTAG.throwWeapon);
		}
	}

	public bool IsAmmo
	{
		get
		{
			return this.trait is TraitAmmo;
		}
	}

	public bool IsAgent
	{
		get
		{
			return this == EClass.player.Agent;
		}
	}

	public bool IsFurniture
	{
		get
		{
			return this.category.IsChildOf("furniture");
		}
	}

	public bool IsBlessed
	{
		get
		{
			return this.blessedState >= BlessedState.Blessed;
		}
	}

	public bool IsCursed
	{
		get
		{
			return this.blessedState <= BlessedState.Cursed;
		}
	}

	public bool IsRestrainedResident
	{
		get
		{
			return this.isRestrained && this.IsPCFaction;
		}
	}

	public virtual bool IsPCFaction
	{
		get
		{
			return false;
		}
	}

	public bool IsPCFactionOrMinion
	{
		get
		{
			return this.IsPCFaction || this.IsPCFactionMinion;
		}
	}

	public virtual bool IsGlobal
	{
		get
		{
			return false;
		}
	}

	public virtual int MaxDecay
	{
		get
		{
			return 1000;
		}
	}

	public bool IsDecayed
	{
		get
		{
			return this.decay > this.MaxDecay;
		}
	}

	public bool IsRotting
	{
		get
		{
			return this.decay >= this.MaxDecay / 4 * 3;
		}
	}

	public bool IsFresn
	{
		get
		{
			return this.decay < this.MaxDecay / 4;
		}
	}

	public virtual int MaxHP
	{
		get
		{
			return 100;
		}
	}

	public virtual int Power
	{
		get
		{
			return Mathf.Max(20, EClass.curve(this.GetTotalQuality(true) * 10, 400, 100, 75));
		}
	}

	public int FameLv
	{
		get
		{
			if (!this.IsPC)
			{
				return this.LV;
			}
			return EClass.player.fame / 100 + 1;
		}
	}

	public virtual int[] Tiles
	{
		get
		{
			return this.sourceCard._tiles;
		}
	}

	public virtual int PrefIndex
	{
		get
		{
			if (this.Tiles.Length < 3)
			{
				return this.dir % 2;
			}
			return this.dir;
		}
	}

	public bool IsVariation
	{
		get
		{
			return this.sourceCard.origin != null;
		}
	}

	public virtual int DV
	{
		get
		{
			return this.elements.Value(64);
		}
	}

	public virtual int PV
	{
		get
		{
			return this.elements.Value(65);
		}
	}

	public int HIT
	{
		get
		{
			return this.elements.Value(66);
		}
	}

	public int DMG
	{
		get
		{
			return this.elements.Value(67);
		}
	}

	public int STR
	{
		get
		{
			return this.elements.Value(70);
		}
	}

	public int DEX
	{
		get
		{
			return this.elements.Value(72);
		}
	}

	public int END
	{
		get
		{
			return this.elements.Value(71);
		}
	}

	public int PER
	{
		get
		{
			return this.elements.Value(73);
		}
	}

	public int LER
	{
		get
		{
			return this.elements.Value(74);
		}
	}

	public int WIL
	{
		get
		{
			return this.elements.Value(75);
		}
	}

	public int MAG
	{
		get
		{
			return this.elements.Value(76);
		}
	}

	public int CHA
	{
		get
		{
			return this.elements.Value(77);
		}
	}

	public int INT
	{
		get
		{
			return this.elements.Value(80);
		}
	}

	public int LUC
	{
		get
		{
			return this.elements.Value(78);
		}
	}

	public int GetBestAttribute()
	{
		int num = 1;
		foreach (Element element in from a in this.elements.dict.Values
		where Element.List_MainAttributesMajor.Contains(a.id)
		select a)
		{
			int num2 = element.Value;
			if (this.isChara && this.Chara.tempElements != null)
			{
				num2 -= this.Chara.tempElements.Value(element.id);
			}
			if (num2 > num)
			{
				num = num2;
			}
		}
		return num;
	}

	public void ModExp(string alias, int a)
	{
		this.ModExp(EClass.sources.elements.alias[alias].id, a);
	}

	public void ModExp(int ele, int a)
	{
		if (!this.isChara)
		{
			return;
		}
		this.elements.ModExp(ele, a, false);
	}

	public int W
	{
		get
		{
			if (this.dir % 2 != 0)
			{
				return this.sourceCard.H;
			}
			return this.sourceCard.W;
		}
	}

	public int H
	{
		get
		{
			if (this.dir % 2 != 0)
			{
				return this.sourceCard.W;
			}
			return this.sourceCard.H;
		}
	}

	public bool IsIdentified
	{
		get
		{
			return this.c_IDTState == 0;
		}
	}

	public string TextRarity
	{
		get
		{
			return Lang.GetList("quality")[Mathf.Clamp((int)(this.rarity + 1), 0, 5)];
		}
	}

	public bool IsInstalled
	{
		get
		{
			return this.placeState == PlaceState.installed;
		}
	}

	public bool IsMale
	{
		get
		{
			return this.bio != null && this.bio.gender == 2;
		}
	}

	public bool IsNegativeGift
	{
		get
		{
			return this.HasTag(CTAG.neg) || this.blessedState <= BlessedState.Cursed;
		}
	}

	public bool IsChildOf(Card c)
	{
		return this.GetRootCard() == c;
	}

	public bool HasContainerSize
	{
		get
		{
			return this.c_containerSize != 0;
		}
	}

	public Thing Tool
	{
		get
		{
			if (!this.IsPC)
			{
				return null;
			}
			return EClass.player.currentHotItem.Thing;
		}
	}

	public virtual SourceMaterial.Row DefaultMaterial
	{
		get
		{
			return this.sourceCard.DefaultMaterial;
		}
	}

	public virtual bool HasHost
	{
		get
		{
			return false;
		}
	}

	[OnSerializing]
	private void _OnSerializing(StreamingContext context)
	{
		this._x = this.pos.x;
		this._z = this.pos.z;
		this._ints[0] = this._bits1.ToInt();
		this._ints[2] = this._bits2.ToInt();
		this._placeState = this.placeState;
		this.OnSerializing();
	}

	protected virtual void OnSerializing()
	{
	}

	[OnDeserialized]
	private void _OnDeserialized(StreamingContext context)
	{
		this._bits1.SetInt(this._ints[0]);
		this._bits2.SetInt(this._ints[2]);
		this.placeState = this._placeState;
		this.pos.Set(this._x, this._z);
		this.SetSource();
		this.things.SetOwner(this);
		this.elements.SetOwner(this, false);
		this.OnDeserialized();
		this.ApplyTrait();
		this.ApplyMaterialElements(false);
		this._CreateRenderer();
		foreach (Thing thing in this.things)
		{
			thing.parent = this;
		}
		if (this.isThing && this.Num <= 0)
		{
			this.isDestroyed = true;
		}
	}

	protected virtual void OnDeserialized()
	{
	}

	public string ReferenceId()
	{
		return "c" + this.uid.ToString();
	}

	public void Create(string _id, int _idMat = -1, int genLv = -1)
	{
		if (CardBlueprint.current != null)
		{
			this.bp = CardBlueprint.current;
			CardBlueprint.current = null;
		}
		else
		{
			this.bp = CardBlueprint._Default;
		}
		this.id = _id;
		this.Num = 1;
		this.autoRefuel = true;
		EClass.game.cards.AssignUID(this);
		this.isNew = true;
		this.SetSource();
		this.OnBeforeCreate();
		if (this.sourceCard.quality != 0)
		{
			this.rarity = this.sourceCard.quality.ToEnum<Rarity>();
		}
		else if (this.bp.rarity != Rarity.Random)
		{
			this.rarity = this.bp.rarity;
		}
		else if (this.category.slot != 0 && this.category.slot != 45 && this.category.slot != 44)
		{
			if (EClass.rnd(10) == 0)
			{
				this.rarity = Rarity.Crude;
			}
			else if (EClass.rnd(10) == 0)
			{
				this.rarity = Rarity.Superior;
			}
			else if (EClass.rnd(80) == 0)
			{
				this.rarity = Rarity.Legendary;
			}
			else if (EClass.rnd(250) == 0)
			{
				this.rarity = Rarity.Mythical;
			}
		}
		if (this.rarity != Rarity.Normal && this.category.tag.Contains("fixedRarity"))
		{
			this.rarity = Rarity.Normal;
		}
		this.LV = this.DefaultLV;
		if (this.bp.lv != -999)
		{
			this.LV = this.bp.lv;
		}
		if (this.sourceCard.fixedMaterial)
		{
			this._material = EClass.sources.materials.alias[this.AliasMaterialOnCreate];
		}
		else
		{
			bool flag = (this.bp != null && this.bp.fixedMat) || this.sourceCard.quality == 4 || this.sourceCard.tierGroup.IsEmpty();
			if (_idMat != -1)
			{
				this._material = EClass.sources.materials.rows[_idMat];
			}
			else if (!flag)
			{
				if (this.sourceCard.tierGroup == "wood")
				{
					Debug.Log(this.id);
				}
				this._material = MATERIAL.GetRandomMaterial(genLv, this.sourceCard.tierGroup, this.bp.tryLevelMatTier);
			}
			else
			{
				this._material = EClass.sources.materials.alias[this.AliasMaterialOnCreate];
			}
		}
		this.idMaterial = this._material.id;
		this.things.SetOwner(this);
		this.elements.SetOwner(this, true);
		this.ApplyTrait();
		if (!this.bp.fixedQuality && this.trait.LevelAsQuality && (!EClass._zone.IsPCFaction || EClass.Branch.lv != 1) && EClass.debug.testThingQuality && EClass.rnd(2) == 0)
		{
			this.qualityTier = Mathf.Clamp(EClass.rnd(5) + 1, 1, 3);
			this.LV = this.LV + this.qualityTier * 10 + (this.LV - 1) * (125 + this.qualityTier * 25) / 100;
		}
		this.ApplyMaterial(false);
		this.OnCreate(genLv);
		this._CreateRenderer();
		this.trait.OnCreate(genLv);
		this.hp = this.MaxHP;
		if (this.HasTag(CTAG.hidden))
		{
			this.SetHidden(true);
		}
		this.isFloating = this.Pref.Float;
	}

	public virtual void OnBeforeCreate()
	{
	}

	public virtual void OnCreate(int genLv)
	{
	}

	public virtual void SetSource()
	{
	}

	public virtual void ApplyEditorTags(EditorTag tag)
	{
		if (tag <= EditorTag.IsOn)
		{
			if (tag == EditorTag.Empty)
			{
				this.RemoveThings();
				return;
			}
			if (tag == EditorTag.Hidden)
			{
				this.SetHidden(true);
				return;
			}
			if (tag != EditorTag.IsOn)
			{
				return;
			}
			this.isOn = true;
			return;
		}
		else
		{
			if (tag == EditorTag.IsOff)
			{
				this.isOn = false;
				return;
			}
			if (tag == EditorTag.NoSnow)
			{
				this.noSnow = true;
				return;
			}
			if (tag != EditorTag.Boss)
			{
				return;
			}
			EClass._zone.Boss = this.Chara;
			return;
		}
	}

	public void ApplyTrait()
	{
		string c_idTrait = this.c_idTrait;
		if (c_idTrait.IsEmpty())
		{
			if (this.sourceCard.trait.IsEmpty())
			{
				this.trait = (this.isChara ? new TraitChara() : new Trait());
			}
			else
			{
				this.trait = ClassCache.Create<Trait>("Trait" + this.sourceCard.trait[0], "Elin");
			}
		}
		else
		{
			this.trait = ClassCache.Create<Trait>(c_idTrait, "Elin");
		}
		this.trait.SetOwner(this);
	}

	public Card SetLv(int a)
	{
		this.LV = a;
		if (!this.isChara)
		{
			return this;
		}
		Rand.SetSeed(this.uid);
		ElementContainer elementContainer = new ElementContainer();
		elementContainer.ApplyElementMap(this.uid, SourceValueType.Chara, this.Chara.job.elementMap, this.LV, false, false);
		elementContainer.ApplyElementMap(this.uid, SourceValueType.Chara, this.Chara.race.elementMap, this.LV, false, false);
		elementContainer.ApplyElementMap(this.uid, SourceValueType.Chara, this.Chara.source.elementMap, 1, false, true);
		foreach (Element element in this.elements.dict.Values)
		{
			int num = elementContainer.Value(element.id);
			if (num != 0)
			{
				int num2 = num - element.ValueWithoutLink;
				if (num2 != 0)
				{
					this.elements.ModBase(element.id, num2);
				}
			}
		}
		Rand.SetSeed(-1);
		this.hp = this.MaxHP;
		this.Chara.mana.value = this.Chara.mana.max;
		this.Chara.CalculateMaxStamina();
		this.SetDirtyWeight();
		return this;
	}

	public void AddExp(int a)
	{
		if (!this.IsPC)
		{
			a *= 2;
			if (this.IsPCFaction)
			{
				a = a * Mathf.Clamp(100 + this.Chara.affinity.value / 10, 50, 100) / 100;
			}
		}
		this.exp += a;
		while (this.exp >= this.ExpToNext)
		{
			this.exp -= this.ExpToNext;
			this.LevelUp();
		}
	}

	public void LevelUp()
	{
		if (this.IsPC)
		{
			if (EClass.core.version.demo && EClass.player.totalFeat >= 5)
			{
				Msg.Say("demoLimit");
				return;
			}
			EClass.player.totalFeat++;
			Tutorial.Reserve("feat", null);
		}
		int num = this.feat;
		this.feat = num + 1;
		num = this.LV;
		this.LV = num + 1;
		this.Say("dingExp", this, null, null);
		this.PlaySound("jingle_lvup", 1f, true);
		this.PlayEffect("aura_heaven", true, 0f, default(Vector3));
		if (this.HasElement(1415, 1) && this.Evalue(1415) < 9 && this.LV >= this.Evalue(1415) * 5 + 10)
		{
			this.Chara.SetFeat(1415, this.Evalue(1415) + 1, true);
		}
		if (!this.IsPC)
		{
			if (this.Chara.race.id == "mutant")
			{
				int num2 = Mathf.Min(1 + this.LV / 5, 22);
				for (int i = 0; i < num2; i++)
				{
					if (this.Evalue(1644) < i + 1)
					{
						this.Chara.SetFeat(1644, i + 1, false);
					}
				}
			}
			this.Chara.TryUpgrade(true);
		}
	}

	public virtual void ApplyMaterialElements(bool remove)
	{
	}

	public virtual void ApplyMaterial(bool remove = false)
	{
		this._colorInt = 0;
	}

	public Card ChangeMaterial(int idNew)
	{
		return this.ChangeMaterial(EClass.sources.materials.map[idNew]);
	}

	public Card ChangeMaterial(string idNew)
	{
		return this.ChangeMaterial(EClass.sources.materials.alias[idNew]);
	}

	public Card ChangeMaterial(SourceMaterial.Row row)
	{
		if (this.sourceCard.fixedMaterial)
		{
			return this;
		}
		this.ApplyMaterial(true);
		this._material = row;
		this.idMaterial = row.id;
		this.decay = 0;
		this.dirtyWeight = true;
		Card rootCard = this.GetRootCard();
		if (rootCard != null && rootCard.IsPC)
		{
			this.GetRootCard().SetDirtyWeight();
		}
		if (this.isThing)
		{
			LayerInventory.SetDirty(this.Thing);
		}
		this.ApplyMaterial(false);
		return this;
	}

	public void SetReplica(bool on)
	{
		this.isReplica = true;
		this.ChangeMaterial("granite");
	}

	public Thing Add(string id, int num = 1, int lv = 1)
	{
		if (num == 0)
		{
			num = 1;
		}
		return this.AddCard(ThingGen.Create(id, -1, lv).SetNum(num)) as Thing;
	}

	public Card AddCard(Card c)
	{
		return this.AddThing(c as Thing, true, -1, -1);
	}

	public void RemoveCard(Card c)
	{
		this.RemoveThing(c as Thing);
	}

	public void NotifyAddThing(Thing t, int num)
	{
	}

	public Thing AddThing(string id, int lv = -1)
	{
		return this.AddThing(ThingGen.Create(id, -1, (lv == -1) ? this.LV : lv), true, -1, -1);
	}

	public Thing AddThing(Thing t, bool tryStack = true, int destInvX = -1, int destInvY = -1)
	{
		if (t.Num == 0 || t.isDestroyed)
		{
			Debug.LogWarning(string.Concat(new string[]
			{
				"tried to add destroyed thing:",
				t.Num.ToString(),
				"/",
				t.isDestroyed.ToString(),
				"/",
				(t != null) ? t.ToString() : null,
				"/",
				(this != null) ? this.ToString() : null
			}));
			return t;
		}
		if (t.parent == this)
		{
			Debug.LogWarning("already child:" + ((t != null) ? t.ToString() : null));
			return t;
		}
		if (this.things.Contains(t))
		{
			Debug.Log("already in the list" + ((t != null) ? t.ToString() : null));
			return t;
		}
		ICardParent cardParent = t.parent;
		Zone zone = EClass._zone;
		bool flag = this.IsPC && t.GetRootCard() == EClass.pc;
		if (t.parent != null)
		{
			t.parent.RemoveCard(t);
		}
		if (t.trait.ToggleType == ToggleType.Fire && destInvY == 0)
		{
			t.trait.Toggle(false, false);
		}
		t.isMasked = false;
		t.ignoreAutoPick = false;
		t.parent = this;
		t.noShadow = false;
		t.isSale = false;
		if (t.IsContainer)
		{
			t.RemoveEditorTag(EditorTag.PreciousContainer);
		}
		t.invX = -1;
		if (destInvY == -1)
		{
			t.invY = 0;
		}
		if (t.IsUnique && t.HasTag(CTAG.godArtifact))
		{
			Chara chara = t.GetRootCard() as Chara;
			if (chara != null && chara.IsPCFactionOrMinion)
			{
				this.PurgeDuplicateArtifact(t);
			}
		}
		Thing thing = tryStack ? this.things.TryStack(t, destInvX, destInvY) : t;
		if (t == thing)
		{
			this.things.Add(t);
			this.things.OnAdd(t);
		}
		if (thing == t && this.IsPC && EClass.core.IsGameStarted && EClass._map != null && this.parent == EClass.game.activeZone && this.pos.IsValid && !flag)
		{
			this.NotifyAddThing(t, t.Num);
		}
		if (t == thing && this.isThing && this.parent == EClass._zone && this.placeState != PlaceState.roaming)
		{
			EClass._map.Stocked.Add(t);
		}
		this.SetDirtyWeight();
		if (this.ShouldTrySetDirtyInventory())
		{
			EClass.pc.SetDirtyWeight();
			LayerInventory.SetDirty(thing);
		}
		if (!this.IsPC)
		{
			if (!this.IsContainer)
			{
				return thing;
			}
			Card rootCard = this.GetRootCard();
			if (rootCard == null || !rootCard.IsPC)
			{
				return thing;
			}
		}
		Card.<>c__DisplayClass692_0 CS$<>8__locals1 = new Card.<>c__DisplayClass692_0();
		t.isNPCProperty = false;
		t.isGifted = false;
		CS$<>8__locals1.count = 0;
		CS$<>8__locals1.ings = EClass.player.recipes.knownIngredients;
		CS$<>8__locals1.<AddThing>g__TryAdd|0(t);
		if (t.CanSearchContents)
		{
			t.things.Foreach(delegate(Thing _t)
			{
				base.<AddThing>g__TryAdd|0(_t);
			}, true);
		}
		if (CS$<>8__locals1.count > 0 && EClass.core.IsGameStarted)
		{
			Msg.Say((CS$<>8__locals1.count == 1) ? "newIng" : "newIngs", CS$<>8__locals1.count.ToString() ?? "", null, null, null);
		}
		return thing;
	}

	public void PurgeDuplicateArtifact(Thing af)
	{
		List<Chara> list = new List<Chara>();
		foreach (FactionBranch factionBranch in EClass.pc.faction.GetChildren())
		{
			foreach (Chara item in factionBranch.members)
			{
				list.Add(item);
			}
		}
		foreach (Chara item2 in EClass._map.charas)
		{
			list.Add(item2);
		}
		Func<Thing, bool> <>9__0;
		foreach (Chara chara in list)
		{
			if (chara.IsPCFactionOrMinion)
			{
				ThingContainer thingContainer = chara.things;
				Func<Thing, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = ((Thing t) => t.id == af.id && t != af));
				}
				List<Thing> list2 = thingContainer.List(func, false);
				if (list2.Count != 0)
				{
					foreach (Thing thing in list2)
					{
						Msg.Say("destroyed_inv_", thing, chara, null, null);
						thing.Destroy();
					}
				}
			}
		}
	}

	public void RemoveThings()
	{
		for (int i = this.things.Count - 1; i >= 0; i--)
		{
			this.RemoveThing(this.things[i]);
		}
	}

	public void RemoveThing(Thing thing)
	{
		Chara chara = this.GetRootCard() as Chara;
		if (((chara != null) ? chara.held : null) == thing)
		{
			(this.GetRootCard() as Chara).held = null;
			this.RecalculateFOV();
		}
		this.dirtyWeight = true;
		if (thing.c_equippedSlot != 0 && this.isChara)
		{
			this.Chara.body.Unequip(thing, true);
		}
		this.things.Remove(thing);
		this.things.OnRemove(thing);
		if (this.isSale && this.things.Count == 0 && this.IsContainer)
		{
			this.isSale = false;
			EClass._map.props.sales.Remove(this);
		}
		if (thing.invY == 1)
		{
			WidgetCurrentTool.dirty = true;
		}
		thing.invX = -1;
		thing.invY = 0;
		if (thing.props != null)
		{
			thing.props.Remove(thing);
		}
		this.SetDirtyWeight();
		if (this.ShouldTrySetDirtyInventory())
		{
			LayerInventory.SetDirty(thing);
			WidgetHotbar.dirtyCurrentItem = true;
			thing.parent = null;
			if (thing.trait.IsContainer)
			{
				foreach (LayerInventory layerInventory in LayerInventory.listInv.Copy<LayerInventory>())
				{
					if (layerInventory.invs[0].owner.Container.GetRootCard() != EClass.pc && layerInventory.floatInv)
					{
						EClass.ui.layerFloat.RemoveLayer(layerInventory);
					}
				}
			}
		}
		thing.parent = null;
	}

	public bool ShouldTrySetDirtyInventory()
	{
		return EClass.player.chara != null && (this.IsPC || this.GetRootCard() == EClass.pc || EClass.ui.layers.Count > 0);
	}

	public virtual bool CanStackTo(Thing to)
	{
		return false;
	}

	public bool TryStackTo(Thing to)
	{
		if (!this.CanStackTo(to))
		{
			return false;
		}
		to.ModNum(this.Num, true);
		to.decay = (to.decay * to.Num + this.decay * this.Num) / (to.Num + this.Num);
		if (this.c_isImportant)
		{
			to.c_isImportant = true;
		}
		if (EClass.core.config.game.markStack && to.GetRootCard() == EClass.pc)
		{
			to.isNew = true;
		}
		this.Destroy();
		return true;
	}

	public ICardParent GetRoot()
	{
		if (this.parent == null)
		{
			return this;
		}
		return this.parent.GetRoot();
	}

	public Card GetRootCard()
	{
		Card card = this.parent as Card;
		if (card == null)
		{
			return this;
		}
		return card.GetRootCard();
	}

	public bool IsStackable(Thing tg)
	{
		return !(this.id != tg.id) && this.material == tg.material;
	}

	public Thing Duplicate(int num)
	{
		Thing thing = ThingGen.Create(this.id, -1, -1);
		thing.ChangeMaterial(this.idMaterial);
		thing._bits1 = this._bits1;
		thing._bits2 = this._bits2;
		thing.dir = this.dir;
		thing.refVal = this.refVal;
		thing.altitude = this.altitude;
		thing.idSkin = this.idSkin;
		thing.blessedState = this.blessedState;
		thing.rarityLv = this.rarityLv;
		thing.qualityTier = this.qualityTier;
		thing.LV = this.LV;
		thing.exp = this.exp;
		thing.encLV = this.encLV;
		thing.decay = this.decay;
		thing.mapInt.Clear();
		thing.mapStr.Clear();
		foreach (KeyValuePair<int, int> keyValuePair in this.mapInt)
		{
			thing.mapInt[keyValuePair.Key] = keyValuePair.Value;
		}
		foreach (KeyValuePair<int, string> keyValuePair2 in this.mapStr)
		{
			thing.mapStr[keyValuePair2.Key] = keyValuePair2.Value;
		}
		this.elements.CopyTo(thing.elements);
		thing.SetNum(num);
		if (thing.IsRangedWeapon)
		{
			thing.sockets = IO.DeepCopy<List<int>>(this.sockets);
		}
		if (thing.c_containerSize != 0)
		{
			thing.things.SetOwner(thing);
		}
		return thing;
	}

	public Thing Split(int a)
	{
		if (a == this.Num)
		{
			return this.Thing;
		}
		Thing result = this.Duplicate(a);
		this.ModNum(-a, false);
		return result;
	}

	public Thing SetNum(int a)
	{
		if (!this.isThing)
		{
			return null;
		}
		if (a == this.Num)
		{
			return this as Thing;
		}
		this.ModNum(a - this.Num, true);
		return this as Thing;
	}

	public Thing SetNoSell()
	{
		this.noSell = true;
		return this as Thing;
	}

	public void ModNum(int a, bool notify = true)
	{
		if (this.Num + a < 0)
		{
			a = -this.Num;
		}
		this.Num += a;
		if (this.props != null)
		{
			this.props.OnNumChange(this, a);
		}
		if (this.parent != null)
		{
			this.parent.OnChildNumChange(this);
		}
		if (a > 0 && EClass.core.IsGameStarted && this.GetRootCard() == EClass.pc && notify)
		{
			this.NotifyAddThing(this.Thing, a);
		}
		this.SetDirtyWeight();
		if (this.Num <= 0)
		{
			this.Destroy();
		}
	}

	public void AddSocket()
	{
		if (this.sockets == null)
		{
			this.sockets = new List<int>();
		}
		this.sockets.Add(0);
	}

	public void ApplySocket(Thing t)
	{
		TraitMod traitMod = t.trait as TraitMod;
		if (traitMod == null || this.sockets == null)
		{
			return;
		}
		this.ApplySocket(traitMod.source.id, traitMod.owner.encLV, traitMod.owner);
	}

	public void ApplySocket(int id, int lv, Card mod = null)
	{
		int i = 0;
		while (i < this.sockets.Count)
		{
			if (this.sockets[i] == 0)
			{
				if (lv >= 100)
				{
					lv = 99;
				}
				this.sockets[i] = id * 100 + lv;
				this.elements.ModBase(id, lv);
				if (mod != null)
				{
					mod.Destroy();
					return;
				}
				break;
			}
			else
			{
				i++;
			}
		}
	}

	public void EjectSockets()
	{
		for (int i = 0; i < this.sockets.Count; i++)
		{
			int num = this.sockets[i];
			if (num != 0)
			{
				Thing thing = ThingGen.Create(this.isCopy ? "ash3" : "mod_ranged", -1, -1);
				int refVal = num / 100;
				int encLV = num % 100;
				if (!this.isCopy)
				{
					thing.refVal = refVal;
					thing.encLV = encLV;
				}
				EClass._map.TrySmoothPick(this.pos.IsBlocked ? EClass.pc.pos : this.pos, thing, EClass.pc);
				this.elements.ModBase(thing.refVal, -thing.encLV);
				this.sockets[i] = 0;
			}
		}
	}

	public void OnChildNumChange(Card c)
	{
		if (this.ShouldTrySetDirtyInventory() && c.isThing)
		{
			LayerInventory.SetDirty(c.Thing);
			WidgetCurrentTool.RefreshCurrentHotItem();
		}
	}

	public Card Install()
	{
		this.SetPlaceState(PlaceState.installed, false);
		return this;
	}

	public void SetPlaceState(PlaceState newState, bool byPlayer = false)
	{
		if (this.placeState == newState)
		{
			return;
		}
		if (this.parent != EClass._zone)
		{
			Debug.Log("tried to change placestate of non-root card:" + ((this != null) ? this.ToString() : null));
			return;
		}
		PlaceState placeState = this.placeState;
		Area area = this.pos.area;
		if (placeState == PlaceState.installed)
		{
			if (area != null)
			{
				area.OnUninstallCard(this);
			}
			if (!this.isRoofItem)
			{
				this.altitude = 0;
				this.freePos = false;
				this.fx = (this.fy = 0f);
			}
			this.trait.Uninstall();
		}
		if (placeState == PlaceState.installed || newState == PlaceState.installed)
		{
			this.ForeachPoint(delegate(Point p, bool main)
			{
				p.cell.RemoveCard(this);
			});
			this.placeState = newState;
			this.ForeachPoint(delegate(Point p, bool main)
			{
				p.cell.AddCard(this);
			});
		}
		else
		{
			this.placeState = newState;
		}
		if (newState == PlaceState.none)
		{
			this.placeState = PlaceState.roaming;
			if (this.props != null)
			{
				this.props.Remove(this);
			}
		}
		else
		{
			EClass._map.props.OnCardAddedToZone(this);
			if (this.placeState == PlaceState.installed)
			{
				if (this.isThing)
				{
					this.pos.detail.MoveThingToTop(this.Thing);
				}
				if (area != null)
				{
					area.OnInstallCard(this);
				}
				this.isRoofItem = false;
				this.trait.Install(byPlayer);
			}
		}
		if (this.trait.ShouldRefreshTile)
		{
			this.pos.RefreshNeighborTiles();
		}
		if (this.trait.ShouldTryRefreshRoom && (placeState == PlaceState.installed || this.placeState == PlaceState.installed))
		{
			EClass._map.OnSetBlockOrDoor(this.pos.x, this.pos.z);
		}
		this.trait.OnChangePlaceState(newState);
		if (EClass._zone.IsPCFaction)
		{
			EClass.Branch.resources.SetDirty();
		}
	}

	public int Quality
	{
		get
		{
			return this.Evalue(2);
		}
	}

	public int QualityLv
	{
		get
		{
			return this.Quality / 10;
		}
	}

	public int GetTotalQuality(bool applyBonus = true)
	{
		int num = 5 + this.LV + this.material.quality;
		if (applyBonus)
		{
			num = num * (100 + this.Quality) / 100;
		}
		return num;
	}

	public void ModEncLv(int a)
	{
		this.ApplyMaterialElements(true);
		this.encLV += a;
		this.ApplyMaterialElements(false);
		if (this.IsEquipmentOrRanged || this.IsAmmo)
		{
			if (this.IsWeapon || this.IsAmmo)
			{
				this.elements.ModBase(67, a);
				return;
			}
			this.elements.ModBase(65, a * 2);
		}
	}

	public void SetEncLv(int a)
	{
		this.ModEncLv(a - this.encLV);
	}

	public virtual void SetBlessedState(BlessedState s)
	{
		int num = 0;
		if (s == BlessedState.Blessed && this.blessedState < BlessedState.Blessed)
		{
			num = 1;
		}
		if (s < BlessedState.Blessed && this.blessedState == BlessedState.Blessed)
		{
			num = -1;
		}
		if (num != 0 && (this.IsEquipmentOrRanged || this.IsAmmo))
		{
			if (this.IsWeapon || this.IsAmmo)
			{
				this.elements.ModBase(67, num);
			}
			else
			{
				this.elements.ModBase(65, num * 2);
			}
		}
		this.blessedState = s;
	}

	public virtual void ChangeRarity(Rarity q)
	{
		this.rarity = q;
	}

	public bool TryPay(int a, string id = "money")
	{
		if (this.GetCurrency(id) < a)
		{
			if (this.IsPC)
			{
				SE.Beep();
				Msg.Say((id == "ration") ? "notEnoughFood" : "notEnoughMoney");
			}
			return false;
		}
		if (this.IsPC && !(id == "ration"))
		{
			SE.Pay();
		}
		this.ModCurrency(-a, id);
		return true;
	}

	public void SetCharge(int a)
	{
		this.c_charges = a;
		LayerInventory.SetDirty(this.Thing);
	}

	public void ModCharge(int a, bool destroy = false)
	{
		this.c_charges += a;
		LayerInventory.SetDirty(this.Thing);
		if (this.c_charges <= 0 && destroy)
		{
			this.Say("spellbookCrumble", this, null, null);
			this.ModNum(-1, true);
		}
	}

	public void ModCurrency(int a, string id = "money")
	{
		if (a == 0)
		{
			return;
		}
		if (id == "influence")
		{
			EClass._zone.ModInfluence(a);
			return;
		}
		SourceMaterial.Row mat = null;
		this.things.AddCurrency(this, id, a, mat);
	}

	public int GetCurrency(string id = "money")
	{
		if (id == "influence")
		{
			return EClass._zone.influence;
		}
		int result = 0;
		SourceMaterial.Row mat = null;
		this.things.GetCurrency(id, ref result, mat);
		return result;
	}

	public virtual void HealHPHost(int a, HealSource origin = HealSource.None)
	{
		if (this.isChara)
		{
			if (this.Chara.parasite != null)
			{
				this.Chara.parasite.HealHP(a, HealSource.None);
			}
			if (this.Chara.ride != null)
			{
				this.Chara.ride.HealHP(a, HealSource.None);
			}
		}
		this.HealHP(a, origin);
	}

	public virtual void HealHP(int a, HealSource origin = HealSource.None)
	{
		this.hp += a;
		if (this.hp > this.MaxHP)
		{
			this.hp = this.MaxHP;
		}
		if (origin == HealSource.Magic)
		{
			this.PlaySound("heal", 1f, true);
			this.PlayEffect("heal", true, 0f, default(Vector3));
			return;
		}
		if (origin != HealSource.HOT)
		{
			return;
		}
		this.PlaySound("heal_tick", 1f, true);
		this.PlayEffect("heal_tick", true, 0f, default(Vector3));
	}

	public virtual int GetArmorSkill()
	{
		return 0;
	}

	public virtual int ApplyProtection(int dmg, int mod = 100)
	{
		int armorSkill = this.GetArmorSkill();
		Element orCreateElement = this.elements.GetOrCreateElement(armorSkill);
		int num = this.PV + orCreateElement.Value + this.DEX / 10;
		int num2 = 1;
		int sides = 1;
		int bonus = 0;
		if (num > 0)
		{
			int num3 = num / 4;
			num2 = num3 / 10 + 1;
			sides = num3 / num2 + 1;
			bonus = 0;
			dmg = dmg * 100 / (100 + num);
		}
		int num4 = Dice.Roll(num2, sides, bonus, this);
		dmg -= num4 * mod / 100;
		if (dmg < 0)
		{
			dmg = 0;
		}
		return dmg;
	}

	public void DamageHP(int dmg, AttackSource attackSource = AttackSource.None, Card origin = null)
	{
		this.DamageHP(dmg, 0, 0, attackSource, origin, true);
	}

	public void DamageHP(int dmg, int ele, int eleP = 100, AttackSource attackSource = AttackSource.None, Card origin = null, bool showEffect = true)
	{
		Card.<>c__DisplayClass733_0 CS$<>8__locals1 = new Card.<>c__DisplayClass733_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.dmg = dmg;
		CS$<>8__locals1.origin = origin;
		if (this.hp < 0)
		{
			return;
		}
		if (ele == 0)
		{
			CS$<>8__locals1.e = Element.Void;
		}
		else
		{
			CS$<>8__locals1.e = Element.Create(ele, 0);
			if (attackSource != AttackSource.Condition && showEffect)
			{
				ActEffect.TryDelay(delegate
				{
					CS$<>8__locals1.<>4__this.PlayEffect(CS$<>8__locals1.e.id, true, 0.25f);
					EClass.Sound.Play("atk_" + CS$<>8__locals1.e.source.alias);
				});
			}
			if (!CS$<>8__locals1.e.source.aliasRef.IsEmpty() && attackSource != AttackSource.ManaBackfire)
			{
				CS$<>8__locals1.dmg = Element.GetResistDamage(CS$<>8__locals1.dmg, this.Evalue(CS$<>8__locals1.e.source.aliasRef));
				CS$<>8__locals1.dmg = CS$<>8__locals1.dmg * 100 / (100 + Mathf.Clamp(this.Evalue(961) * 5, -50, 200));
			}
			int dmg2 = CS$<>8__locals1.e.id;
			if (dmg2 != 910)
			{
				if (dmg2 == 912)
				{
					Chara chara = this.Chara;
					if (chara != null && chara.isWet)
					{
						CS$<>8__locals1.dmg = CS$<>8__locals1.dmg * 150 / 100;
					}
				}
			}
			else
			{
				Chara chara2 = this.Chara;
				if (chara2 != null && chara2.isWet)
				{
					CS$<>8__locals1.dmg /= 3;
				}
			}
		}
		if (attackSource != AttackSource.Finish)
		{
			if (!this.IsPCFaction && this.LV > 50)
			{
				CS$<>8__locals1.dmg = CS$<>8__locals1.dmg * (100 - (int)Mathf.Min(80f, Mathf.Sqrt((float)(this.LV - 50)) * 2.5f)) / 100;
			}
			CS$<>8__locals1.dmg = CS$<>8__locals1.dmg * Mathf.Max(0, 100 - this.Evalue((CS$<>8__locals1.e == Element.Void) ? 55 : 56)) / 100;
			if (CS$<>8__locals1.origin != null && CS$<>8__locals1.origin.IsPC && EClass.player.codex.Has(this.id))
			{
				CS$<>8__locals1.dmg = CS$<>8__locals1.dmg * (100 + Mathf.Min(10, EClass.player.codex.GetOrCreate(this.id).weakspot)) / 100;
			}
			if (this.isChara && this.Chara.body.GetAttackStyle() == AttackStyle.Shield && this.elements.ValueWithoutLink(123) >= 5 && CS$<>8__locals1.e == Element.Void)
			{
				CS$<>8__locals1.dmg = CS$<>8__locals1.dmg * 90 / 100;
			}
			if (this.Evalue(971) > 0)
			{
				CS$<>8__locals1.dmg = CS$<>8__locals1.dmg * 100 / Mathf.Clamp(100 + this.Evalue(971), 25, 1000);
			}
			if (this.HasElement(1305, 1))
			{
				CS$<>8__locals1.dmg = CS$<>8__locals1.dmg * 90 / 100;
			}
			if (EClass.pc.HasElement(1207, 1) && this.isChara)
			{
				int num = 0;
				int num2 = 0;
				foreach (Condition condition in this.Chara.conditions)
				{
					if (condition.Type == ConditionType.Buff)
					{
						num++;
					}
					else if (condition.Type == ConditionType.Debuff)
					{
						num2++;
					}
				}
				if (this.IsPCParty)
				{
					CS$<>8__locals1.dmg = CS$<>8__locals1.dmg * 100 / Mathf.Min(100 + num * 5, 120);
				}
				else
				{
					CS$<>8__locals1.dmg = CS$<>8__locals1.dmg * Mathf.Min(100 + num2 * 5, 120) / 100;
				}
			}
			if (this.IsPCParty && EClass.pc.ai is GoalAutoCombat)
			{
				CS$<>8__locals1.dmg = CS$<>8__locals1.dmg - EClass.pc.Evalue(13) - 1;
			}
			else if (CS$<>8__locals1.origin != null && CS$<>8__locals1.origin.IsPCParty && EClass.pc.ai is GoalAutoCombat)
			{
				CS$<>8__locals1.dmg = CS$<>8__locals1.dmg * 100 / Mathf.Max(110 - EClass.pc.Evalue(13), 105);
			}
			if (this.HasElement(1218, 1))
			{
				CS$<>8__locals1.dmg = CS$<>8__locals1.dmg * (1000 - this.Evalue(1218)) / 1000;
				if (CS$<>8__locals1.dmg <= 0 && EClass.rnd(4) == 0)
				{
					int dmg2 = CS$<>8__locals1.dmg;
					CS$<>8__locals1.dmg = dmg2 + 1;
				}
			}
			if (CS$<>8__locals1.dmg >= this.MaxHP / 10 && this.Evalue(68) > 0)
			{
				int num3 = this.MaxHP / 10;
				int num4 = CS$<>8__locals1.dmg - num3;
				num4 = num4 * 100 / (200 + this.Evalue(68) * 10);
				CS$<>8__locals1.dmg = num3 + num4;
			}
		}
		if (CS$<>8__locals1.origin != null && CS$<>8__locals1.origin.IsPC && EClass.pc.Evalue(654) > 0)
		{
			CS$<>8__locals1.dmg = 0;
		}
		if (CS$<>8__locals1.dmg < 0)
		{
			CS$<>8__locals1.dmg = 0;
		}
		int num5 = Mathf.Clamp(CS$<>8__locals1.dmg * 6 / this.MaxHP, 0, 4) + ((CS$<>8__locals1.dmg > 0) ? 1 : 0);
		if (this.Evalue(1421) > 0)
		{
			int num6 = CS$<>8__locals1.dmg;
			if (this.hp > 0)
			{
				num6 = CS$<>8__locals1.dmg - this.hp;
				this.hp -= CS$<>8__locals1.dmg;
				if (this.hp < 0 && this.Chara.mana.value >= 0)
				{
					this.hp = 0;
				}
			}
			if (this.hp <= 0)
			{
				if (this.Evalue(1421) >= 2)
				{
					num6 /= 2;
				}
				CS$<>8__locals1.dmg = num6;
				if (this.Chara.mana.value > 0)
				{
					num6 -= this.Chara.mana.value;
					this.Chara.mana.value -= CS$<>8__locals1.dmg;
				}
				if (this.Chara.mana.value <= 0)
				{
					this.hp -= num6;
				}
			}
		}
		else
		{
			this.hp -= CS$<>8__locals1.dmg;
		}
		if (this.isSynced && CS$<>8__locals1.dmg != 0)
		{
			float ratio = (float)CS$<>8__locals1.dmg / (float)this.MaxHP;
			Card c = (this.parent is Chara) ? (this.parent as Chara) : this;
			ActEffect.TryDelay(delegate
			{
				c.PlayEffect("blood", true, 0f, default(Vector3)).SetParticleColor(EClass.Colors.matColors[CS$<>8__locals1.<>4__this.material.alias].main).Emit(20 + (int)(30f * ratio));
				if (EClass.core.config.test.showNumbers || CS$<>8__locals1.<>4__this.isThing)
				{
					Popper popper = EClass.scene.popper.Pop(CS$<>8__locals1.<>4__this.renderer.PositionCenter(), "DamageNum");
					Color color = c.IsPC ? EClass.Colors.textColors.damagePC : (c.IsPCFaction ? EClass.Colors.textColors.damagePCParty : EClass.Colors.textColors.damage);
					if (CS$<>8__locals1.e != Element.Void)
					{
						color = EClass.Colors.elementColors.TryGetValue(CS$<>8__locals1.e.source.alias, default(Color));
						float num13 = (color.r + color.g + color.b) / 3f;
						num13 = ((num13 > 0.5f) ? 0f : (0.6f - num13));
						color = new Color(color.r + num13, color.g + num13, color.b + num13, 1f);
					}
					popper.SetText(CS$<>8__locals1.dmg.ToString() ?? "", color);
				}
			});
		}
		ZoneInstanceBout zoneInstanceBout = EClass._zone.instance as ZoneInstanceBout;
		if (this.hp < 0 && Religion.recentWrath == null)
		{
			if (this.isRestrained && this.IsPCFaction && EClass._zone.IsPCFaction && (!this.IsPC || (this.Chara.ai is AI_Torture && this.Chara.ai.IsRunning)))
			{
				CS$<>8__locals1.<DamageHP>g__EvadeDeath|1();
				if (this.Chara.stamina.value > 0 && (EClass.rnd(2) == 0 || !this.IsPC))
				{
					this.Chara.stamina.Mod(-1);
				}
			}
			else if (this.IsInstalled && this.pos.HasBlock && this.trait.IsDoor)
			{
				CS$<>8__locals1.<DamageHP>g__EvadeDeath|1();
			}
			else if (!this.trait.CanBeDestroyed)
			{
				CS$<>8__locals1.<DamageHP>g__EvadeDeath|1();
			}
			else if (this.HasEditorTag(EditorTag.Invulnerable) || (this.HasEditorTag(EditorTag.InvulnerableToMobs) && (CS$<>8__locals1.origin == null || !CS$<>8__locals1.origin.IsPCParty)))
			{
				CS$<>8__locals1.<DamageHP>g__EvadeDeath|1();
			}
			else if (this.isChara)
			{
				if (this.Chara.HasCondition<ConInvulnerable>())
				{
					CS$<>8__locals1.<DamageHP>g__EvadeDeath|1();
				}
				else if (this.IsPC && EClass.debug.godMode)
				{
					CS$<>8__locals1.<DamageHP>g__EvadeDeath|1();
				}
				else if (this.Chara.host != null)
				{
					CS$<>8__locals1.<DamageHP>g__EvadeDeath|1();
				}
				else if (zoneInstanceBout != null && LayerDrama.Instance)
				{
					CS$<>8__locals1.<DamageHP>g__EvadeDeath|1();
				}
				else if (LayerDrama.IsActive() && this.IsPC)
				{
					CS$<>8__locals1.<DamageHP>g__EvadeDeath|1();
				}
				else
				{
					if (attackSource != AttackSource.Finish && this.IsPCParty && this.Chara.host == null && EClass.pc.ai is GoalAutoCombat)
					{
						if (!EClass.player.invlunerable && (EClass.pc.ai as GoalAutoCombat).listHealthy.Contains(this.Chara))
						{
							List<Action> actionsNextFrame = EClass.core.actionsNextFrame;
							Action item;
							if ((item = CS$<>8__locals1.<>9__5) == null)
							{
								item = (CS$<>8__locals1.<>9__5 = delegate()
								{
									Msg.Say(CS$<>8__locals1.<>4__this.IsPC ? "abort_damage" : "abort_damgeAlly");
								});
							}
							actionsNextFrame.Add(item);
							EClass.player.invlunerable = true;
							EClass.player.TryAbortAutoCombat();
							EClass.pc.stamina.Mod(-EClass.pc.stamina.max / 5);
						}
						if (EClass.player.invlunerable)
						{
							CS$<>8__locals1.<DamageHP>g__EvadeDeath|1();
							goto IL_99C;
						}
					}
					if (this.IsPC && this.Evalue(1220) > 0 && this.Chara.stamina.value >= this.Chara.stamina.max / 2)
					{
						this.Chara.stamina.Mod(-this.Chara.stamina.max / 2);
						this.Chara.AddCondition<ConInvulnerable>(100, false);
						CS$<>8__locals1.<DamageHP>g__EvadeDeath|1();
					}
				}
			}
		}
		IL_99C:
		if (this.trait.CanBeAttacked)
		{
			this.renderer.PlayAnime(AnimeID.HitObj, default(Vector3), false);
			this.hp = this.MaxHP;
		}
		if (this.hp < 0)
		{
			if ((attackSource == AttackSource.Melee || attackSource == AttackSource.Range) && CS$<>8__locals1.origin != null && (CS$<>8__locals1.origin.isSynced || this.IsPC))
			{
				string text = "";
				if (this.IsPC && Lang.setting.combatTextStyle == 0)
				{
					if (CS$<>8__locals1.e != Element.Void && CS$<>8__locals1.e != null)
					{
						text = "dead_" + CS$<>8__locals1.e.source.alias;
					}
					if (text == "" || !LangGame.Has(text))
					{
						text = "dead_attack";
					}
					EClass.pc.Say(text, this, "", null);
				}
				else
				{
					if (CS$<>8__locals1.e != Element.Void && CS$<>8__locals1.e != null)
					{
						text = "kill_" + CS$<>8__locals1.e.source.alias;
					}
					if (text == "" || !LangGame.Has(text))
					{
						text = "kill_attack";
					}
					(this.IsPC ? EClass.pc : CS$<>8__locals1.origin).Say(text, CS$<>8__locals1.origin, this, null, null);
				}
			}
			if (this.isChara && Religion.recentWrath == null)
			{
				if (this.HasElement(1410, 1) && !this.Chara.HasCooldown(1410))
				{
					this.Chara.AddCooldown(1410, 0);
					this.Say("reboot", this, null, null);
					this.PlaySound("reboot", 1f, true);
					this.Chara.Cure(CureType.Boss, 100, BlessedState.Normal);
					this.hp = this.MaxHP / 3;
					this.Chara.AddCondition<ConInvulnerable>(100, false);
					return;
				}
				foreach (Chara chara3 in EClass._map.charas)
				{
					if (this.Chara.IsFriendOrAbove(chara3) && chara3.HasElement(1408, 1) && chara3.faith == EClass.game.religions.Healing && EClass.world.date.GetRawDay() != chara3.GetInt(58, null) && (!chara3.IsPCFaction || this.IsPCFaction))
					{
						Msg.alwaysVisible = true;
						Msg.Say("layhand", chara3, this, null, null);
						Msg.Say("pray_heal", this, null, null, null);
						this.hp = this.MaxHP;
						this.Chara.AddCondition<ConInvulnerable>(100, false);
						this.PlayEffect("revive", true, 0f, default(Vector3));
						this.PlaySound("revive", 1f, true);
						chara3.SetInt(58, EClass.world.date.GetRawDay());
						return;
					}
				}
			}
			if (zoneInstanceBout != null)
			{
				Chara chara4 = EClass._map.FindChara(zoneInstanceBout.uidTarget);
				if (chara4 != null)
				{
					if (this.IsPC)
					{
						EClass.pc.hp = 0;
						chara4.ShowDialog("_chara", "bout_lose", "");
						return;
					}
					if (chara4 == this)
					{
						this.hp = 0;
						chara4.ModAffinity(EClass.pc, 10, true);
						chara4.ShowDialog("_chara", "bout_win", "");
						return;
					}
				}
			}
			if (!this.isDestroyed)
			{
				this.Die(CS$<>8__locals1.e, CS$<>8__locals1.origin, attackSource);
			}
			if (CS$<>8__locals1.origin != null && CS$<>8__locals1.origin.isChara)
			{
				if (CS$<>8__locals1.origin.IsPCFactionOrMinion && this.isChara && !this.isCopy)
				{
					EClass.player.stats.kills++;
					EClass.game.quests.list.ForeachReverse(delegate(Quest q)
					{
						q.OnKillChara(CS$<>8__locals1.<>4__this.Chara);
					});
					EClass.player.codex.AddKill(this.id);
					if (Guild.Fighter.CanGiveContribution(this.Chara))
					{
						Guild.Fighter.AddContribution(5 + this.LV / 5);
					}
					if (Guild.Fighter.HasBounty(this.Chara))
					{
						int a = EClass.rndHalf(200 + this.LV * 20);
						Msg.Say("bounty", this.Chara, a.ToString() ?? "", null, null);
						EClass.pc.ModCurrency(a, "money");
						SE.Pay();
					}
				}
				if (CS$<>8__locals1.origin.IsPCParty && EClass.pc.Evalue(1355) > 0)
				{
					ConStrife conStrife = (EClass.pc.AddCondition<ConStrife>(100, false) as ConStrife) ?? EClass.pc.GetCondition<ConStrife>();
					if (conStrife != null)
					{
						conStrife.AddKill();
					}
				}
				if (CS$<>8__locals1.origin.GetInt(106, null) == 0)
				{
					CS$<>8__locals1.origin.Chara.TalkTopic("kill");
				}
			}
			Msg.SetColor();
		}
		else if ((attackSource == AttackSource.Melee || attackSource == AttackSource.Range) && CS$<>8__locals1.origin != null)
		{
			(this.IsPC ? EClass.pc : CS$<>8__locals1.origin).Say("dmgMelee" + num5.ToString() + (this.IsPC ? "pc" : ""), CS$<>8__locals1.origin, this, null, null);
		}
		else if (this.isChara)
		{
			int num7 = (attackSource == AttackSource.Condition || attackSource == AttackSource.WeaponEnchant) ? 2 : 1;
			if (num5 >= num7)
			{
				if (CS$<>8__locals1.e != Element.Void)
				{
					this.Say("dmg_" + CS$<>8__locals1.e.source.alias, this, null, null);
				}
				if (CS$<>8__locals1.e == Element.Void || num5 >= 2)
				{
					this.Say("dmg" + num5.ToString(), this, null, null);
				}
			}
		}
		if (this.isChara && CS$<>8__locals1.origin != null && CS$<>8__locals1.origin.IsAliveInCurrentZone && CS$<>8__locals1.origin.isChara)
		{
			if (CS$<>8__locals1.e.id == 916)
			{
				CS$<>8__locals1.origin.HealHP(Mathf.Clamp(EClass.rnd(CS$<>8__locals1.dmg * (50 + eleP) / 500 + 5), 1, CS$<>8__locals1.origin.MaxHP / 5 + EClass.rnd(10)), HealSource.None);
			}
			if ((attackSource == AttackSource.Melee || attackSource == AttackSource.Range) && CS$<>8__locals1.origin.Dist(this) <= 1)
			{
				if (attackSource == AttackSource.Melee && this.HasElement(1221, 1))
				{
					int ele2 = (this.Chara.MainElement == Element.Void) ? 924 : this.Chara.MainElement.id;
					this.Say("reflect_thorne", this, CS$<>8__locals1.origin, null, null);
					CS$<>8__locals1.origin.DamageHP(Mathf.Clamp(CS$<>8__locals1.dmg / 20, 1, this.MaxHP / 20), ele2, this.Power, AttackSource.Condition, null, true);
				}
				if (this.HasElement(1223, 1))
				{
					int ele3 = (this.Chara.MainElement == Element.Void) ? 923 : this.Chara.MainElement.id;
					this.Say("reflect_acid", this, CS$<>8__locals1.origin, null, null);
					CS$<>8__locals1.origin.DamageHP(Mathf.Clamp(CS$<>8__locals1.dmg / 20, 1, this.MaxHP / 20), ele3, this.Power * 2, AttackSource.Condition, null, true);
				}
			}
			if (CS$<>8__locals1.origin.HasElement(662, 1) && attackSource == AttackSource.Melee && CS$<>8__locals1.origin.isChara && this.Chara.IsHostile(CS$<>8__locals1.origin as Chara))
			{
				int num8 = EClass.rnd(3 + Mathf.Clamp(CS$<>8__locals1.dmg / 100, 0, CS$<>8__locals1.origin.Evalue(662) / 10));
				CS$<>8__locals1.origin.Chara.stamina.Mod(num8);
				if (this.IsAliveInCurrentZone)
				{
					this.Chara.stamina.Mod(-num8);
				}
			}
			if (CS$<>8__locals1.origin.HasElement(1350, 1) && attackSource == AttackSource.Melee)
			{
				int num9 = EClass.rndHalf(2 + Mathf.Clamp(CS$<>8__locals1.dmg / 10, 0, CS$<>8__locals1.origin.Chara.GetPietyValue() + 10));
				CS$<>8__locals1.origin.Chara.mana.Mod(num9);
				if (this.IsAliveInCurrentZone)
				{
					this.Chara.mana.Mod(-num9);
				}
			}
			if (CS$<>8__locals1.origin.HasElement(661, 1) && attackSource == AttackSource.Melee)
			{
				int num10 = EClass.rnd(2 + Mathf.Clamp(CS$<>8__locals1.dmg / 10, 0, CS$<>8__locals1.origin.Evalue(661) + 10));
				CS$<>8__locals1.origin.Chara.mana.Mod(num10);
				if (this.IsAliveInCurrentZone)
				{
					this.Chara.mana.Mod(-num10);
				}
			}
		}
		if (this.hp < 0)
		{
			return;
		}
		if (!this.isChara)
		{
			return;
		}
		if (CS$<>8__locals1.dmg > 0)
		{
			int a2 = 100 * (CS$<>8__locals1.dmg * 100 / this.MaxHP) / 100;
			a2 = Mathf.Min(a2, this.Chara.isRestrained ? 15 : 200);
			this.elements.ModExp(this.GetArmorSkill(), a2, false);
			if (this.Chara.body.GetAttackStyle() == AttackStyle.Shield)
			{
				this.elements.ModExp(123, a2, false);
			}
		}
		int num11 = (EClass.rnd(2) == 0) ? 1 : 0;
		if (attackSource == AttackSource.Condition)
		{
			num11 = 1 + EClass.rnd(2);
		}
		if (num11 > 0)
		{
			bool flag = this.Chara.HasCondition<ConPoison>() || ((CS$<>8__locals1.e.id == 915 || CS$<>8__locals1.e.id == 923) && this.ResistLv(this.Evalue(955)) < 4);
			this.AddBlood(num11, flag ? 6 : -1);
		}
		bool flag2 = true;
		switch (CS$<>8__locals1.e.id)
		{
		case 910:
			if (CS$<>8__locals1.<DamageHP>g__Chance|3(30 + eleP / 5, 100))
			{
				this.Chara.AddCondition<ConBurning>(eleP, false);
			}
			break;
		case 911:
			if (this.Chara.isWet)
			{
				if (CS$<>8__locals1.<DamageHP>g__Chance|3(30 + eleP / 10, 100))
				{
					this.Chara.AddCondition<ConFreeze>(eleP, false);
				}
			}
			else if (CS$<>8__locals1.<DamageHP>g__Chance|3(50 + eleP / 10, 100))
			{
				this.Chara.AddCondition<ConWet>(eleP, false);
			}
			break;
		case 912:
			if (CS$<>8__locals1.<DamageHP>g__Chance|3(75 + eleP / 20, 100) && EClass.rnd(3) == 0)
			{
				this.Chara.AddCondition<ConParalyze>(1, true);
			}
			break;
		case 913:
			if (CS$<>8__locals1.<DamageHP>g__Chance|3(30 + eleP / 5, 100))
			{
				this.Chara.AddCondition<ConBlind>(eleP, false);
			}
			break;
		case 914:
			flag2 = false;
			if (EClass.rnd(3) != 0)
			{
				if (CS$<>8__locals1.<DamageHP>g__Chance|3(30 + eleP / 5, 100))
				{
					this.Chara.AddCondition<ConConfuse>(eleP, false);
				}
			}
			else if (CS$<>8__locals1.<DamageHP>g__Chance|3(30 + eleP / 5, 100))
			{
				this.Chara.AddCondition<ConSleep>(eleP, false);
			}
			if (CS$<>8__locals1.<DamageHP>g__Chance|3(50, 100))
			{
				this.Chara.SAN.Mod(EClass.rnd(2));
			}
			break;
		case 915:
			if (CS$<>8__locals1.<DamageHP>g__Chance|3(30 + eleP / 5, 100))
			{
				this.Chara.AddCondition<ConPoison>(eleP, false);
			}
			break;
		case 917:
			if (CS$<>8__locals1.<DamageHP>g__Chance|3(50 + eleP / 10, 100))
			{
				this.Chara.AddCondition<ConDim>(eleP, false);
			}
			break;
		case 918:
			flag2 = false;
			if (CS$<>8__locals1.<DamageHP>g__Chance|3(30 + eleP / 5, 100))
			{
				this.Chara.AddCondition<ConParalyze>(eleP, false);
			}
			break;
		case 920:
			flag2 = false;
			if (CS$<>8__locals1.<DamageHP>g__Chance|3(5 + eleP / 25, 40))
			{
				this.Chara.AddCondition<ConBlind>(eleP / 2, false);
			}
			if (CS$<>8__locals1.<DamageHP>g__Chance|3(5 + eleP / 25, 40))
			{
				this.Chara.AddCondition<ConParalyze>(eleP / 2, false);
			}
			if (CS$<>8__locals1.<DamageHP>g__Chance|3(5 + eleP / 25, 40))
			{
				this.Chara.AddCondition<ConConfuse>(eleP / 2, false);
			}
			if (CS$<>8__locals1.<DamageHP>g__Chance|3(5 + eleP / 25, 40))
			{
				this.Chara.AddCondition<ConPoison>(eleP / 2, false);
			}
			if (CS$<>8__locals1.<DamageHP>g__Chance|3(5 + eleP / 25, 40))
			{
				this.Chara.AddCondition<ConSleep>(eleP / 2, false);
			}
			if (CS$<>8__locals1.<DamageHP>g__Chance|3(5 + eleP / 25, 40))
			{
				this.Chara.AddCondition<ConDim>(eleP / 2, false);
			}
			if (CS$<>8__locals1.<DamageHP>g__Chance|3(30 + eleP / 10, 100))
			{
				this.Chara.SAN.Mod(EClass.rnd(2));
			}
			break;
		case 922:
			this.Chara.ModCorruption(EClass.rnd(eleP / 50 + 10));
			break;
		case 923:
			if (CS$<>8__locals1.<DamageHP>g__Chance|3(50 + eleP / 10, 100) && EClass.rnd(4) == 0)
			{
				ActEffect.Proc(EffectId.Acid, this.Chara, null, 100, default(ActRef));
			}
			break;
		case 924:
			if (CS$<>8__locals1.<DamageHP>g__Chance|3(50 + eleP / 10, 100))
			{
				this.Chara.AddCondition<ConBleed>(eleP, false);
			}
			break;
		case 925:
			if (EClass.rnd(3) == 0)
			{
				if (CS$<>8__locals1.<DamageHP>g__Chance|3(30 + eleP / 5, 100))
				{
					this.Chara.AddCondition<ConDim>(eleP, false);
				}
			}
			else if (EClass.rnd(2) == 0)
			{
				if (EClass.rnd(3) == 0)
				{
					this.Chara.AddCondition<ConParalyze>(1, true);
				}
			}
			else if (EClass.rnd(2) == 0)
			{
				this.Chara.AddCondition<ConConfuse>(1 + EClass.rnd(3), true);
			}
			break;
		}
		if (CS$<>8__locals1.origin != null && CS$<>8__locals1.origin.HasElement(1411, 1) && !this.Chara.HasCondition<ConGravity>())
		{
			Condition.ignoreEffect = true;
			this.Chara.AddCondition<ConGravity>(2000, false);
			Condition.ignoreEffect = false;
		}
		if (this.Chara.conSleep != null && flag2)
		{
			this.Chara.conSleep.Kill(false);
		}
		if (this.Chara.host != null && this.hp == 0 && !this.Chara.HasCondition<ConFaint>())
		{
			this.Chara.AddCondition<ConFaint>(200, true);
		}
		if (this.IsPC)
		{
			float num12 = (float)this.hp / (float)this.MaxHP;
			if (this.Evalue(1421) > 0)
			{
				num12 = (float)this.Chara.mana.value / (float)this.Chara.mana.max;
			}
			if (num12 < 0.3f)
			{
				this.PlaySound("heartbeat", 1f - num12 * 2f, true);
			}
		}
		if (!this.IsPC && this.hp < this.MaxHP / 5 && this.Evalue(423) <= 0 && CS$<>8__locals1.dmg * 100 / this.MaxHP + 10 > EClass.rnd(this.IsPowerful ? 400 : 150) && !this.HasCondition<ConFear>())
		{
			this.Chara.AddCondition<ConFear>(100 + EClass.rnd(100), false);
		}
		if (this.Chara.ai.Current.CancelWhenDamaged && attackSource != AttackSource.Hunger && attackSource != AttackSource.Fatigue)
		{
			this.Chara.ai.Current.TryCancel(CS$<>8__locals1.origin);
		}
		if (this.Chara.HasCondition<ConWeapon>())
		{
			ConWeapon condition2 = this.Chara.GetCondition<ConWeapon>();
			if (CS$<>8__locals1.e.source.aliasRef == condition2.sourceElement.aliasRef)
			{
				condition2.Mod(-1, false);
			}
		}
		if (this.Chara.HasElement(1222, 1) && (CS$<>8__locals1.dmg >= this.MaxHP / 10 || EClass.rnd(20) == 0))
		{
			ActEffect.Proc(EffectId.Duplicate, this, null, 100, default(ActRef));
		}
		if (this.hp < this.MaxHP / 3 && this.HasElement(1409, 1) && !this.Chara.HasCooldown(1409))
		{
			this.Chara.AddCooldown(1409, 0);
			this.Chara.AddCondition<ConBoost>(this.Power, false);
			this.Chara.Cure(CureType.Boss, 100, BlessedState.Normal);
			this.Chara.HealHP(this.MaxHP / 2, HealSource.None);
		}
		if (CS$<>8__locals1.origin != null && CS$<>8__locals1.origin.isChara && attackSource != AttackSource.Finish)
		{
			if (!AI_PlayMusic.ignoreDamage)
			{
				this.Chara.TrySetEnemy(CS$<>8__locals1.origin.Chara);
			}
			if (CS$<>8__locals1.origin.Evalue(428) > 0 && !this.IsPCFactionOrMinion && EClass.rnd(CS$<>8__locals1.dmg) >= EClass.rnd(this.MaxHP / 10) + this.MaxHP / 100 + 1)
			{
				CS$<>8__locals1.origin.Chara.TryNeckHunt(this.Chara, CS$<>8__locals1.origin.Evalue(428) * 20, true);
			}
		}
	}

	public virtual void Die(Element e = null, Card origin = null, AttackSource attackSource = AttackSource.None)
	{
		Card rootCard = this.GetRootCard();
		Point _pos = ((rootCard != null) ? rootCard.pos : null) ?? this.pos;
		if (_pos != null && !_pos.IsValid)
		{
			_pos = null;
		}
		if (this.trait.EffectDead == EffectDead.Default && _pos != null)
		{
			_pos.PlaySound(this.material.GetSoundDead(this.sourceCard), true, 1f, true);
			_pos.PlayEffect("mine").SetParticleColor(this.material.GetColor()).Emit(10 + EClass.rnd(10));
			this.material.AddBlood(_pos, 6);
			if (_pos.IsSync)
			{
				string text = (rootCard != this) ? "destroyed_inv_" : "destroyed_ground_";
				if (e != null && LangGame.Has(text + e.source.alias))
				{
					text += e.source.alias;
				}
				if (attackSource != AttackSource.Throw)
				{
					Msg.Say(text, this, rootCard, null, null);
				}
			}
			else if (attackSource != AttackSource.Throw)
			{
				Msg.Say("destroyed", this, null, null, null);
			}
		}
		if (_pos != null && !EClass._zone.IsUserZone)
		{
			this.things.ForeachReverse(delegate(Thing t)
			{
				if (t.trait is TraitChestMerchant)
				{
					return;
				}
				EClass._zone.AddCard(t, _pos);
			});
		}
		this.Destroy();
		if (e != null && _pos != null && e.id == 21)
		{
			EClass._zone.AddCard(ThingGen.Create((EClass.rnd(2) == 0) ? "ash" : "ash2", -1, -1), _pos);
		}
		if (this.trait.ThrowType == ThrowType.Explosive)
		{
			this.Explode(this.pos, origin);
		}
	}

	public void Explode(Point p, Card origin)
	{
		ActEffect.ProcAt(EffectId.Explosive, 100, this.blessedState, this, null, p, true, new ActRef
		{
			origin = ((origin != null) ? origin.Chara : null),
			refThing = this.Thing,
			aliasEle = "eleImpact"
		});
	}

	public void Deconstruct()
	{
		this.PlaySound(this.material.GetSoundDead(this.sourceCard), 1f, true);
		this.Destroy();
	}

	public void Destroy()
	{
		if (this.isDestroyed)
		{
			Debug.Log(this.Name + " is already destroyed.");
			return;
		}
		if (this.isChara)
		{
			if (this.IsPCFaction && !this.Chara.isSummon)
			{
				Debug.Log(this);
				return;
			}
			this.Chara.DropHeld(null);
			this.Chara.isDead = true;
			if (this.IsPCParty)
			{
				EClass.pc.party.RemoveMember(this.Chara);
			}
			if (this.IsGlobal)
			{
				EClass.game.cards.globalCharas.Remove(this.Chara);
			}
		}
		if (this.renderer.hasActor)
		{
			this.renderer.KillActor();
		}
		if (this.parent != null)
		{
			this.parent.RemoveCard(this);
		}
		for (int i = this.things.Count - 1; i >= 0; i--)
		{
			this.things[i].Destroy();
		}
		this.isDestroyed = true;
	}

	public void SpawnLoot(Card origin)
	{
		if (!this.isChara || this.IsPCFactionMinion || (this.isCopy && EClass.rnd(10) != 0))
		{
			return;
		}
		bool isUserZone = EClass._zone.IsUserZone;
		bool flag = EClass._zone is Zone_Music;
		List<Card> list = new List<Card>();
		SourceRace.Row race = this.Chara.race;
		if (!this.IsPCFaction && !isUserZone && this.sourceCard.idActor.IsEmpty())
		{
			int i = 500;
			if (this.rarity >= Rarity.Legendary)
			{
				i = ((!EClass.player.codex.DroppedCard(this.id)) ? 1 : 10);
				EClass.player.codex.MarkCardDrop(this.id);
			}
			if (this.trait is TraitAdventurerBacker)
			{
				i = 10;
			}
			if (this.<SpawnLoot>g__chance|738_0(i))
			{
				Thing thing = ThingGen.Create("figure", -1, -1);
				thing.MakeFigureFrom(this.id);
				list.Add(thing);
			}
			if (this.<SpawnLoot>g__chance|738_0(i))
			{
				Thing thing2 = ThingGen.Create("figure3", -1, -1);
				thing2.MakeFigureFrom(this.id);
				list.Add(thing2);
			}
		}
		bool flag2 = this.Chara.race.corpse[1].ToInt() > EClass.rnd(1500) || (this.Chara.IsPowerful && !this.IsPCFaction) || EClass.debug.godFood;
		if (this.Chara.race.IsAnimal && EClass.rnd(EClass._zone.IsPCFaction ? 3 : 5) == 0)
		{
			flag2 = true;
		}
		if (origin != null && origin.HasElement(290, 1))
		{
			if (!flag2 && this.Chara.race.corpse[1].ToInt() > EClass.rnd(150000 / (100 + (int)Mathf.Sqrt((float)origin.Evalue(290)) * 5)))
			{
				flag2 = true;
				origin.elements.ModExp(290, 100, false);
			}
			else
			{
				origin.elements.ModExp(290, 5, false);
			}
		}
		if (flag2 && !isUserZone)
		{
			string a = this.Chara.race.corpse[0];
			if (a == "_meat" && EClass.rnd(10) == 0)
			{
				a = "meat_marble";
			}
			Thing thing3 = ThingGen.Create(a, -1, -1);
			if (thing3.source._origin == "meat")
			{
				thing3.MakeFoodFrom(this);
			}
			else
			{
				thing3.ChangeMaterial(this.Chara.race.material);
			}
			list.Add(thing3);
		}
		if (!this.IsPCFaction && this.<SpawnLoot>g__chance|738_0(200))
		{
			list.Add(this.Chara.MakeGene(null));
		}
		if (!this.IsPCFaction && !isUserZone)
		{
			foreach (string text in this.sourceCard.loot.Concat(this.Chara.race.loot).ToList<string>())
			{
				string[] array = text.Split('/', StringSplitOptions.None);
				int num = array[1].ToInt();
				if (num >= 1000 || num > EClass.rnd(1000) || EClass.debug.godMode)
				{
					list.Add(ThingGen.Create(array[0], -1, -1).SetNum((num >= 1000) ? (num / 1000 + ((EClass.rnd(1000) > num % 1000) ? 1 : 0)) : 1));
				}
			}
			if (race.IsMachine)
			{
				if (this.<SpawnLoot>g__chance|738_0(20))
				{
					list.Add(ThingGen.Create("microchip", -1, -1));
				}
				if (this.<SpawnLoot>g__chance|738_0(15))
				{
					list.Add(ThingGen.Create("battery", -1, -1));
				}
			}
			else
			{
				if (race.IsAnimal)
				{
					if (this.<SpawnLoot>g__chance|738_0(15))
					{
						list.Add(ThingGen.Create("fang", -1, -1));
					}
					if (this.<SpawnLoot>g__chance|738_0(10))
					{
						list.Add(ThingGen.Create("skin", -1, -1));
					}
				}
				if (this.<SpawnLoot>g__chance|738_0(20))
				{
					list.Add(ThingGen.Create("offal", -1, -1));
				}
				if (this.<SpawnLoot>g__chance|738_0(20))
				{
					list.Add(ThingGen.Create("heart", -1, -1));
				}
			}
			if (!this.isBackerContent && !flag)
			{
				int num2 = 1;
				List<Thing> list2 = new List<Thing>();
				foreach (Thing thing4 in this.things)
				{
					if (!thing4.HasTag(CTAG.gift) && !(thing4.trait is TraitChestMerchant))
					{
						if (thing4.isGifted || thing4.rarity >= Rarity.Artifact)
						{
							list.Add(thing4);
						}
						else if (thing4.IsEquipmentOrRanged)
						{
							if (thing4.rarity >= Rarity.Legendary)
							{
								list2.Add(thing4);
							}
							else if (EClass.rnd(200) == 0)
							{
								list.Add(thing4);
							}
						}
						else if (EClass.rnd(5) == 0)
						{
							list.Add(thing4);
						}
					}
				}
				if (num2 > 0 && list2.Count > 0)
				{
					list2.Shuffle<Thing>();
					int num3 = 0;
					while (num3 < list2.Count && num3 < num2)
					{
						list.Add(list2[num3]);
						num3++;
					}
				}
			}
			if (!this.isBackerContent && this.id != "big_sister")
			{
				foreach (Thing thing5 in this.things)
				{
					if (!thing5.HasTag(CTAG.gift) && !(thing5.trait is TraitChestMerchant))
					{
						if (thing5.isGifted || thing5.rarity >= Rarity.Legendary || thing5.trait.DropChance > EClass.rndf(1f))
						{
							list.Add(thing5);
						}
						else if (thing5.IsEquipmentOrRanged)
						{
							if (EClass.rnd(200) == 0)
							{
								list.Add(thing5);
							}
						}
						else if (EClass.rnd(5) == 0)
						{
							list.Add(thing5);
						}
					}
				}
			}
			int num4 = 0;
			foreach (Card card in list)
			{
				if (card.rarity >= Rarity.Legendary || card.IsContainer)
				{
					num4++;
				}
			}
			Rand.SetSeed(this.uid);
			if (num4 == 0 && !this.isBackerContent && !flag && this.rarity >= Rarity.Legendary && !this.IsUnique && this.c_bossType != BossType.Evolved)
			{
				if (EClass.rnd((EClass._zone.events.GetEvent<ZoneEventDefenseGame>() != null) ? 3 : 2) == 0)
				{
					Rarity rarity = (EClass.rnd(20) == 0) ? Rarity.Mythical : Rarity.Legendary;
					CardBlueprint.Set(new CardBlueprint
					{
						rarity = rarity
					});
					Thing item = ThingGen.CreateFromFilter("eq", this.LV);
					list.Add(item);
				}
				else if (EClass.rnd(3) == 0)
				{
					list.Add(ThingGen.Create("medal", -1, -1));
				}
			}
			Rand.SetSeed(-1);
		}
		foreach (Thing thing6 in this.things)
		{
			if (thing6.GetInt(116, null) != 0)
			{
				list.Add(thing6);
			}
		}
		Point nearestPoint = this.GetRootCard().pos;
		if (nearestPoint.IsBlocked)
		{
			nearestPoint = nearestPoint.GetNearestPoint(false, true, true, false);
		}
		foreach (Card card2 in list)
		{
			card2.isHidden = false;
			card2.SetInt(116, 0);
			EClass._zone.AddCard(card2, nearestPoint);
			if (card2.IsEquipmentOrRanged && card2.rarity >= Rarity.Superior && !card2.IsCursed)
			{
				foreach (Chara chara in EClass._map.charas)
				{
					if (chara.HasElement(1412, 1) && chara.Dist(nearestPoint) < 3)
					{
						card2.Thing.TryLickEnchant(chara, true, null, null);
						break;
					}
				}
			}
		}
	}

	public Thing TryMakeRandomItem(int lv = -1)
	{
		if (lv == -1)
		{
			lv = EClass._zone.DangerLv;
		}
		string text = this.id;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 2291038882U)
		{
			if (num <= 1062293841U)
			{
				if (num != 51003139U)
				{
					if (num != 1062293841U)
					{
						goto IL_2A6;
					}
					if (!(text == "log"))
					{
						goto IL_2A6;
					}
					this.ChangeMaterial((from m in EClass.sources.materials.rows
					where m.category == "wood"
					select m).RandomItem<SourceMaterial.Row>());
					goto IL_2A6;
				}
				else if (!(text == "egg_fertilized"))
				{
					goto IL_2A6;
				}
			}
			else if (num != 1555468929U)
			{
				if (num != 2291038882U)
				{
					goto IL_2A6;
				}
				if (!(text == "milk"))
				{
					goto IL_2A6;
				}
			}
			else if (!(text == "_meat"))
			{
				goto IL_2A6;
			}
		}
		else if (num <= 3595465691U)
		{
			if (num != 2585652531U)
			{
				if (num != 3595465691U)
				{
					goto IL_2A6;
				}
				if (!(text == "ore_gem"))
				{
					goto IL_2A6;
				}
				this.ChangeMaterial(MATERIAL.GetRandomMaterialFromCategory(lv, "gem", this.material));
				goto IL_2A6;
			}
			else
			{
				if (!(text == "ore"))
				{
					goto IL_2A6;
				}
				this.ChangeMaterial(MATERIAL.GetRandomMaterialFromCategory(lv, "ore", this.material));
				goto IL_2A6;
			}
		}
		else if (num != 3786258943U)
		{
			if (num != 3871660930U)
			{
				goto IL_2A6;
			}
			if (!(text == "meat_marble"))
			{
				goto IL_2A6;
			}
		}
		else if (!(text == "_egg"))
		{
			goto IL_2A6;
		}
		string text2 = "c_wilds";
		if (this.id == "_meat" || this.id == "meat_marble")
		{
			text2 = "c_animal";
		}
		int i = 0;
		while (i < 20)
		{
			CardRow cardRow = SpawnList.Get(text2, null, null).Select(lv, -1);
			if (!(cardRow.model.Chara.race.corpse[0] != "_meat") || !(this.id != "milk") || !(this.id != "_egg") || !(this.id != "egg_fertilized"))
			{
				if (!(this.id == "milk"))
				{
					this.MakeFoodFrom(cardRow.model);
					break;
				}
				if (this.c_idRefCard.IsEmpty())
				{
					this.MakeRefFrom(cardRow.model, null);
					break;
				}
				break;
			}
			else
			{
				i++;
			}
		}
		IL_2A6:
		return this as Thing;
	}

	public Card MakeFoodFrom(string _id)
	{
		return this.MakeFoodFrom(EClass.sources.cards.map[_id].model);
	}

	public Card MakeFoodFrom(Card c)
	{
		this.MakeRefFrom(c, null);
		this.ChangeMaterial(c.material);
		SourceRace.Row race = c.Chara.race;
		int num = race.food[0].ToInt();
		bool flag = this.id == "meat_marble";
		int num2 = 1;
		bool flag2 = this.category.IsChildOf("meat");
		bool flag3 = this.category.IsChildOf("egg");
		if (flag)
		{
			num += 100;
		}
		if (flag2)
		{
			this.elements.SetBase(70, race.STR * race.STR / 5 * num / 100 - 10 + num / 10, 0);
			if (flag)
			{
				this.elements.SetBase(440, race.END * race.END / 5 * num / 100 - 10 + num / 10, 0);
			}
			this.elements.SetBase(71, (int)Mathf.Clamp((float)(num / 10) + Mathf.Sqrt((float)race.height) - 10f, 1f, 60f), 0);
		}
		else if (flag3)
		{
			this.elements.SetBase(444, race.LER * race.LER / 5 * num / 100 - 10 + num / 10, 0);
			num2 = 2;
		}
		else
		{
			num2 = 3;
		}
		foreach (Element element in c.elements.dict.Values)
		{
			if ((!flag3 || element.id != 1229) && (element.source.category == "food" || element.source.tag.Contains("foodEnc") || element.IsTrait))
			{
				this.elements.SetBase(element.id, element.Value, 0);
			}
		}
		List<Tuple<int, int>> list = new List<Tuple<int, int>>();
		foreach (KeyValuePair<int, int> keyValuePair in race.elementMap)
		{
			if (EClass.sources.elements.map[keyValuePair.Key].tag.Contains("primary"))
			{
				list.Add(new Tuple<int, int>(keyValuePair.Key, keyValuePair.Value));
			}
		}
		list.Sort((Tuple<int, int> a, Tuple<int, int> b) => b.Item2 - a.Item2);
		int num3 = 0;
		while (num3 < num2 && num3 < list.Count)
		{
			Tuple<int, int> tuple = list[num3];
			this.elements.SetBase(tuple.Item1, tuple.Item2 * tuple.Item2 / 4, 0);
			num3++;
		}
		if (race.IsUndead)
		{
			this.elements.ModBase(73, -20);
		}
		this.isWeightChanged = true;
		this.c_weight = race.height * 4 + 100;
		this.c_idMainElement = c.c_idMainElement;
		this.SetBlessedState(BlessedState.Normal);
		int num4 = c.LV - c.sourceCard.LV;
		if (num4 < 0)
		{
			num4 = 0;
		}
		num4 = EClass.curve(num4, 10, 10, 80);
		if (c.rarity >= Rarity.Legendary || c.IsUnique)
		{
			num4 += 60;
		}
		if (num4 > 0)
		{
			this.elements.ModBase(2, num4);
		}
		return this;
	}

	public void MakeFoodRef(Card c1, Card c2 = null)
	{
		Card card = c1;
		Card card2 = c2;
		if (Card.<MakeFoodRef>g__IsIgnoreName|742_0(card))
		{
			card = null;
		}
		if (Card.<MakeFoodRef>g__IsIgnoreName|742_0(card2))
		{
			card2 = null;
		}
		if (card == null && card2 != null)
		{
			card = card2;
			card2 = null;
		}
		if (card != null)
		{
			this.MakeRefFrom(card, card2);
			if (card.c_idRefCard != null)
			{
				this.c_idRefCard = card.c_idRefCard;
				this.c_altName = this.TryGetFoodName(card);
			}
			if (card2 != null && card2.c_idRefCard != null)
			{
				this.c_idRefCard2 = card2.c_idRefCard;
				this.c_altName2 = this.TryGetFoodName(card2);
			}
		}
	}

	public string TryGetFoodName(Card c)
	{
		if (c.c_idRefCard.IsEmpty())
		{
			return c.c_altName;
		}
		SourceChara.Row row = c.refCard as SourceChara.Row;
		if (row == null || !row.isChara)
		{
			return c.c_altName;
		}
		if (!row.aka.IsEmpty())
		{
			if (row.name == "*r" && row.aka == "*r")
			{
				return "corpseGeneral".lang();
			}
			if (row.name == "*r")
			{
				return row.GetText("aka", false);
			}
		}
		return row.GetName();
	}

	public string GetFoodName(string s)
	{
		return s.Replace("_corpseFrom".lang(), "_corpseTo".lang());
	}

	public void MakeFigureFrom(string id)
	{
		this.MakeRefFrom(id);
	}

	public void MakeRefFrom(string id)
	{
		this.c_idRefCard = id;
	}

	public void MakeRefFrom(Card c1, Card c2 = null)
	{
		this.c_idRefCard = c1.id;
		this.c_altName = (c1.IsPC ? c1.c_altName : c1.GetName(NameStyle.Ref, c1.isChara ? 0 : 1));
		if (c2 != null)
		{
			this.c_idRefCard2 = c2.id;
			this.c_altName2 = (c2.IsPC ? c2.c_altName : c2.GetName(NameStyle.Ref, c2.isChara ? 0 : 1));
		}
	}

	public void SetHidden(bool hide = true)
	{
		this.isHidden = hide;
		this.pos.cell.Refresh();
	}

	public virtual Card.MoveResult _Move(Point p, Card.MoveType type = Card.MoveType.Walk)
	{
		EClass._map.MoveCard(p, this);
		if (this.isChara)
		{
			this.Chara.SyncRide();
		}
		return Card.MoveResult.Success;
	}

	public unsafe void MoveImmediate(Point p, bool focus = true, bool cancelAI = true)
	{
		if (p == null)
		{
			return;
		}
		EClass._map.MoveCard(p, this);
		if (!this.IsPC || focus)
		{
			this.renderer.SetFirst(true, *p.PositionCenter());
		}
		if (this.isChara)
		{
			if (cancelAI)
			{
				this.Chara.ai.Cancel();
			}
			this.Chara.SyncRide();
		}
		if (this.IsPC && focus)
		{
			EClass.screen.FocusPC();
			EClass.screen.RefreshPosition();
		}
	}

	public unsafe void Teleport(Point point, bool silent = false, bool force = false)
	{
		if (EClass._zone.IsRegion)
		{
			this.SayNothingHappans();
			return;
		}
		this.PlayEffect("teleport", true, 0f, default(Vector3));
		if (!force && (this.trait is TraitNewZone || this.elements.Has(400) || (this.isChara && this.Chara.HasCondition<ConGravity>())))
		{
			this.Say("antiTeleport", this, null, null);
			this.PlaySound("gravity", 1f, true);
			return;
		}
		if (!silent)
		{
			this.PlaySound("teleport", 1f, true);
			this.Say("teleported", this, null, null);
		}
		this._Move(point, Card.MoveType.Walk);
		this.renderer.SetFirst(true, *this.pos.PositionCenter());
		if (this.isChara)
		{
			this.Chara.ai.Cancel();
			foreach (Chara chara in EClass._map.charas)
			{
				if (chara.enemy == this)
				{
					chara.SetEnemy(null);
				}
			}
			this.Chara.RemoveCondition<ConEntangle>();
		}
		if (this.IsPC)
		{
			EClass.screen.FocusPC();
			EClass.screen.RefreshPosition();
			EClass.player.haltMove = true;
		}
		this.PlayEffect("teleport", false, 0f, default(Vector3));
	}

	public virtual void OnLand()
	{
		if (this.Cell.IsTopWaterAndNoSnow)
		{
			this.PlayEffect("ripple", true, 0f, default(Vector3));
			this.PlaySound("Footstep/water", 1f, true);
		}
	}

	public int ResistLvFrom(int ele)
	{
		return this.ResistLv(EClass.sources.elements.alias[EClass.sources.elements.map[ele].aliasRef].id);
	}

	public int ResistLv(int res)
	{
		return Element.GetResistLv(this.Evalue(res));
	}

	public bool HasElement(int ele, int req = 1)
	{
		return this.elements.Value(ele) >= req;
	}

	public bool HasElement(string id, int req = 1)
	{
		return this.HasElement(EClass.sources.elements.alias[id].id, req);
	}

	public virtual CardRenderer _CreateRenderer()
	{
		this.renderer = new CardRenderer();
		this.renderer.SetOwner(this);
		return this.renderer;
	}

	public void AddBlood(int a = 1, int id = -1)
	{
		if (EClass._zone.IsRegion)
		{
			return;
		}
		for (int i = 0; i < a; i++)
		{
			EClass._map.AddDecal(this.pos.x + ((EClass.rnd(2) == 0) ? 0 : (EClass.rnd(3) - 1)), this.pos.z + ((EClass.rnd(2) == 0) ? 0 : (EClass.rnd(3) - 1)), (id == -1) ? (this.isChara ? this.Chara.race.blood : this.material.decal) : id, 1, true);
		}
		this.PlaySound("blood", 1f, true);
	}

	public RenderParam GetRenderParam()
	{
		RenderParam shared = RenderParam.shared;
		shared.color = 11010048f;
		shared.liquidLv = 0;
		shared.cell = null;
		this.SetRenderParam(shared);
		return shared;
	}

	public virtual void SetRenderParam(RenderParam p)
	{
	}

	public void DyeRandom()
	{
		this.Dye(EClass.sources.materials.rows.RandomItem<SourceMaterial.Row>().alias);
	}

	public void Dye(string idMat)
	{
		this.Dye(EClass.sources.materials.alias[idMat]);
	}

	public void Dye(SourceMaterial.Row mat)
	{
		this.isDyed = true;
		this.c_dyeMat = mat.id;
		this._colorInt = 0;
	}

	public void RefreshColor()
	{
		if (this.isChara)
		{
			if (this.isDyed)
			{
				this._colorInt = BaseTileMap.GetColorInt(ref this.DyeMat.matColor, this.sourceRenderCard.colorMod);
				return;
			}
			if (this.isElemental)
			{
				this._colorInt = BaseTileMap.GetColorInt(ref EClass.setting.elements[this.Chara.MainElement.source.alias].colorSprite, this.sourceRenderCard.colorMod);
				return;
			}
			this._colorInt = 104025;
			return;
		}
		else
		{
			if (this.isDyed)
			{
				this._colorInt = BaseTileMap.GetColorInt(ref this.DyeMat.matColor, this.sourceRenderCard.colorMod);
				return;
			}
			if (this.sourceRenderCard.useRandomColor)
			{
				this._colorInt = BaseTileMap.GetColorInt(this.GetRandomColor(), this.sourceRenderCard.colorMod);
				return;
			}
			if (this.sourceRenderCard.useAltColor)
			{
				this._colorInt = BaseTileMap.GetColorInt(ref this.material.altColor, this.sourceRenderCard.colorMod);
				return;
			}
			this._colorInt = BaseTileMap.GetColorInt(ref this.material.matColor, this.sourceRenderCard.colorMod);
			return;
		}
	}

	public ref Color GetRandomColor()
	{
		int num = EClass.game.seed + this.refVal;
		num += (int)(this.id[0] % '✐');
		if (this.id.Length > 1)
		{
			num += (int)(this.id[1] % 'Ϩ');
			if (this.id.Length > 2)
			{
				num += (int)(this.id[2] % 'Ϩ');
				if (this.id.Length > 3)
				{
					num += (int)(this.id[3] % 'Ϩ');
					if (this.id.Length > 4)
					{
						num += (int)(this.id[4] % 'Ϩ');
					}
				}
			}
		}
		Rand.UseSeed(num, delegate
		{
			Card._randColor = EClass.sources.materials.rows[EClass.rnd(90)].matColor;
		});
		return ref Card._randColor;
	}

	public virtual Sprite GetSprite(int dir = 0)
	{
		if (this.trait is TraitAbility)
		{
			Act act = (this.trait as TraitAbility).CreateAct();
			return ((act != null) ? act.GetSprite() : null) ?? EClass.core.refs.icons.defaultAbility;
		}
		return this.sourceCard.GetSprite(dir, this.idSkin, this.IsInstalled && this.pos.cell.IsSnowTile);
	}

	public virtual Sprite GetImageSprite()
	{
		return null;
	}

	public void SetImage(Image image, int dir, int idSkin = 0)
	{
		this.sourceRenderCard.SetImage(image, this.GetSprite(dir), this.colorInt, true, dir, idSkin);
	}

	public virtual void SetImage(Image image)
	{
		if (this.trait is TraitAbility)
		{
			(this.trait as TraitAbility).act.SetImage(image);
			return;
		}
		this.sourceRenderCard.SetImage(image, this.GetSprite(0), this.colorInt, true, 0, 0);
	}

	public void ShowEmo(Emo _emo = Emo.none, float duration = 0f, bool skipSame = true)
	{
		if (this.isChara && this.Chara.host != null)
		{
			return;
		}
		if (_emo == this.lastEmo && skipSame)
		{
			return;
		}
		if (_emo != Emo.none)
		{
			this.renderer.ShowEmo(_emo, duration);
		}
		this.lastEmo = _emo;
	}

	public Point ThisOrParentPos
	{
		get
		{
			if (!(this.parent is Card))
			{
				return this.pos;
			}
			return (this.parent as Card).pos;
		}
	}

	public void PlaySoundHold(bool spatial = true)
	{
		this.PlaySound(this.material.GetSoundDrop(this.sourceCard), 1f, spatial);
	}

	public void PlaySoundDrop(bool spatial = true)
	{
		this.PlaySound(this.material.GetSoundDrop(this.sourceCard), 1f, spatial);
	}

	public SoundSource PlaySound(string id, float v = 1f, bool spatial = true)
	{
		if (this.IsPC)
		{
			spatial = false;
		}
		if (this.Dist(EClass.pc) < EClass.player.lightRadius + 1 || !spatial)
		{
			return this.ThisOrParentPos.PlaySound(id, this.isSynced || !spatial, v, spatial);
		}
		return null;
	}

	public void KillAnime()
	{
		this.renderer.KillAnime();
	}

	public void PlayAnime(AnimeID id, bool force = false)
	{
		this.renderer.PlayAnime(id, force);
	}

	public void PlayAnime(AnimeID id, Point dest, bool force = false)
	{
		this.renderer.PlayAnime(id, dest);
	}

	public void PlayAnimeLoot()
	{
		this.renderer.PlayAnime(AnimeID.Loot, default(Vector3), false);
	}

	public unsafe Effect PlayEffect(string id, bool useRenderPos = true, float range = 0f, Vector3 fix = default(Vector3))
	{
		if (id.IsEmpty())
		{
			return null;
		}
		return Effect.Get(id)._Play(this.pos, fix + ((this.isSynced && useRenderPos) ? this.renderer.position : (*this.pos.Position())) + new Vector3(Rand.Range(-range, range), Rand.Range(-range, range), 0f), 0f, null, null);
	}

	public unsafe void PlayEffect(int ele, bool useRenderPos = true, float range = 0f)
	{
		Effect effect = Effect.Get("Element/" + EClass.sources.elements.map[ele].alias);
		if (effect == null)
		{
			Debug.Log(ele);
			return;
		}
		effect._Play(this.pos, ((this.isSynced && useRenderPos) ? this.renderer.position : (*this.pos.Position())) + new Vector3(Rand.Range(-range, range), Rand.Range(-range, range), 0f), 0f, null, null);
	}

	public virtual void SetDir(int d)
	{
		this.dir = d;
		this.renderer.RefreshSprite();
	}

	public void SetRandomDir()
	{
		this.SetDir(EClass.rnd(4));
	}

	public virtual void LookAt(Card c)
	{
	}

	public virtual void LookAt(Point p)
	{
	}

	public virtual void Rotate(bool reverse = false)
	{
		int num = 4;
		if (this.sourceCard.tiles.Length > 4)
		{
			num = this.sourceCard.tiles.Length;
		}
		if (this.TileType == TileType.Door)
		{
			num = 2;
		}
		if (reverse)
		{
			int dir = this.dir;
			this.dir = dir - 1;
		}
		else
		{
			int dir = this.dir;
			this.dir = dir + 1;
		}
		if (this.dir < 0)
		{
			this.dir = num - 1;
		}
		if (this.dir == num)
		{
			this.dir = 0;
		}
		this.SetDir(this.dir);
		this.renderer.RefreshSprite();
	}

	public void ChangeAltitude(int a)
	{
		this.altitude += a;
		if (this.altitude < 0)
		{
			this.altitude = 0;
		}
		if (this.altitude > this.TileType.MaxAltitude)
		{
			this.altitude = this.TileType.MaxAltitude;
		}
	}

	public virtual SubPassData GetSubPassData()
	{
		return SubPassData.Default;
	}

	public unsafe void SetFreePos(Point point)
	{
		this.freePos = (EClass.game.config.FreePos && this.isThing && this.TileType.FreeStyle && !this.sourceCard.multisize && !EClass.scene.actionMode.IsRoofEditMode(this));
		if (this.freePos)
		{
			Vector3 vector = *point.Position();
			Vector3 thingPosition = EClass.screen.tileMap.GetThingPosition(this, point);
			this.fx = EInput.mposWorld.x + EClass.setting.render.freePosFix.x;
			this.fy = EInput.mposWorld.y + EClass.setting.render.freePosFix.y;
			if (EClass.game.config.snapFreePos)
			{
				this.fx -= this.fx % 0.2f;
				this.fy -= this.fy % 0.1f;
			}
			this.fx = this.fx - vector.x + thingPosition.x;
			this.fy = this.fy - vector.y + thingPosition.y;
			return;
		}
		this.fx = (this.fy = 0f);
	}

	public unsafe void RenderMarker(Point point, bool active, HitResult result, bool main, int dir, bool useCurrentPosition = false)
	{
		if (dir != -1)
		{
			this.dir = dir;
		}
		Vector3 vector = *point.Position();
		bool skipRender = point.cell.skipRender;
		if (result != HitResult.Default && EClass.screen.guide.isActive && !skipRender)
		{
			EClass.screen.guide.passGuideBlock.Add(ref vector, (float)((point.HasObj || point.HasChara) ? 5 : 0), 0f);
		}
		if (!main)
		{
			return;
		}
		RenderParam renderParam = this.GetRenderParam();
		if (EClass.scene.actionMode.IsRoofEditMode(this))
		{
			renderParam.x = vector.x;
			renderParam.y = vector.y;
			renderParam.z = vector.z;
			EClass.screen.tileMap.SetRoofHeight(renderParam, point.cell, point.x, point.z, 0, 0, -1, false);
			vector.x = renderParam.x;
			vector.y = renderParam.y;
			vector.z = renderParam.z;
		}
		if (this.TileType.UseMountHeight && !EClass.scene.actionMode.IsRoofEditMode(this))
		{
			this.TileType.GetMountHeight(ref vector, point, this.dir, this);
		}
		vector.z += EClass.setting.render.thingZ;
		if (!skipRender)
		{
			Vector3 thingPosition = EClass.screen.tileMap.GetThingPosition(this, point);
			if (this.freePos)
			{
				vector.x += this.fx;
				vector.y += this.fy;
				vector.z += thingPosition.z;
			}
			else
			{
				vector += thingPosition;
			}
		}
		if (useCurrentPosition)
		{
			vector = this.renderer.position;
			vector.z += -0.01f;
		}
		if (this.TileType == TileType.Door)
		{
			vector.z -= 0.5f;
		}
		renderParam.matColor = (float)(active ? EClass.Colors.blockColors.Active : EClass.Colors.blockColors.Inactive);
		point.ApplyAnime(ref vector);
		if (this.renderer.hasActor)
		{
			this.renderer.actor.RefreshSprite();
		}
		this.renderer.Draw(renderParam, ref vector, false);
	}

	public void RecalculateFOV()
	{
		if (this.fov != null)
		{
			this.ClearFOV();
			this.fov = null;
			if (this.IsPC)
			{
				EClass.player.lightRadius = 1;
			}
		}
		this.CalculateFOV();
	}

	public bool HasLight()
	{
		return this.GetLightRadius() > 0;
	}

	public LightData LightData
	{
		get
		{
			if (this._LightData == null)
			{
				return this._LightData = ((!this.sourceCard.lightData.IsEmpty()) ? EClass.Colors.lightColors[this.sourceCard.lightData] : null);
			}
			return this._LightData;
		}
	}

	public float GetLightPower()
	{
		float num = this.isChara ? EClass.scene.profile.light.fovCurveChara.Evaluate(EClass.scene.timeRatio) : EClass.scene.profile.global.fovPower;
		if (this.LightData != null)
		{
			return 0.01f * this.LightData.color.a * 256f * 1.12f;
		}
		if (this.IsPCFaction && !this.IsPC)
		{
			num *= 4f;
		}
		return num;
	}

	public int GetSightRadius()
	{
		if (this.IsPC)
		{
			return EClass.player.lightRadius;
		}
		return (EClass._map.IsIndoor ? 4 : 5) + (this.IsPCFaction ? 1 : 0);
	}

	public int GetLightRadius()
	{
		if (!this.isThing)
		{
			int num = (this.LightData != null) ? this.LightData.radius : 0;
			int num2 = 0;
			if (this.IsPC)
			{
				if (this.Chara.isBlind)
				{
					return 1;
				}
				num = ((EClass._map.IsIndoor || EClass.world.date.IsNight) ? 2 : ((EClass.world.date.periodOfDay == PeriodOfDay.Day) ? 6 : 5));
				num2 = 2;
			}
			else
			{
				if (!EClass.core.config.graphic.drawAllyLight)
				{
					return 0;
				}
				if (this.LightData == null && !EClass._map.IsIndoor && !EClass.world.date.IsNight)
				{
					return 0;
				}
			}
			if (this.IsPCFaction)
			{
				Thing equippedThing = this.Chara.body.GetEquippedThing(45);
				if (equippedThing != null)
				{
					num2 = (equippedThing.trait as TraitLightSource).LightRadius;
				}
				if (this.Chara.held != null && this.IsPC)
				{
					int lightRadius = this.Chara.held.GetLightRadius();
					if (lightRadius > 0)
					{
						if (lightRadius > num2)
						{
							num2 = this.Chara.held.GetLightRadius() - 1;
						}
						if (num2 < 3)
						{
							num2 = 3;
						}
					}
				}
				if (num < num2)
				{
					num = num2;
				}
			}
			return num;
		}
		if (!this.IsInstalled && EClass.pc.held != this)
		{
			return 0;
		}
		if (this.trait is TraitLightSource && this.Thing.isEquipped)
		{
			return (this.trait as TraitLightSource).LightRadius;
		}
		if (this.LightData == null || !this.trait.IsLightOn)
		{
			return 0;
		}
		return this.LightData.radius;
	}

	public void CalculateFOV()
	{
		int lightRadius = this.GetLightRadius();
		if (lightRadius == 0 || !this.IsAliveInCurrentZone || !EClass._zone.isStarted)
		{
			return;
		}
		float num = this.GetLightPower();
		if (this.IsPC)
		{
			if (this.Chara.held != null && this.Chara.held.GetLightRadius() > 0)
			{
				num += this.Chara.held.GetLightPower();
			}
			if (lightRadius <= 2)
			{
				num = 0f;
			}
			foreach (Condition condition in this.Chara.conditions)
			{
				condition.OnCalculateFov(this.fov, ref lightRadius, ref num);
			}
			if (num > EClass.scene.profile.global.playerLightPowerLimit)
			{
				num = EClass.scene.profile.global.playerLightPowerLimit;
			}
			num *= EClass.scene.profile.light.playerLightMod;
			EClass.player.lightRadius = lightRadius;
			EClass.player.lightPower = num;
		}
		if (this.fov == null)
		{
			this.fov = this.CreateFov();
		}
		this.fov.Perform(this.pos.x, this.pos.z, lightRadius, num * 2f);
	}

	public void SetRandomLightColors()
	{
		this.c_lightColor = (int)((byte)(EClass.rnd(8) + 1)) * 1024 + (int)((byte)(EClass.rnd(8) + 1) * 32) + (int)((byte)(EClass.rnd(8) + 1));
	}

	public Fov CreateFov()
	{
		Fov fov = new Fov();
		int num = this.trait.UseLightColor ? this.c_lightColor : 0;
		if (num != 0)
		{
			fov.r = (byte)(num / 1024);
			fov.g = (byte)(num % 1024 / 32);
			fov.b = (byte)(num % 32);
		}
		else if (this.LightData != null)
		{
			fov.r = (byte)(this.LightData.color.r * 16f);
			fov.g = (byte)(this.LightData.color.g * 16f);
			fov.b = (byte)(this.LightData.color.b * 16f);
		}
		else if (this.isChara)
		{
			fov.r = 0;
			fov.g = 0;
			fov.b = 0;
		}
		else
		{
			fov.r = 3;
			fov.g = 2;
			fov.b = 1;
		}
		if (this.isChara && this.Chara.held != null && this.Chara.held.GetLightRadius() > 0)
		{
			Fov fov2 = this.Chara.held.CreateFov();
			Fov fov3 = fov;
			fov3.r += fov2.r;
			Fov fov4 = fov;
			fov4.g += fov2.g;
			Fov fov5 = fov;
			fov5.b += fov2.b;
		}
		if (this.IsPC)
		{
			fov.isPC = true;
			foreach (Condition condition in this.Chara.conditions)
			{
				condition.OnCreateFov(fov);
			}
		}
		fov.limitGradient = (this.IsPCParty && EClass.scene.profile.global.limitGradient);
		return fov;
	}

	public void ClearFOV()
	{
		if (this.fov == null || this.fov.lastPoints.Count == 0)
		{
			return;
		}
		this.fov.Perform(this.pos.x, this.pos.z, 0, 1f);
	}

	public virtual void OnSimulateHour(VirtualDate date)
	{
		this.trait.OnSimulateHour(date);
		if (date.IsRealTime)
		{
			this.DecayNatural(1);
		}
	}

	public void DecayNatural(int hour = 1)
	{
		if (this.isNPCProperty)
		{
			return;
		}
		this.things.ForeachReverse(delegate(Thing t)
		{
			t.DecayNatural(hour);
		});
		if (this.parent is Card && (this.parent as Card).trait.CanChildDecay(this))
		{
			this.Decay(10 * hour);
			return;
		}
		if (this.isChara || this.trait.Decay == 0)
		{
			return;
		}
		this.Decay(this.trait.Decay * hour);
	}

	public void Decay(int a = 10)
	{
		Card card = this.parent as Card;
		int num = 200;
		int num2 = this.MaxDecay / 4 * 3;
		if (a > 0)
		{
			if (card != null)
			{
				num = card.trait.DecaySpeedChild;
			}
			num = num * this.trait.DecaySpeed / 100;
			int num3 = this.Evalue(405);
			if (num3 != 0)
			{
				num = num * (100 - num3 * 2) / 100;
			}
			if (num < 0)
			{
				num = 0;
			}
		}
		else
		{
			num = 100;
		}
		a = a * num / 100;
		if (this.decay + a > this.MaxDecay)
		{
			if (card != null && !card.trait.OnChildDecay(this, !this.IsDecayed))
			{
				return;
			}
			if (!this.IsDecayed)
			{
				if (EClass.pc.HasElement(1325, 1) && this.GetRootCard() is Chara && this.category.IsChildOf("food"))
				{
					Thing thing = TraitSeed.MakeRandomSeed(true).SetNum(Mathf.Min(this.Num, 3));
					card.AddCard(thing);
					if (!this.<Decay>g__IsParentLocked|809_0())
					{
						this.GetRootCard().Say("seed_rot", this.GetRootCard(), this, thing.Name, null);
					}
					this.Destroy();
					return;
				}
				if (this.parent == EClass._zone)
				{
					this.Say("rot", this, null, null);
				}
				else if (this.GetRootCard() == EClass.pc)
				{
					if (!this.<Decay>g__IsParentLocked|809_0())
					{
						EClass.pc.Say("rotInv", this, EClass.pc, null, null);
					}
					LayerInventory.SetDirty(this.Thing);
				}
				if (this.IsFood)
				{
					this.elements.SetBase(73, -10, 0);
				}
			}
		}
		else if (this.decay < num2 && this.decay + a >= num2 && this.GetRootCard() == EClass.pc)
		{
			if (!this.<Decay>g__IsParentLocked|809_0())
			{
				EClass.pc.Say("rottingInv", this, EClass.pc, null, null);
			}
			LayerInventory.SetDirty(this.Thing);
		}
		this.decay += a;
	}

	public void Talk(string idTopic, string ref1 = null, string ref2 = null, bool forceSync = false)
	{
		if (this.IsPC && !EClass.player.forceTalk && idTopic != "goodBoy" && idTopic != "insane")
		{
			EClass.player.forceTalk = false;
			Msg.SetColor();
			return;
		}
		EClass.player.forceTalk = false;
		if (!this.isSynced && !forceSync)
		{
			Msg.SetColor();
			return;
		}
		GameLang.refDrama1 = ref1;
		GameLang.refDrama2 = ref2;
		string talkText = this.GetTalkText(idTopic, true, true);
		string a = this.id;
		if (!(a == "adv_gaki"))
		{
			if (a == "corgon")
			{
				BackerContent.GakiConvert(ref talkText, "mokyu");
			}
		}
		else
		{
			BackerContent.GakiConvert(ref talkText, "zako");
		}
		this.TalkRaw(talkText, ref1, ref2, forceSync);
	}

	public void TalkRaw(string text, string ref1 = null, string ref2 = null, bool forceSync = false)
	{
		if ((!this.isSynced && !forceSync) || text.IsEmpty())
		{
			Msg.SetColor();
			return;
		}
		if (ref1 != null)
		{
			text = text.Replace("#1", ref1);
		}
		if (ref2 != null)
		{
			text = text.Replace("#2", ref2);
		}
		this.HostRenderer.Say(this.ApplyNewLine(text), default(Color), 0f);
		bool flag = text.StartsWith("*");
		Msg.SetColor(text.StartsWith("(") ? Msg.colors.Thinking : (flag ? Msg.colors.Ono : Msg.colors.Talk));
		if (!flag)
		{
			text = text.Bracket(0);
		}
		Msg.Say(text.Replace("&", ""));
	}

	public string ApplyNewLine(string text)
	{
		if (text.Contains("&"))
		{
			string str = "_comma".lang();
			text = text.Replace(str + " &", Environment.NewLine ?? "");
			text = text.Replace(str + "&", Environment.NewLine ?? "");
			text = text.Replace("&", Environment.NewLine ?? "");
		}
		return text;
	}

	public CardRenderer HostRenderer
	{
		get
		{
			if (!this.isChara || this.Chara.host == null)
			{
				return this.renderer;
			}
			return this.Chara.host.renderer;
		}
	}

	public void SayRaw(string text, string ref1 = null, string ref2 = null)
	{
		if (!this.isSynced)
		{
			return;
		}
		if (text.IsEmpty())
		{
			return;
		}
		if (ref1 != null)
		{
			text = text.Replace("#1", ref1);
		}
		if (ref2 != null)
		{
			text = text.Replace("#2", ref2);
		}
		this.HostRenderer.Say(text, default(Color), 0f);
	}

	public bool ShouldShowMsg
	{
		get
		{
			return (this.IsPC || this.parent == EClass.pc || this.isSynced) && (!this.isChara || !this.Chara.isDead);
		}
	}

	public void SayNothingHappans()
	{
		this.Say("nothingHappens", null, null);
	}

	public void Say(string lang, string ref1 = null, string ref2 = null)
	{
		if (this.ShouldShowMsg)
		{
			Msg.Say(this.IsPC ? Lang.Game.TryGetId(lang + "_pc", lang) : lang, ref1, ref2, null, null);
		}
		Msg.SetColor();
	}

	public void Say(string lang, Card c1, Card c2, string ref1 = null, string ref2 = null)
	{
		if (this.ShouldShowMsg)
		{
			Msg.Say(this.IsPC ? Lang.Game.TryGetId(lang + "_pc", lang) : lang, c1, c2, ref1, ref2);
		}
		Msg.SetColor();
	}

	public void Say(string lang, Card c1, string ref1 = null, string ref2 = null)
	{
		if (this.ShouldShowMsg)
		{
			Msg.Say(this.IsPC ? Lang.Game.TryGetId(lang + "_pc", lang) : lang, c1, ref1, ref2, null);
		}
		Msg.SetColor();
	}

	public string GetTalkText(string idTopic, bool stripPun = false, bool useDefault = true)
	{
		bool flag = this.isChara && this.Chara.IsHuman;
		string text = MOD.listTalk.GetTalk(this.c_idTalk.IsEmpty(this.id), idTopic, useDefault, flag);
		if (!text.IsEmpty())
		{
			text = text.Split('|', StringSplitOptions.None).RandomItem<string>();
			if (!flag || (this.IsDeadOrSleeping && this.IsAliveInCurrentZone))
			{
				if (!text.StartsWith("(") && !text.StartsWith("*"))
				{
					text = "(" + text + ")";
				}
				text = text.Replace("。)", ")");
			}
		}
		return this.ApplyTone(text, stripPun);
	}

	public string ApplyTone(string text, bool stripPun = false)
	{
		text = GameLang.ConvertDrama(text, this.Chara);
		Chara chara = this.Chara;
		string c_idTone = this.c_idTone;
		Biography biography = this.bio;
		return Card.ApplyTone(chara, ref text, c_idTone, (biography != null) ? biography.gender : 0, stripPun);
	}

	public static string ApplyTone(Chara c, ref string text, string _tones, int gender, bool stripPun = false)
	{
		if (text.IsEmpty())
		{
			return text;
		}
		string[] array = _tones.IsEmpty("").Split('|', StringSplitOptions.None);
		string key = array[0];
		MOD.tones.Initialize();
		string text2;
		if (!Lang.setting.useTone || MOD.tones.list.Count == 0)
		{
			text2 = text.Replace("{", "").Replace("}", "");
		}
		else
		{
			if (array[0].IsEmpty())
			{
				key = "default";
			}
			if (MOD.tones.all.ContainsKey(key))
			{
				StringBuilder stringBuilder = MOD.tones.ApplyTone(key, ref text, gender);
				if (Lang.isJP && c != null && c.trait.EnableTone)
				{
					if (array.Length >= 2)
					{
						stringBuilder.Replace("_toneI".lang(), array[1]);
					}
					if (array.Length >= 3)
					{
						stringBuilder.Replace("_toneYou".lang(), array[2]);
					}
				}
				text2 = stringBuilder.ToString();
			}
			else
			{
				text2 = text.Replace("{", "").Replace("}", "");
			}
		}
		if (!stripPun || !Lang.setting.stripPuns)
		{
			return text2;
		}
		return text2.StripLastPun();
	}

	public void SetRandomTalk()
	{
		MOD.listTalk.Initialize();
		if (!MOD.listTalk.list[0].ContainsKey(this.id))
		{
			this.c_idTalk = MOD.listTalk.GetRandomID("human");
		}
	}

	public void SetRandomTone()
	{
		MOD.tones.Initialize();
		List<Dictionary<string, string>> list = MOD.tones.list;
		if (list.Count == 0)
		{
			return;
		}
		string text = list.RandomItem<Dictionary<string, string>>()["id"];
		for (int i = 0; i < 10; i++)
		{
			Dictionary<string, string> dictionary = list.RandomItem<Dictionary<string, string>>();
			if (EClass.rnd(100) <= dictionary["chance"].ToInt())
			{
				text = dictionary["id"];
				break;
			}
		}
		ToneDataList tones = MOD.tones;
		string text2 = text;
		Biography biography = this.bio;
		this.c_idTone = tones.GetToneID(text2, (biography != null) ? biography.gender : 0);
	}

	public void TryStack(Thing t)
	{
		if (t == this)
		{
			return;
		}
		ThingContainer.DestData dest = this.things.GetDest(t, true);
		if (dest.stack != null)
		{
			if (this.IsPC)
			{
				this.Say("stack_thing", t, dest.stack, null, null);
			}
			t.TryStackTo(dest.stack);
		}
	}

	public void ApplyBacker(int bid)
	{
		this.ChangeRarity(Rarity.Normal);
		SourceBacker.Row row = EClass.sources.backers.map.TryGetValue(bid, null);
		if (row == null)
		{
			return;
		}
		this.c_idBacker = row.id;
		if (row.type == 4)
		{
			this.Chara.bio.SetGender(row.gender);
			this.Chara.hostility = (this.Chara.c_originalHostility = Hostility.Neutral);
		}
		if (row.type == 6)
		{
			this.Chara.bio.SetGender(row.gender);
			this.Chara.bio.SetPortrait(this.Chara);
			this.Chara.idFaith = row.deity.ToLower();
		}
		if (row.type == 4 || row.type == 5 || row.type == 7)
		{
			this.idSkin = ((row.skin == 0) ? EClass.rnd(this.sourceCard._tiles.Length) : row.skin);
			if (this.id == "putty_snow")
			{
				this.idSkin = 0;
			}
		}
		if (bid == 164)
		{
			this.Chara.EQ_ID("amulet_moonnight", -1, Rarity.Random);
		}
	}

	public void RemoveBacker()
	{
		if (this.c_idBacker == 164)
		{
			Thing thing = this.Chara.things.Find("amulet_moonnight", -1, -1);
			if (thing != null)
			{
				thing.Destroy();
			}
		}
		this.c_idBacker = 0;
	}

	public void SetPaintData()
	{
		EClass.ui.Hide(0f);
		EClass.core.WaitForEndOfFrame(delegate
		{
			this.ClearPaintSprite();
			this.c_textureData = this.GetPaintData();
			EClass.core.WaitForEndOfFrame(delegate
			{
				EClass.ui.Show(0f);
			});
		});
	}

	public byte[] GetPaintData()
	{
		Sprite sprite = this.GetSprite(0);
		Texture2D texture2D = ScreenCapture.CaptureScreenshotAsTexture();
		int num = sprite.texture.width * 2;
		int num2 = sprite.texture.height * 2;
		int x = (int)Mathf.Clamp(Input.mousePosition.x - (float)(num / 2), 1f, (float)(texture2D.width - num - 1));
		int y = (int)Mathf.Clamp(Input.mousePosition.y - (float)(num2 / 2), 1f, (float)(texture2D.height - num2 - 1));
		Color[] pixels = texture2D.GetPixels(x, y, num, num2);
		Texture2D texture2D2 = new Texture2D(num, num2, TextureFormat.ARGB32, false);
		texture2D2.SetPixels(pixels);
		texture2D2.Apply();
		byte[] result = texture2D2.EncodeToJPG();
		UnityEngine.Object.Destroy(texture2D);
		UnityEngine.Object.Destroy(texture2D2);
		return result;
	}

	public void ClearPaintSprite()
	{
		if (this._paintSprite)
		{
			UnityEngine.Object.Destroy(this._paintSprite.texture);
			UnityEngine.Object.Destroy(this._paintSprite);
			this._paintSprite = null;
		}
	}

	public Sprite GetPaintSprite()
	{
		if (!this._paintSprite)
		{
			byte[] c_textureData = this.c_textureData;
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.LoadImage(c_textureData);
			this._paintSprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f), 200f);
			EClass.game.loadedTextures.Add(texture2D);
			TraitCanvas traitCanvas = this.trait as TraitCanvas;
			if (traitCanvas != null)
			{
				texture2D.filterMode = (traitCanvas.PointFilter ? FilterMode.Point : FilterMode.Bilinear);
			}
		}
		return this._paintSprite;
	}

	public void TryUnrestrain(bool force = false, Chara c = null)
	{
		if (this.GetRestrainer() == null || force)
		{
			this.isRestrained = false;
			this.Say("unrestrained", this, null, null);
			if (this != c)
			{
				this.Talk("thanks", null, null, false);
			}
			this.MoveImmediate(this.pos.GetNearestPoint(false, true, true, false), true, true);
			this.renderer.SetFirst(true);
			if (c != null)
			{
				if (this.c_rescueState == RescueState.WaitingForRescue)
				{
					this.c_rescueState = RescueState.Rescued;
					if (c.IsPC)
					{
						EClass.player.ModKarma(2);
					}
				}
				foreach (Chara tg in c.pos.ListCharasInRadius(c, 5, (Chara _c) => _c.id == "fanatic" && _c.faith != this.Chara.faith))
				{
					c.DoHostileAction(tg, true);
				}
			}
		}
	}

	public TraitShackle GetRestrainer()
	{
		foreach (Card card in this.pos.ListCards(false))
		{
			if (card.trait is TraitShackle && card.c_uidRefCard == this.uid)
			{
				return card.trait as TraitShackle;
			}
		}
		return null;
	}

	public virtual void Tick()
	{
	}

	public static int GetTilePrice(TileRow row, SourceMaterial.Row mat)
	{
		int num = 0;
		if (row.id == 0)
		{
			return num;
		}
		num = row.value * mat.value / 100;
		if (num < 0)
		{
			num = 1;
		}
		return num;
	}

	public Thing SetPriceFix(int a)
	{
		this.c_priceFix = a;
		return this.Thing;
	}

	public int GetEquipValue()
	{
		return this.GetValue(false);
	}

	public void SetSale(bool sale)
	{
		if (this.isSale == sale)
		{
			return;
		}
		this.isSale = sale;
		if (this.isSale)
		{
			EClass._map.props.sales.Add(this);
			return;
		}
		EClass._map.props.sales.Remove(this);
	}

	public int GetValue(bool sell = false)
	{
		int value = this.trait.GetValue();
		if (value == 0)
		{
			return 0;
		}
		float num = (float)value;
		num = num * (float)Mathf.Max(100 + this.rarityLv + Mathf.Min(this.QualityLv * 10, 200), 80) / 100f;
		if (this.IsFood && !this.material.tag.Contains("food"))
		{
			num *= 0.5f;
		}
		float num2;
		if (this.IsEquipmentOrRanged || this.trait is TraitMod)
		{
			if (sell)
			{
				num *= 0.3f;
			}
			num2 = 2f;
		}
		else
		{
			num2 = 0.5f;
		}
		if (this.isReplica)
		{
			num *= 0.15f;
		}
		if (!this.IsUnique)
		{
			if (this.IsEquipmentOrRanged && this.rarity >= Rarity.Legendary)
			{
				num = Mathf.Max(num, 1800f + num / 5f);
			}
			num = num * (100f + num2 * (float)(this.material.value - 100)) / 100f;
			if (this.IsEquipmentOrRanged)
			{
				int num3 = 0;
				foreach (Element element in this.elements.dict.Values)
				{
					num3 += element.source.value;
				}
				num = num * (float)(100 + (sell ? ((int)MathF.Sqrt((float)num3) * 10) : num3)) / 100f;
				if (this.rarity >= Rarity.Legendary)
				{
					num = Mathf.Max(num, 3600f + num / 5f);
				}
			}
		}
		if (this.trait is TraitRecipe && sell)
		{
			num *= 0.1f;
		}
		if (this.encLV != 0)
		{
			if (this.category.tag.Contains("enc"))
			{
				num *= 0.7f + (float)(this.encLV - 1) * 0.2f;
			}
			else if (this.IsFood)
			{
				if (this.id == "honey")
				{
					num += (float)(this.encLV * 10);
				}
				else
				{
					num = num * Mathf.Min(1f + 0.1f * (float)this.encLV, 2f) + (float)(this.encLV * 100);
				}
			}
			else
			{
				num *= 1f + (float)this.encLV * 0.01f;
			}
		}
		return (int)num;
	}

	public virtual int GetPrice(CurrencyType currency = CurrencyType.Money, bool sell = false, PriceType priceType = PriceType.Default, Chara c = null)
	{
		if (!sell)
		{
			if (this.id == "littleball")
			{
				return 0;
			}
			if (currency != CurrencyType.Medal)
			{
				if (currency == CurrencyType.Plat && this.id == "lucky_coin")
				{
					return 100;
				}
			}
			else
			{
				string text = this.id;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 2142614709U)
				{
					if (num <= 696483963U)
					{
						if (num != 319581401U)
						{
							if (num != 371758720U)
							{
								if (num == 696483963U)
								{
									if (text == "diary_catsister")
									{
										return 85;
									}
								}
							}
							else if (text == "wrench_extend_v")
							{
								return 6;
							}
						}
						else if (text == "diary_lady")
						{
							return 25;
						}
					}
					else if (num <= 1237752336U)
					{
						if (num != 875087290U)
						{
							if (num == 1237752336U)
							{
								if (text == "water")
								{
									return 3;
								}
							}
						}
						else if (text == "wrench_extend_h")
						{
							return 6;
						}
					}
					else if (num != 1783752896U)
					{
						if (num == 2142614709U)
						{
							if (text == "diary_sister")
							{
								return 12;
							}
						}
					}
					else if (text == "wrench_bed")
					{
						return 3;
					}
				}
				else if (num <= 2853982554U)
				{
					if (num <= 2315782384U)
					{
						if (num != 2149577330U)
						{
							if (num == 2315782384U)
							{
								if (text == "monsterball")
								{
									return this.LV / 8;
								}
							}
						}
						else if (text == "1165")
						{
							return 10;
						}
					}
					else if (num != 2383682268U)
					{
						if (num == 2853982554U)
						{
							if (text == "wrench_storage")
							{
								return 4;
							}
						}
					}
					else if (text == "wrench_tent_elec")
					{
						return 3;
					}
				}
				else if (num <= 2987720544U)
				{
					if (num != 2893910830U)
					{
						if (num == 2987720544U)
						{
							if (text == "wrench_fridge")
							{
								return 15;
							}
						}
					}
					else if (text == "bill_tax")
					{
						return 3;
					}
				}
				else if (num != 3142504220U)
				{
					if (num == 3660019884U)
					{
						if (text == "container_magic")
						{
							return 20;
						}
					}
				}
				else if (text == "bill")
				{
					return 3;
				}
			}
		}
		if (sell && this.noSell)
		{
			return 0;
		}
		if (!sell && this.id == "casino_coin")
		{
			return 20;
		}
		int value = this.GetValue(sell);
		if (value == 0)
		{
			return 0;
		}
		if (c == null)
		{
			c = EClass.pc;
		}
		double p = (double)value;
		Trait trait = this.trait;
		if (!(trait is TraitBed))
		{
			TraitContainer traitContainer = trait as TraitContainer;
			if (traitContainer != null)
			{
				p *= (double)(1f + 4f * (float)(this.things.width - traitContainer.Width) + 4f * (float)(this.things.height - traitContainer.Height));
			}
		}
		else
		{
			p *= (double)(1f + 0.5f * (float)this.c_containerSize);
		}
		p += (double)this.c_priceAdd;
		if (this.c_priceFix != 0)
		{
			p = (double)((int)((float)p * (float)Mathf.Clamp(100 + this.c_priceFix, 0, 1000000) / 100f));
			if (p == 0.0)
			{
				return 0;
			}
		}
		if (this.isStolen)
		{
			if (sell && priceType == PriceType.PlayerShop && EClass.Branch != null && EClass.Branch.policies.IsActive(2824, -1))
			{
				p = p * 100.0 / (double)Mathf.Max(110f, 170f - Mathf.Sqrt((float)(EClass.Branch.Evalue(2824) * 5)));
			}
			else if (sell && Guild.Thief.IsMember)
			{
				p = (double)Guild.Thief.SellStolenPrice((int)p);
			}
			else
			{
				p *= 0.5;
			}
		}
		if (!sell && this.category.id == "spellbook")
		{
			p = (double)Guild.Mage.BuySpellbookPrice((int)p);
		}
		int num2 = (priceType == PriceType.CopyShop) ? 5 : 1;
		float num3 = 1f + Mathf.Min(0.01f * (float)this.Evalue(752), 1f) + Mathf.Min(0.01f * (float)this.Evalue(751), 1f) * (float)num2;
		p *= (double)num3;
		p *= 0.20000000298023224;
		if (sell)
		{
			p *= 0.20000000298023224;
			if (currency == CurrencyType.Money && (this.category.IsChildOf("meal") || this.category.IsChildOf("preserved")))
			{
				p *= 0.5;
			}
			if (priceType - PriceType.Shipping <= 1 && (this.category.IsChildOf("vegi") || this.category.IsChildOf("fruit")))
			{
				p *= (double)((EClass.pc.faith == EClass.game.religions.Harvest) ? 3f : 2f);
			}
		}
		if (this.id == "rod_wish")
		{
			p *= (double)(sell ? 0.01f : 50f);
		}
		switch (currency)
		{
		case CurrencyType.Medal:
			p *= 0.00019999999494757503;
			goto IL_87C;
		case CurrencyType.Ecopo:
			if (this.trait is TraitSeed)
			{
				p *= 2.0;
				goto IL_87C;
			}
			if (this.trait is TraitEcoMark)
			{
				p *= 1.0;
				goto IL_87C;
			}
			p *= 0.20000000298023224;
			goto IL_87C;
		case CurrencyType.Money2:
			p *= 0.004999999888241291;
			goto IL_87C;
		case CurrencyType.Influence:
			p *= 0.0020000000949949026;
			goto IL_87C;
		case CurrencyType.Casino_coin:
			p *= 0.10000000149011612;
			goto IL_87C;
		}
		if (this.IsIdentified || (this.trait is TraitErohon && !sell))
		{
			if (this.blessedState == BlessedState.Blessed)
			{
				p *= 1.25;
			}
			else if (this.blessedState == BlessedState.Cursed)
			{
				p *= 0.5;
			}
			else if (this.blessedState == BlessedState.Doomed)
			{
				p *= 0.20000000298023224;
			}
			if (this.trait.HasCharges)
			{
				p = p * 0.05000000074505806 + p * (double)(0.5f + Mathf.Clamp(0.1f * (float)this.c_charges, 0f, 1.5f));
			}
			if (this.IsDecayed)
			{
				p *= 0.5;
			}
		}
		else
		{
			Rand.UseSeed(this.uid, delegate
			{
				p = (double)(sell ? (1 + EClass.rnd(15)) : (50 + EClass.rnd(500)));
			});
		}
		if (!sell)
		{
			p *= (double)(1f + 0.2f * (float)c.Evalue(1406));
		}
		IL_87C:
		float num4 = Math.Clamp(Mathf.Sqrt((float)(c.Evalue(291) + ((!sell && EClass._zone.IsPCFaction) ? (EClass.Branch.Evalue(2800) * 2) : 0))), 0f, 25f);
		switch (priceType)
		{
		case PriceType.Shipping:
			if (sell)
			{
				p *= 1.100000023841858;
			}
			break;
		case PriceType.PlayerShop:
			if (sell)
			{
				float num5 = 1.25f;
				if (EClass.Branch != null)
				{
					if (EClass.Branch.policies.IsActive(2817, -1))
					{
						num5 += 0.1f + 0.01f * Mathf.Sqrt((float)EClass.Branch.Evalue(2817));
					}
					if (EClass.Branch.policies.IsActive(2816, -1))
					{
						num5 += 0.2f + 0.02f * Mathf.Sqrt((float)EClass.Branch.Evalue(2816));
					}
					if (this.isChara)
					{
						if (EClass.Branch.policies.IsActive(2828, -1))
						{
							num5 += 0.1f + 0.01f * Mathf.Sqrt((float)EClass.Branch.Evalue(2828));
						}
					}
					else if (this.category.IsChildOf("food") || this.category.IsChildOf("drink"))
					{
						if (EClass.Branch.policies.IsActive(2818, -1))
						{
							num5 += 0.05f + 0.005f * Mathf.Sqrt((float)EClass.Branch.Evalue(2818));
						}
					}
					else if (this.category.IsChildOf("furniture"))
					{
						if (EClass.Branch.policies.IsActive(2819, -1))
						{
							num5 += 0.05f + 0.005f * Mathf.Sqrt((float)EClass.Branch.Evalue(2819));
						}
					}
					else if (EClass.Branch.policies.IsActive(2820, -1))
					{
						num5 += 0.05f + 0.005f * Mathf.Sqrt((float)EClass.Branch.Evalue(2820));
					}
				}
				p *= (double)num5;
			}
			break;
		case PriceType.Tourism:
			num4 = 0f;
			break;
		}
		if (currency > CurrencyType.Money)
		{
			num4 = 0f;
		}
		p *= (double)(sell ? (1f + num4 * 0.02f) : (1f - num4 * 0.02f));
		if (sell)
		{
			p = (double)EClass.curve((int)p, 10000, 10000, 80);
		}
		if (p < 1.0)
		{
			p = (double)(sell ? 0 : 1);
		}
		if (!sell)
		{
			if (currency == CurrencyType.Casino_coin)
			{
				if (p > 100000.0)
				{
					p = (double)(Mathf.CeilToInt((float)p / 100000f) * 100000);
				}
				else if (p > 10000.0)
				{
					p = (double)(Mathf.CeilToInt((float)p / 10000f) * 10000);
				}
				else if (p > 1000.0)
				{
					p = (double)(Mathf.CeilToInt((float)p / 1000f) * 1000);
				}
				else if (p > 100.0)
				{
					p = (double)(Mathf.CeilToInt((float)p / 100f) * 100);
				}
				else if (p > 10.0)
				{
					p = (double)(Mathf.CeilToInt((float)p / 10f) * 10);
				}
			}
			if (this.trait is TraitDeed)
			{
				p *= (double)Mathf.Pow(2f, (float)EClass.player.flags.landDeedBought);
			}
		}
		if (p <= (double)(sell ? 500000000 : 1000000000))
		{
			return (int)p;
		}
		if (!sell)
		{
			return 1000000000;
		}
		return 500000000;
	}

	public virtual string GetHoverText()
	{
		return this.Name + this.GetExtraName();
	}

	public virtual string GetHoverText2()
	{
		return "";
	}

	public int Dist(Card c)
	{
		if (!this.IsMultisize && !c.IsMultisize)
		{
			return this.pos.Distance(c.pos);
		}
		if (this.IsMultisize)
		{
			int dist = 99;
			this.ForeachPoint(delegate(Point p, bool main)
			{
				int num = Card.<Dist>g__DistMulti|846_0(p, c);
				if (num < dist)
				{
					dist = num;
				}
			});
			return dist;
		}
		return Card.<Dist>g__DistMulti|846_0(this.pos, c);
	}

	public int Dist(Point p)
	{
		return this.pos.Distance(p);
	}

	public bool IsInMutterDistance(int d = 10)
	{
		return this.pos.Distance(EClass.pc.pos) < d;
	}

	public void SetCensored(bool enable)
	{
		this.isCensored = enable;
		if (EClass.core.config.other.noCensor)
		{
			this.isCensored = false;
		}
		this.renderer.SetCensored(this.isCensored);
	}

	public void SetDeconstruct(bool deconstruct)
	{
		if (this.isDeconstructing == deconstruct)
		{
			return;
		}
		if (deconstruct)
		{
			EClass._map.props.deconstructing.Add(this);
		}
		else
		{
			EClass._map.props.deconstructing.Remove(this);
		}
		this.isDeconstructing = deconstruct;
	}

	public virtual void SetSortVal(UIList.SortMode m, CurrencyType currency = CurrencyType.Money)
	{
		switch (m)
		{
		case UIList.SortMode.ByNumber:
			this.sortVal = -this.Num * 1000;
			return;
		case UIList.SortMode.ByValue:
			this.sortVal = -this.GetPrice(currency, false, PriceType.Default, null) * 1000;
			return;
		case UIList.SortMode.ByCategory:
			this.sortVal = this.category.sortVal * 1000;
			return;
		case UIList.SortMode.ByEquip:
			this.sortVal = ((this.c_equippedSlot != 0) ? 0 : (this.category.sortVal * 1000));
			return;
		case UIList.SortMode.ByWeight:
			this.sortVal = -this.ChildrenAndSelfWeight * 1000;
			return;
		case UIList.SortMode.ByPrice:
			this.sortVal = -this.GetPrice(currency, false, PriceType.Default, null) * 1000;
			return;
		case UIList.SortMode.ByWeightSingle:
			this.sortVal = -this.ChildrenAndSelfWeightSingle * 1000;
			return;
		}
		this.sortVal = this.sourceCard._index * 1000;
	}

	public virtual int SecondaryCompare(UIList.SortMode m, Card c)
	{
		int num = 0;
		if (num == 0)
		{
			num = this.id.CompareTo(c.id);
		}
		if (num == 0)
		{
			num = this.trait.CompareTo(c);
		}
		if (num == 0)
		{
			num = Lang.comparer.Compare(this.GetName(NameStyle.Full, 1), c.GetName(NameStyle.Full, 1));
		}
		if (num == 0)
		{
			num = this.refVal - c.refVal;
		}
		if (num == 0)
		{
			num = this.c_charges - c.c_charges;
		}
		if (num == 0)
		{
			num = this.encLV - c.encLV;
		}
		if (num == 0)
		{
			num = this.uid - c.uid;
		}
		return num;
	}

	public void ForeachFOV(Func<Point, bool> func)
	{
		if (this.fov == null)
		{
			return;
		}
		foreach (KeyValuePair<int, byte> keyValuePair in this.fov.lastPoints)
		{
			Point arg = new Point().Set(keyValuePair.Key);
			if (func(arg))
			{
				break;
			}
		}
	}

	public void ForeachPoint(Action<Point, bool> action)
	{
		if (this.IsMultisize)
		{
			this.pos.ForeachMultiSize(this.W, this.H, action);
			return;
		}
		action(this.pos, true);
	}

	public void OnInspect()
	{
	}

	public bool CanInspect
	{
		get
		{
			return !this.isDestroyed && this.ExistsOnMap;
		}
	}

	public string InspectName
	{
		get
		{
			return this.Name;
		}
	}

	public Point InspectPoint
	{
		get
		{
			return this.pos;
		}
	}

	public Vector3 InspectPosition
	{
		get
		{
			return this.renderer.position;
		}
	}

	public virtual void WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
	{
	}

	public void Inspect()
	{
		SE.Play("pop_paper");
		if (this.isChara)
		{
			LayerChara layerChara = EClass.ui.AddLayerDontCloseOthers<LayerChara>();
			layerChara.windows[0].SetRect(EClass.core.refs.rects.center, false);
			layerChara.SetChara(this.Chara);
			return;
		}
		EClass.ui.AddLayerDontCloseOthers<LayerInfo>().SetThing(this.Thing, false);
	}

	public virtual bool HasCondition<T>() where T : Condition
	{
		return false;
	}

	public bool HaveFur()
	{
		if (!this.isChara)
		{
			return false;
		}
		string a = this.id;
		return a == "putty_snow" || a == "putty_snow_gold" || !this.Chara.race.fur.IsEmpty();
	}

	public bool CanBeSheared()
	{
		return !EClass._zone.IsUserZone && this.HaveFur() && this.c_fur >= 0;
	}

	[CompilerGenerated]
	private bool <SpawnLoot>g__chance|738_0(int i)
	{
		i = i * 100 / (100 + EClass.player.codex.GetOrCreate(this.id).BonusDropLv * 10);
		if (i < 1)
		{
			i = 1;
		}
		return EClass.rnd(i) == 0;
	}

	[CompilerGenerated]
	internal static bool <MakeFoodRef>g__IsIgnoreName|742_0(Card c)
	{
		if (c == null)
		{
			return true;
		}
		string a = c.id;
		return a == "dough_cake" || a == "dough_bread" || a == "noodle" || a == "flour" || a == "rice";
	}

	[CompilerGenerated]
	private bool <Decay>g__IsParentLocked|809_0()
	{
		return this.parent is Thing && (this.parent as Thing).c_lockLv > 0;
	}

	[CompilerGenerated]
	internal static int <Dist>g__DistMulti|846_0(Point p1, Card c)
	{
		if (!c.IsMultisize)
		{
			return p1.Distance(c.pos);
		}
		int dist = 99;
		c.ForeachPoint(delegate(Point p, bool main)
		{
			int num = p1.Distance(p);
			if (num < dist)
			{
				dist = num;
			}
		});
		return dist;
	}

	[JsonProperty(PropertyName = "A")]
	public int[] _ints = new int[30];

	[JsonProperty(PropertyName = "B")]
	public string id = "";

	[JsonProperty(PropertyName = "C")]
	public ThingContainer things = new ThingContainer();

	[JsonProperty(PropertyName = "D")]
	public ElementContainerCard elements = new ElementContainerCard();

	[JsonProperty(PropertyName = "E")]
	public Biography bio;

	[JsonProperty(PropertyName = "SC")]
	public List<int> sockets;

	public AIAct reservedAct;

	public Props props;

	public Trait trait;

	public ICardParent parent;

	public Fov fov;

	public Point pos = new Point();

	public CardRenderer renderer;

	public int turn;

	public int _colorInt;

	public float roundTimer;

	public float angle = 180f;

	public bool isDestroyed;

	public CardBlueprint bp;

	public BitArray32 _bits1;

	public BitArray32 _bits2;

	public PlaceState placeState;

	public bool dirtyWeight = true;

	private int _childrenWeight;

	private SourceCategory.Row _category;

	public SourceMaterial.Row _material;

	private static Color _randColor;

	public Emo lastEmo;

	private LightData _LightData;

	private Sprite _paintSprite;

	public int sortVal;

	public enum MoveResult
	{
		Fail,
		Success,
		Door
	}

	public enum MoveType
	{
		Walk,
		Force
	}
}
