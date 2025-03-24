# Advanced Token Economy Mechanisms

This tutorial explores advanced concepts and techniques for implementing a sophisticated token economy in your Unity game using the Rally Protocol SDK. You'll learn how to create a balanced, sustainable economy that enhances player engagement while avoiding common pitfalls.

## Prerequisites

Before starting this tutorial, ensure you have:

- Completed the [Implementing a Token Economy](./implementing-token-economy.md) tutorial
- Basic understanding of economics concepts (inflation, deflation, scarcity)
- Familiarity with game monetization models

## Overview

In this tutorial, we'll cover:

1. **Economic Design Principles** - Fundamental concepts for a healthy economy
2. **Token Supply Management** - Controlling inflation and deflation
3. **Advanced Reward Mechanisms** - Creating engaging reward systems
4. **Value Sinks** - Designing effective ways to remove tokens from circulation
5. **Economic Analytics** - Monitoring and balancing your economy
6. **Tokenomics Simulation** - Testing your economy before deployment

## Economic Design Principles

### Core Principles for Game Economies

1. **Balance** - Maintain equilibrium between token creation and consumption
2. **Scarcity** - Create meaningful scarcity to drive value and engagement
3. **Utility** - Ensure tokens have clear and valuable use cases
4. **Accessibility** - Allow players at all levels to participate in the economy
5. **Fairness** - Create systems that feel fair and rewarding to all player types

### The Circular Flow Model

A healthy token economy follows a circular flow:

- Players earn tokens through gameplay ("faucets")
- Players spend tokens on valuable items or actions ("sinks")
- The cycle creates ongoing engagement and motivation

## Token Supply Management

### Implementing Supply Controls

```csharp
using RallyProtocol.Unity;
using System.Threading.Tasks;
using UnityEngine;

public class TokenSupplyManager : MonoBehaviour
{
    private IRallyNetwork rlyNetwork;
    
    // Configuration
    [SerializeField] private decimal dailyTokenEmissionCap = 1000.0m;
    [SerializeField] private decimal totalSupplyCap = 1000000.0m;
    
    // Tracking variables
    private decimal todayEmittedTokens = 0;
    private System.DateTime currentDay;
    
    private void Awake()
    {
        rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
        currentDay = System.DateTime.Today;
    }
    
    private void Update()
    {
        // Reset daily counter if day changed
        if (System.DateTime.Today != currentDay)
        {
            currentDay = System.DateTime.Today;
            todayEmittedTokens = 0;
            Debug.Log("Daily token emission counter reset");
        }
    }
    
    public async Task<bool> CanEmitTokens(decimal amount)
    {
        // Check against daily cap
        if (todayEmittedTokens + amount > dailyTokenEmissionCap)
            return false;
        
        // Check against total supply cap
        decimal currentTotalSupply = await GetTotalCirculatingSupply();
        return currentTotalSupply + amount <= totalSupplyCap;
    }
    
    public void RecordTokenEmission(decimal amount)
    {
        todayEmittedTokens += amount;
        Debug.Log($"Tokens emitted today: {todayEmittedTokens}/{dailyTokenEmissionCap}");
    }
    
    private async Task<decimal> GetTotalCirculatingSupply()
    {
        // In a real implementation, you would query your token contract
        // or backend service for the current total circulating supply
        
        // This is a simplified placeholder
        await Task.Delay(100);
        return 500000.0m; // Example value
    }
    
    // Dynamic emission rate based on player population and activity
    public decimal CalculateDynamicReward(RewardContext context)
    {
        // Base reward
        decimal baseReward = context.BaseReward;
        
        // Apply multipliers based on economy state
        decimal supplyMultiplier = CalculateSupplyMultiplier();
        decimal activityMultiplier = CalculateActivityMultiplier(context);
        
        // Calculate final reward
        decimal finalReward = baseReward * supplyMultiplier * activityMultiplier;
        
        // Cap the reward to prevent extreme values
        return System.Math.Min(finalReward, context.MaxReward);
    }
    
    private decimal CalculateSupplyMultiplier()
    {
        // As supply increases, rewards decrease
        // This helps control inflation
        decimal currentSupplyRatio = 500000.0m / totalSupplyCap; // Example value
        return 1.0m - (currentSupplyRatio * 0.5m);
    }
    
    private decimal CalculateActivityMultiplier(RewardContext context)
    {
        // More active players = lower individual rewards
        // This balances the economy during high activity periods
        int activePlayerCount = GameAnalytics.Instance.GetActivePlayerCount();
        decimal baseMultiplier = 1.0m;
        
        if (activePlayerCount > 1000)
            baseMultiplier *= 0.9m;
        else if (activePlayerCount > 5000)
            baseMultiplier *= 0.8m;
        else if (activePlayerCount > 10000)
            baseMultiplier *= 0.7m;
            
        return baseMultiplier;
    }
}

public class RewardContext
{
    public decimal BaseReward { get; set; }
    public decimal MaxReward { get; set; }
    public string RewardReason { get; set; }
    public int PlayerLevel { get; set; }
    public int ConsecutiveDays { get; set; }
}
```

