using System;

namespace StaticSiege.Core;

public sealed class Resources
{
    public int Energy { get; private set; } = 5;
    public int EnergyCap { get; private set; } = 10;
    public int Scrap { get; private set; }
    public int Health { get; private set; } = 100;
    public int HealthMax { get; private set; } = 100;
    public int Shield { get; private set; }
    public int Armor { get; private set; }

    public void AddEnergy(int amount) => Energy = Math.Clamp(Energy + amount, 0, EnergyCap);
    public bool TrySpendEnergy(int amount)
    {
        if (Energy < amount) return false;
        Energy -= amount;
        return true;
    }

    public void AddScrap(int amount) => Scrap = Math.Max(0, Scrap + amount);
    public bool TrySpendScrap(int amount)
    {
        if (Scrap < amount) return false;
        Scrap -= amount;
        return true;
    }

    public void DamageCore(int amount)
    {
        var damage = Math.Max(0, amount - Armor);
        if (damage == 0) return;

        if (Shield > 0)
        {
            var blocked = Math.Min(Shield, damage);
            Shield -= blocked;
            damage -= blocked;
        }

        if (damage <= 0) return;
        Health = Math.Max(0, Health - damage);
    }

    public void Heal(int amount) => Health = Math.Min(HealthMax, Health + amount);
    public void AddShield(int amount) => Shield = Math.Max(0, Shield + amount);
    public void IncreaseMaxHealth(int amount)
    {
        HealthMax += amount;
        Health += amount;
    }

    public void IncreaseArmor(int amount) => Armor += amount;
    public void SetEnergyCap(int cap)
    {
        EnergyCap = Math.Max(0, cap);
        Energy = Math.Min(Energy, EnergyCap);
    }
}

