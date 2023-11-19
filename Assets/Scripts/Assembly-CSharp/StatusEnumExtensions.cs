public static class StatusEnumExtensions
{
	public static string Name(this StatusEnum status)
	{
		switch (status)
		{
		case StatusEnum.AttackBoost:
			return StatusEnum.EnhanceSTR.Name();
		case StatusEnum.AttackSpike:
			return StatusEnum.BurstSTR.Name();
		case StatusEnum.MagicBoost:
			return StatusEnum.EnhanceINT.Name();
		case StatusEnum.MagicSpike:
			return StatusEnum.BurstINT.Name();
		case StatusEnum.DefenseBoost:
			return StatusEnum.FortifyDEF.Name();
		case StatusEnum.ResistanceBoost:
			return StatusEnum.FortifyRES.Name();
		case StatusEnum.IgnoreDefense:
			return StatusEnum.Defiance.Name();
		case StatusEnum.IgnoreResistance:
			return StatusEnum.Wisdom.Name();
		case StatusEnum.Evasion:
			return StatusEnum.Stealth.Name();
		case StatusEnum.FrostArmor:
			return StatusEnum.Chilled.Name();
		case StatusEnum.FlameArmor:
			return StatusEnum.Flaming.Name();
		case StatusEnum.ParisiticArmor:
			return StatusEnum.Parisitic.Name();
		case StatusEnum.RadiantArmor:
			return StatusEnum.Brilliant.Name();
		case StatusEnum.VoidArmor:
			return StatusEnum.Deranged.Name();
		case StatusEnum.Siphon:
			return StatusEnum.APSiphon.Name();
		case StatusEnum.Counterattack:
			return StatusEnum.StrengthCounter.Name();
		case StatusEnum.MagicCounterattack:
			return StatusEnum.MagicCounter.Name();
		case StatusEnum.WeakenStrength:
			return StatusEnum.WeakenSTR.Name();
		case StatusEnum.WeakenMagic:
			return StatusEnum.WeakenINT.Name();
		case StatusEnum.BreakDefense:
			return StatusEnum.BreakDEF.Name();
		case StatusEnum.BreakResistance:
			return StatusEnum.BreakRES.Name();
		case StatusEnum.Burn:
			return StatusEnum.Burned.Name();
		case StatusEnum.Freeze:
			return StatusEnum.Frozen.Name();
		case StatusEnum.Transmorph:
			return StatusEnum.Transmogrify.Name();
		case StatusEnum.ManaLeak:
			return StatusEnum.APLeak.Name();
		case StatusEnum.EnergyBleed:
			return StatusEnum.APPoison.Name();
		default:
			return status.ToString();
		}
	}
}