### Dealing with Inflation and Deflation

Too many tokens in circulation (inflation) can devalue your in-game currency. Too few tokens (deflation) can frustrate players who feel they can't earn enough.

#### Anti-Inflation Mechanisms

```csharp
public class InflationControl : MonoBehaviour
{
    [SerializeField] private TokenSupplyManager supplyManager;
    
    // Progressive taxes on high-value transactions
    public decimal CalculateTransactionFee(decimal amount)
    {
        if (amount <= 10)
            return 0; // No fee for small transactions
        else if (amount <= 100)
            return amount * 0.01m; // 1% fee
        else if (amount <= 1000)
            return amount * 0.03m; // 3% fee
        else
            return amount * 0.05m; // 5% fee for large transactions
    }
    
    // Dynamic pricing based on economy state
    public decimal GetDynamicPrice(string itemId, decimal basePrice)
    {
        // Get current economic factors
        decimal inflationRate = EconomyStats.Instance.GetInflationRate();
        int itemsSoldToday = ItemCatalog.Instance.GetDailySales(itemId);
        
        // Adjust price based on inflation
        decimal inflationAdjusted = basePrice * (1 + inflationRate);
        
        // Adjust for demand
        decimal demandMultiplier = 1.0m;
        if (itemsSoldToday > 100)
            demandMultiplier = 1.1m;
        else if (itemsSoldToday > 500)
            demandMultiplier = 1.2m;
            
        return inflationAdjusted * demandMultiplier;
    }
}
```

## Advanced Reward Mechanisms

### Multi-Tiered Reward System

```csharp
public class AdvancedRewardSystem : MonoBehaviour
{
    [SerializeField] private TokenSupplyManager supplyManager;
    
    // Different reward tiers
    public enum RewardTier
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
    
    // Base rewards for each tier
    private Dictionary<RewardTier, decimal> baseRewards = new Dictionary<RewardTier, decimal>
    {
        { RewardTier.Common, 1.0m },
        { RewardTier.Uncommon, 3.0m },
        { RewardTier.Rare, 10.0m },
        { RewardTier.Epic, 25.0m },
        { RewardTier.Legendary, 100.0m }
    };
    
    // Rewards with diminishing returns
    public async Task<decimal> CalculateQuestReward(string questId, RewardTier tier, int completionCount)
    {
        // Get base reward for this tier
        decimal baseReward = baseRewards[tier];
        
        // Apply diminishing returns for repeated completions
        decimal repetitionFactor = CalculateDiminishingReturns(completionCount);
        
        // Create context for dynamic calculation
        RewardContext context = new RewardContext
        {
            BaseReward = baseReward * repetitionFactor,
            MaxReward = baseReward * 2, // Cap at 2x base reward
            RewardReason = $"Quest: {questId}",
            PlayerLevel = PlayerStats.Instance.Level,
            ConsecutiveDays = PlayerStats.Instance.ConsecutiveLoginDays
        };
        
        // Calculate final reward using supply management rules
        decimal finalReward = supplyManager.CalculateDynamicReward(context);
        
        // Check if we can emit this amount of tokens
        if (await supplyManager.CanEmitTokens(finalReward))
        {
            supplyManager.RecordTokenEmission(finalReward);
            return finalReward;
        }
        else
        {
            // If we can't emit tokens, provide a minimal reward
            // or use an alternative reward system
            return 0.1m;
        }
    }
    
    private decimal CalculateDiminishingReturns(int completionCount)
    {
        if (completionCount <= 1)
            return 1.0m; // First completion gets full reward
            
        // Each subsequent completion gets diminishing returns
        return 1.0m / (1.0m + (0.1m * (completionCount - 1)));
    }
    
    // Seasonal and event-based rewards
    public decimal GetSeasonalBonus(decimal baseReward)
    {
        // Check if any seasonal event is active
        SeasonalEvent currentEvent = EventManager.Instance.GetCurrentSeasonalEvent();
        
        if (currentEvent != null)
        {
            Debug.Log($"Applying seasonal bonus: {currentEvent.BonusMultiplier}x");
            return baseReward * currentEvent.BonusMultiplier;
        }
        
        return baseReward;
    }
}
```

