--!strict

local EconomyConfig = require("../config/EconomyConfig")
local CosmeticConfig = require("../config/CosmeticConfig")
local RetentionConfig = require("../config/RetentionConfig")

local ProfileRules = {}

local CATEGORY_ORDER = {
	"Trails",
	"LandingEffects",
	"StepThemes",
	"EnvironmentThemes",
}

local function nonNegativeInteger(value: any, fallback: number): number
	if type(value) ~= "number" or value ~= value or value == math.huge or value == -math.huge then
		return fallback
	end
	return math.max(0, math.floor(value))
end

local function categoryConfig(category: string)
	return CosmeticConfig.Categories[category]
end

local function defaultOwned()
	local owned = {}
	for _, category in CATEGORY_ORDER do
		local config = categoryConfig(category)
		owned[category] = {
			[config.Default] = true,
		}
	end
	return owned
end

local function defaultEquipped()
	local equipped = {}
	for _, category in CATEGORY_ORDER do
		local config = categoryConfig(category)
		equipped[category] = config.Default
	end
	return equipped
end

local function defaultAchievements()
	local result = {}
	for achievementId in RetentionConfig.Achievements do
		result[achievementId] = false
	end
	return result
end

function ProfileRules.defaultProfile()
	return {
		Version = EconomyConfig.ProfileVersion,
		PB = 0,
		Coins = 0,
		Owned = defaultOwned(),
		Equipped = defaultEquipped(),
		Settings = {
			ReducedMotion = false,
		},
		TutorialComplete = false,
		Daily = {
			LastClaimDay = -1,
			ClaimStreak = 0,
		},
		Challenge = {
			Day = -1,
			Progress = 0,
			Claimed = false,
		},
		Achievements = defaultAchievements(),
		Totals = {
			Perfect = 0,
			Clean = 0,
		},
		Inventory = {
			Revives = 0,
		},
		Boosts = {
			StabilityUntil = 0,
		},
		ProcessedReceipts = {},
	}
end

function ProfileRules.migrate(raw: any)
	local profile = ProfileRules.defaultProfile()
	if type(raw) ~= "table" then
		return profile
	end

	profile.PB = nonNegativeInteger(raw.PB, 0)
	profile.Coins = nonNegativeInteger(raw.Coins, 0)

	if type(raw.Owned) == "table" then
		for _, category in CATEGORY_ORDER do
			local config = categoryConfig(category)
			local rawCategory = raw.Owned[category]
			if type(rawCategory) == "table" then
				for itemId in config.Items do
					if rawCategory[itemId] == true then
						profile.Owned[category][itemId] = true
					end
				end
			end
			profile.Owned[category][config.Default] = true
		end
	end

	if type(raw.Equipped) == "table" then
		for _, category in CATEGORY_ORDER do
			local config = categoryConfig(category)
			local requested = raw.Equipped[category]
			if
				type(requested) == "string"
				and config.Items[requested] ~= nil
				and profile.Owned[category][requested] == true
			then
				profile.Equipped[category] = requested
			end
		end
	end

	if type(raw.Settings) == "table" and type(raw.Settings.ReducedMotion) == "boolean" then
		profile.Settings.ReducedMotion = raw.Settings.ReducedMotion
	end

	if type(raw.TutorialComplete) == "boolean" then
		profile.TutorialComplete = raw.TutorialComplete
	end

	if type(raw.Daily) == "table" then
		profile.Daily.LastClaimDay = math.floor(tonumber(raw.Daily.LastClaimDay) or -1)
		profile.Daily.ClaimStreak = nonNegativeInteger(raw.Daily.ClaimStreak, 0)
	end

	if type(raw.Challenge) == "table" then
		profile.Challenge.Day = math.floor(tonumber(raw.Challenge.Day) or -1)
		profile.Challenge.Progress = nonNegativeInteger(raw.Challenge.Progress, 0)
		profile.Challenge.Claimed = raw.Challenge.Claimed == true
	end

	if type(raw.Achievements) == "table" then
		for achievementId in RetentionConfig.Achievements do
			profile.Achievements[achievementId] = raw.Achievements[achievementId] == true
		end
	end

	if type(raw.Totals) == "table" then
		profile.Totals.Perfect = nonNegativeInteger(raw.Totals.Perfect, 0)
		profile.Totals.Clean = nonNegativeInteger(raw.Totals.Clean, 0)
	end

	if type(raw.Inventory) == "table" then
		profile.Inventory.Revives = nonNegativeInteger(raw.Inventory.Revives, 0)
	end
	if type(raw.Boosts) == "table" then
		profile.Boosts.StabilityUntil = nonNegativeInteger(raw.Boosts.StabilityUntil, 0)
	end
	if type(raw.ProcessedReceipts) == "table" then
		for _, purchaseId in raw.ProcessedReceipts do
			if type(purchaseId) == "string" and #purchaseId >= 1 and #purchaseId <= 128 then
				table.insert(profile.ProcessedReceipts, purchaseId)
				if #profile.ProcessedReceipts >= 50 then
					break
				end
			end
		end
	end

	return profile
end

function ProfileRules.rewardForLanding(grade: string, streak: number, height: number): number
	local currency = EconomyConfig.Currency
	local reward = 0

	if grade == "Perfect" then
		reward += currency.PerfectLanding
	elseif grade == "Clean" then
		reward += currency.CleanLanding
	else
		return 0
	end

	local normalizedStreak = math.max(0, math.floor(streak))
	local normalizedHeight = math.max(0, math.floor(height))

	if normalizedStreak > 0 and normalizedStreak % currency.StreakEvery == 0 then
		reward += currency.StreakBonus
	end
	if normalizedHeight > 0 and normalizedHeight % currency.HeightMilestoneEvery == 0 then
		reward += currency.HeightMilestoneBonus
	end

	return reward
end

function ProfileRules.validatePurchase(profile: any, category: string, itemId: string): (boolean, string, number)
	local config = categoryConfig(category)
	if not config then
		return false, "invalid_category", 0
	end

	local item = config.Items[itemId]
	if not item then
		return false, "invalid_item", 0
	end
	if profile.Owned[category][itemId] == true then
		return false, "already_owned", item.Price
	end
	if profile.Coins < item.Price then
		return false, "insufficient_coins", item.Price
	end

	return true, "ok", item.Price
end

function ProfileRules.validateEquip(profile: any, category: string, itemId: string): (boolean, string)
	local config = categoryConfig(category)
	if not config or not config.Items[itemId] then
		return false, "invalid_item"
	end
	if profile.Owned[category][itemId] ~= true then
		return false, "not_owned"
	end
	return true, "ok"
end

function ProfileRules.categoryOrder()
	return table.clone(CATEGORY_ORDER)
end

return table.freeze(ProfileRules)
