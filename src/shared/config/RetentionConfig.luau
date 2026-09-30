--!strict

local RetentionConfig = {
	Daily = table.freeze({
		BaseCoins = 30,
		ConsecutiveDayBonus = 5,
		MaxBonusDays = 7,
	}),
	Challenges = table.freeze({
		table.freeze({ Kind = "Height", Target = 15, RewardCoins = 45 }),
		table.freeze({ Kind = "Streak", Target = 8, RewardCoins = 50 }),
		table.freeze({ Kind = "Perfect", Target = 5, RewardCoins = 55 }),
	}),
	Achievements = table.freeze({
		height_10 = table.freeze({ Kind = "Height", Target = 10, RewardCoins = 25 }),
		height_30 = table.freeze({ Kind = "Height", Target = 30, RewardCoins = 60 }),
		streak_10 = table.freeze({ Kind = "Streak", Target = 10, RewardCoins = 40 }),
		perfect_25 = table.freeze({ Kind = "PerfectTotal", Target = 25, RewardCoins = 75 }),
	}),
}

return table.freeze(RetentionConfig)