### Engagement-Based Rewards

Rewarding player engagement helps maintain player retention:

```csharp
public class EngagementRewards : MonoBehaviour
{
    [SerializeField] private AdvancedRewardSystem rewardSystem;
    
    // Streak bonuses for consecutive daily logins
    public async Task<decimal> GetDailyLoginReward()
    {
        int consecutiveDays = PlayerStats.Instance.ConsecutiveLoginDays;
        RewardTier tier = RewardTier.Common;
        
        // Determine reward tier based on streak
        if (consecutiveDays >= 365)
            tier = RewardTier.Legendary;
        else if (consecutiveDays >= 180)
            tier = RewardTier.Epic;
        else if (consecutiveDays >= 30)
            tier = RewardTier.Rare;
        else if (consecutiveDays >= 7)
            tier = RewardTier.Uncommon;
            
        // Calculate reward
        decimal reward = await rewardSystem.CalculateQuestReward("daily_login", tier, 1);
        
        // Apply week milestone bonuses
        if (consecutiveDays % 7 == 0) // Weekly milestone
        {
            reward *= 1.5m; // 50% bonus on 7-day milestone
        }
        
        return reward;
    }
    
    // Time-based activity rewards
    public decimal GetPlayTimeReward(int minutesPlayed)
    {
        // Base reward for playtime
        decimal baseReward = minutesPlayed / 60.0m; // 1 token per hour
        
        // Cap rewards to prevent exploitation
        decimal maxReward = 5.0m; // Max 5 tokens per session
        
        return System.Math.Min(baseReward, maxReward);
    }
}
```

## Value Sinks

Value sinks are mechanisms that remove tokens from circulation, helping balance the economy.

### Implementing Effective Sinks

