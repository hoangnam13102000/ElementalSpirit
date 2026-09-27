using ElementalSpirit.Domain.Loot;
using ElementalSpirit.Domain.Player;
using Xunit;

namespace ElementalSpirit.Tests;

public class LootDropTests
{
    [Fact]
    public void HealthPotion_HasTenPercentHealValue()
    {
        var potion = LootDrop.CreateHealthPotion(12f, 24f);

        Assert.Equal(LootType.HealthPotion, potion.Type);
        Assert.Equal(10, potion.HealPercent);
        Assert.Equal(12f, potion.X);
        Assert.Equal(24f, potion.Y);
    }

    [Fact]
    public void Player_HealsByTenPercentOfMaximumHealth_WithoutExceedingMaximum()
    {
        var player = new Player(0f, 0f);
        player.TakeDamage(50);
        int healAmount = (int)System.Math.Ceiling(player.MaxHp * 10 / 100d);

        player.Heal(healAmount);

        Assert.Equal(60, player.CurrentHp);
        player.Heal(healAmount * 10);
        Assert.Equal(player.MaxHp, player.CurrentHp);
    }
}
