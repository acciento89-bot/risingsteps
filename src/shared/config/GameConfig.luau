--!strict

local GameConfig = {
	Name = "Rising Steps",
	Version = 1,
	Score = {
		PrimaryMetric = "Height",
		BestMetric = "PB",
		StreakMetric = "Streak",
		Grades = {
			Perfect = "Perfect",
			Clean = "Clean",
			Miss = "Miss",
		},
		PerfectToleranceRatio = 0.30,
		CleanToleranceRatio = 0.82,
	},
	Movement = {
		WalkSpeed = 16,
		JumpPower = 50,
		UseJumpPower = true,
		Gravity = 196.2,
		AllowDoubleJump = false,
		AllowDash = false,
	},
	Step = {
		BaseFootprint = 9,
		CollisionThickness = 1.25,
		ProtectedSeconds = 1.25,
		WarningSeconds = 1.35,
		MinimumWarningSeconds = 0.70,
		ExpiryTransitionSeconds = 0.35,
		MinimumSafeTargetsAhead = 2,
		MaxHorizontalCenterDistance = 8.25,
		MaxVerticalRise = 5.0,
	},
	Failure = {
		FallDistanceBelowHighestValidated = 34,
		MaxPaidRevivesPerRun = 1,
		RetryTargetSeconds = 2.0,
	},
	Camera = {
		CompactPhoneAvatarHeightMin = 0.24,
		CompactPhoneAvatarHeightMax = 0.30,
		TabletAvatarHeightMin = 0.21,
		TabletAvatarHeightMax = 0.27,
		DesktopAvatarHeightMin = 0.18,
		DesktopAvatarHeightMax = 0.24,
		MinimumUpcomingTargetsVisible = 2,
	},
}

return table.freeze(GameConfig)