```csharp
public class TokenSinkManager : MonoBehaviour
{
    private IRallyNetwork rlyNetwork;
    
    [SerializeField] private TokenSupplyManager supplyManager;
    
    private void Awake()
    {
        rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
    }
    
    // Consumable items that burn tokens
    public async Task<bool> PurchaseConsumable(string itemId, int quantity)
    {
        // Get item details
        ConsumableItem item = ItemCatalog.Instance.GetConsumable(itemId);
        if (item == null)
            return false;
            
        // Calculate total cost
        decimal totalCost = item.Price * quantity;
        
        // Check if player has enough tokens
        decimal balance = await rlyNetwork.GetBalanceAsync();
        if (balance < totalCost)
            return false;
            
        // Process the purchase (in a real implementation, this would call a contract)
        Debug.Log($"Burning {totalCost} tokens for consumable purchase");
        
        // Record token sink
        EconomyStats.Instance.RecordTokenSink("consumable_purchase", totalCost);
        
        // Add items to player inventory
        Inventory.Instance.AddItem(itemId, quantity);
        
        return true;
    }
    
    // Premium features that require tokens
    public async Task<bool> ActivatePremiumFeature(string featureId, decimal cost)
    {
        // Check if player has enough tokens
        decimal balance = await rlyNetwork.GetBalanceAsync();
        if (balance < cost)
            return false;
            
        // Activate the feature
        bool activated = await GameFeatures.Instance.ActivateFeature(featureId);
        
        if (activated)
        {
            // Record token sink
            EconomyStats.Instance.RecordTokenSink("premium_feature", cost);
        }
        
        return activated;
    }
    
    // Upgrade systems that consume tokens
    public async Task<bool> UpgradeItem(string itemId, int targetLevel)
    {
        // Get upgrade cost
        UpgradableItem item = Inventory.Instance.GetUpgradableItem(itemId);
        if (item == null || item.Level >= targetLevel)
            return false;
            
        decimal upgradeCost = CalculateUpgradeCost(item, targetLevel);
        
        // Check if player has enough tokens
        decimal balance = await rlyNetwork.GetBalanceAsync();
        if (balance < upgradeCost)
            return false;
            
        // Process the upgrade
        bool upgraded = Inventory.Instance.UpgradeItem(itemId, targetLevel);
        
        if (upgraded)
        {
            // Record token sink
            EconomyStats.Instance.RecordTokenSink("item_upgrade", upgradeCost);
        }
        
        return upgraded;
    }
    
    private decimal CalculateUpgradeCost(UpgradableItem item, int targetLevel)
    {
        decimal baseCost = 10.0m;
        decimal totalCost = 0;
        
        // Calculate cost for each level
        for (int level = item.Level + 1; level <= targetLevel; level++)
        {
            // Exponential cost increase with level
            decimal levelCost = baseCost * System.Math.Pow((double)level, 1.5);
            totalCost += (decimal)levelCost;
        }
        
        return totalCost;
    }
    
    // Time-limited boosts
    public async Task<bool> ActivateTimeBoost(string boostId, int durationHours)
    {
        // Get boost details
        TimeBoost boost = BoostCatalog.Instance.GetBoost(boostId);
        if (boost == null)
            return false;
            
        decimal cost = boost.HourlyCost * durationHours;
        
        // Check if player has enough tokens
        decimal balance = await rlyNetwork.GetBalanceAsync();
        if (balance < cost)
            return false;
            
        // Activate the boost
        bool activated = PlayerBuffs.Instance.ActivateBoost(boostId, durationHours);
        
        if (activated)
        {
            // Record token sink
            EconomyStats.Instance.RecordTokenSink("time_boost", cost);
        }
        
        return activated;
    }
}
```

## Economic Analytics

Monitoring your economy is crucial for maintaining balance. Create systems to track key metrics:

```csharp
public class EconomyStats : MonoBehaviour
{
    public static EconomyStats Instance { get; private set; }
    
    // Token emission tracking
    private Dictionary<string, decimal> tokenEmissionBySource = new Dictionary<string, decimal>();
    
    // Token sink tracking
    private Dictionary<string, decimal> tokenSinkByReason = new Dictionary<string, decimal>();
    
    // Player balance tracking
    private List<decimal> playerBalanceSamples = new List<decimal>();
    
    // Inflation rate calculation
    private decimal previousAverageBalance = 0;
    private decimal currentAverageBalance = 0;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void RecordTokenEmission(string source, decimal amount)
    {
        if (tokenEmissionBySource.ContainsKey(source))
            tokenEmissionBySource[source] += amount;
        else
            tokenEmissionBySource[source] = amount;
            
        // Update total supply metrics
        UpdateEconomyMetrics();
    }
    
    public void RecordTokenSink(string reason, decimal amount)
    {
        if (tokenSinkByReason.ContainsKey(reason))
            tokenSinkByReason[reason] += amount;
        else
            tokenSinkByReason[reason] = amount;
            
        // Update total supply metrics
        UpdateEconomyMetrics();
    }
    
    public void RecordPlayerBalance(decimal balance)
    {
        // Add to samples
        playerBalanceSamples.Add(balance);
        
        // Keep only recent samples (e.g., last 1000)
        if (playerBalanceSamples.Count > 1000)
            playerBalanceSamples.RemoveAt(0);
            
        // Update average balance
        previousAverageBalance = currentAverageBalance;
        currentAverageBalance = CalculateAverageBalance();
    }
    
    private decimal CalculateAverageBalance()
    {
        if (playerBalanceSamples.Count == 0)
            return 0;
            
        decimal sum = 0;
        foreach (decimal balance in playerBalanceSamples)
        {
            sum += balance;
        }
        
        return sum / playerBalanceSamples.Count;
    }
    
    public decimal GetInflationRate()
    {
        if (previousAverageBalance == 0)
            return 0;
            
        return (currentAverageBalance - previousAverageBalance) / previousAverageBalance;
    }
    
    public decimal GetTokenVelocity()
    {
        // Token velocity measures how quickly tokens change hands
        // A simplified calculation: total transactions / average supply
        decimal totalTransactions = GetTotalTransactions();
        decimal averageSupply = currentAverageBalance * playerBalanceSamples.Count;
        
        if (averageSupply == 0)
            return 0;
            
        return totalTransactions / averageSupply;
    }
    
    private decimal GetTotalTransactions()
    {
        decimal total = 0;
        
        foreach (var entry in tokenEmissionBySource)
        {
            total += entry.Value;
        }
        
        foreach (var entry in tokenSinkByReason)
        {
            total += entry.Value;
        }
        
        return total;
    }
    
    public void GenerateEconomyReport()
    {
        Debug.Log("===== ECONOMY REPORT =====");
        
        // Token emission report
        Debug.Log("TOKEN EMISSIONS:");
        foreach (var source in tokenEmissionBySource)
        {
            Debug.Log($"  {source.Key}: {source.Value}");
        }
        
        // Token sink report
        Debug.Log("TOKEN SINKS:");
        foreach (var sink in tokenSinkByReason)
        {
            Debug.Log($"  {sink.Key}: {sink.Value}");
        }
        
        // Overall metrics
        Debug.Log($"Average Player Balance: {currentAverageBalance}");
        Debug.Log($"Inflation Rate: {GetInflationRate() * 100}%");
        Debug.Log($"Token Velocity: {GetTokenVelocity()}");
        
        // Analyze economy health
        AnalyzeEconomyHealth();
    }
    
    private void AnalyzeEconomyHealth()
    {
        decimal inflationRate = GetInflationRate();
        decimal totalEmissions = 0;
        decimal totalSinks = 0;
        
        foreach (var emission in tokenEmissionBySource.Values)
        {
            totalEmissions += emission;
        }
        
        foreach (var sink in tokenSinkByReason.Values)
        {
            totalSinks += sink;
        }
        
        if (inflationRate > 0.05m)
        {
            Debug.LogWarning("HIGH INFLATION: Consider adding more token sinks or reducing emissions");
        }
        else if (inflationRate < -0.05m)
        {
            Debug.LogWarning("DEFLATION: Consider increasing rewards or reducing sink costs");
        }
        
        if (totalEmissions > totalSinks * 1.5m)
        {
            Debug.LogWarning("IMBALANCE: Token emissions significantly exceed sinks");
        }
        else if (totalSinks > totalEmissions * 1.5m)
        {
            Debug.LogWarning("IMBALANCE: Token sinks significantly exceed emissions");
        }
    }
    
    private void UpdateEconomyMetrics()
    {
        // Could implement automatic adjustments based on metrics
        // For now, just log significant changes
        
        decimal inflationRate = GetInflationRate();
        if (System.Math.Abs(inflationRate) > 0.1m)
        {
            Debug.LogWarning($"Significant inflation rate detected: {inflationRate * 100}%");
        }
    }
}
```

## Tokenomics Simulation

Before deploying your economy, simulate it to identify potential issues:

```csharp
public class EconomySimulator : MonoBehaviour
{
    [SerializeField] private int simulationDays = 365;
    [SerializeField] private int playerCount = 1000;
    
    // Simulation parameters
    [SerializeField] private decimal averageDailyRewardPerPlayer = 10.0m;
    [SerializeField] private float dailySinkProbability = 0.3f;
    [SerializeField] private decimal averageSinkAmount = 50.0m;
    [SerializeField] private float playerRetentionRate = 0.95f;
    [SerializeField] private float newPlayerRate = 0.02f;
    
    private struct SimulatedPlayer
    {
        public decimal balance;
        public int daysActive;
        public bool isActive;
    }
    
    public void RunSimulation()
    {
        Debug.Log($"Starting economy simulation with {playerCount} players over {simulationDays} days");
        
        // Initialize players
        SimulatedPlayer[] players = new SimulatedPlayer[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            players[i] = new SimulatedPlayer
            {
                balance = 0,
                daysActive = 0,
                isActive = true
            };
        }
        
        // Metrics tracking
        decimal[] dailyTotalSupply = new decimal[simulationDays];
        decimal[] dailyAverageBalance = new decimal[simulationDays];
        int[] activePlayers = new int[simulationDays];
        
        // Run simulation
        for (int day = 0; day < simulationDays; day++)
        {
            decimal dailyEmissions = 0;
            decimal dailySinks = 0;
            int activePlayerCount = 0;
            
            // Process each player
            for (int i = 0; i < players.Length; i++)
            {
                if (!players[i].isActive)
                    continue;
                    
                // Increment active days
                players[i].daysActive++;
                activePlayerCount++;
                
                // Daily rewards
                decimal dailyReward = CalculateSimulatedReward(players[i].daysActive);
                players[i].balance += dailyReward;
                dailyEmissions += dailyReward;
                
                // Simulate token sinks
                if (UnityEngine.Random.value < dailySinkProbability && players[i].balance > 0)
                {
                    // Calculate sink amount (capped by player balance)
                    decimal sinkAmount = System.Math.Min(
                        averageSinkAmount * (decimal)UnityEngine.Random.Range(0.5f, 1.5f),
                        players[i].balance
                    );
                    
                    players[i].balance -= sinkAmount;
                    dailySinks += sinkAmount;
                }
                
                // Player retention
                if (UnityEngine.Random.value > playerRetentionRate)
                {
                    players[i].isActive = false;
                }
            }
            
            // New players
            int newPlayers = Mathf.FloorToInt(playerCount * newPlayerRate);
            for (int i = 0; i < players.Length && newPlayers > 0; i++)
            {
                if (!players[i].isActive)
                {
                    players[i] = new SimulatedPlayer
                    {
                        balance = 0,
                        daysActive = 0,
                        isActive = true
                    };
                    newPlayers--;
                }
            }
            
            // Record daily metrics
            decimal totalSupply = 0;
            decimal activeBalanceSum = 0;
            
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].isActive)
                {
                    totalSupply += players[i].balance;
                    activeBalanceSum += players[i].balance;
                }
                else
                {
                    // Include inactive player balances in total supply
                    totalSupply += players[i].balance;
                }
            }
            
            dailyTotalSupply[day] = totalSupply;
            dailyAverageBalance[day] = activePlayerCount > 0 ? activeBalanceSum / activePlayerCount : 0;
            activePlayers[day] = activePlayerCount;
            
            // Log periodic updates
            if (day % 30 == 0 || day == simulationDays - 1)
            {
                Debug.Log($"Day {day}: Active Players: {activePlayerCount}, " +
                          $"Total Supply: {totalSupply}, " +
                          $"Avg Balance: {dailyAverageBalance[day]}, " +
                          $"Emissions: {dailyEmissions}, " +
                          $"Sinks: {dailySinks}");
            }
        }
        
        // Final analysis
        AnalyzeSimulationResults(dailyTotalSupply, dailyAverageBalance, activePlayers);
    }
    
    private decimal CalculateSimulatedReward(int daysActive)
    {
        // Base reward
        decimal reward = averageDailyRewardPerPlayer;
        
        // Diminishing returns for long-term players
        if (daysActive > 30)
        {
            reward *= 1.0m - (0.3m * (1.0m - (1.0m / (decimal)System.Math.Sqrt(daysActive / 30.0))));
        }
        
        // Small randomization
        reward *= (decimal)(UnityEngine.Random.Range(0.8f, 1.2f));
        
        return reward;
    }
    
    private void AnalyzeSimulationResults(decimal[] totalSupply, decimal[] averageBalance, int[] activePlayers)
    {
        Debug.Log("===== SIMULATION RESULTS =====");
        
        // Supply growth
        decimal initialSupply = totalSupply[0];
        decimal finalSupply = totalSupply[simulationDays - 1];
        decimal supplyGrowth = initialSupply > 0 ? (finalSupply - initialSupply) / initialSupply : 0;
        
        Debug.Log($"Supply Growth: {supplyGrowth * 100}% over {simulationDays} days");
        
        // Player retention
        int initialPlayers = activePlayers[0];
        int finalPlayers = activePlayers[simulationDays - 1];
        float retentionRate = initialPlayers > 0 ? (float)finalPlayers / initialPlayers : 0;
        
        Debug.Log($"Player Retention: {retentionRate * 100}% ({finalPlayers}/{initialPlayers})");
        
        // Balance distribution at end
        Debug.Log($"Final Average Balance: {averageBalance[simulationDays - 1]}");
        
        // Inflation rate
        decimal[] inflationRates = new decimal[simulationDays - 1];
        for (int i = 1; i < simulationDays; i++)
        {
            if (averageBalance[i - 1] > 0)
            {
                inflationRates[i - 1] = (averageBalance[i] - averageBalance[i - 1]) / averageBalance[i - 1];
            }
        }
        
        decimal avgInflation = CalculateAverage(inflationRates);
        Debug.Log($"Average Daily Inflation Rate: {avgInflation * 100}%");
        
        // Recommendations based on results
        ProvideRecommendations(supplyGrowth, avgInflation, retentionRate);
    }
    
    private decimal CalculateAverage(decimal[] values)
    {
        if (values.Length == 0)
            return 0;
            
        decimal sum = 0;
        foreach (decimal val in values)
        {
            sum += val;
        }
        
        return sum / values.Length;
    }
    
    private void ProvideRecommendations(decimal supplyGrowth, decimal avgInflation, float retentionRate)
    {
        Debug.Log("===== RECOMMENDATIONS =====");
        
        if (supplyGrowth > 2.0m)
        {
            Debug.LogWarning("HIGH SUPPLY GROWTH: Consider adding more token sinks or reducing emissions");
        }
        else if (supplyGrowth < 0.2m)
        {
            Debug.LogWarning("LOW SUPPLY GROWTH: Consider increasing rewards or reducing sink costs");
        }
        else
        {
            Debug.Log("HEALTHY SUPPLY GROWTH: The economy is expanding at a sustainable rate");
        }
        
        if (avgInflation > 0.01m)
        {
            Debug.LogWarning("HIGH INFLATION: Daily average balance is increasing rapidly");
        }
        else if (avgInflation < -0.01m)
        {
            Debug.LogWarning("DEFLATION: Daily average balance is decreasing");
        }
        else
        {
            Debug.Log("STABLE PRICES: The economy shows price stability");
        }
        
        if (retentionRate < 0.5f)
        {
            Debug.LogWarning("POOR RETENTION: The economy may not be engaging enough");
        }
        else
        {
            Debug.Log("GOOD RETENTION: Players are staying engaged with the economy");
        }
    }
}
```

## Best Practices for Advanced Economies

1. **Start Conservative**: Begin with lower token emissions and gradually increase based on data.

2. **Monitor Constantly**: Implement detailed analytics to track economic health, and be prepared to make adjustments.

3. **Balance Growth**: Aim for modest inflation (1-3% over time) to encourage spending but maintain value.

4. **Design for All Players**: Create meaningful rewards and sinks for players at all levels.

5. **Test Thoroughly**: Simulate your economy before launch and during beta testing.

6. **Communicate Changes**: When making economic adjustments, communicate the reasons to your players.

7. **Avoid Radical Changes**: Make gradual adjustments rather than sudden dramatic shifts.

8. **Plan for the Long Term**: Design your economy to be sustainable over years, not just months.

## Economy Balance Decision Table

| Symptom | Potential Cause | Solution |
|---------|-----------------|----------|
| Rapid inflation | Too many tokens entering, too few exiting | Add more sinks, reduce rewards |
| Deflation | Too few tokens entering, too many exiting | Increase rewards, reduce sink costs |
| Poor engagement | Insufficient reward utility | Add more valuable uses for tokens |
| Wealth inequality | Linear rewards favoring power players | Implement diminishing returns, boost newcomers |
| Price volatility | Unstable supply/demand | Add price stabilization mechanisms |
| Player complaints | Perceived unfairness | Increase transparency, adjust based on feedback |

## Conclusion

Building an advanced token economy requires careful planning, continuous monitoring, and thoughtful adjustments. By implementing the techniques described in this tutorial, you can create a sustainable and engaging economic system that enhances your game's player retention and monetization.

Remember that every game economy is unique, and what works for one game may not work for another. Use these techniques as a starting point, and adapt them to your specific game's needs and player behaviors.

## Next Steps

- Implement [NFTs in Your Economy](./implementing-nfts.md) to add unique digital assets
- Explore [Player-to-Player Trading](./implementing-p2p-trading.md) to enable a player-driven marketplace
- Learn about [Security Best Practices](../guides/security-best-practices.md) to protect your economy from exploitation
