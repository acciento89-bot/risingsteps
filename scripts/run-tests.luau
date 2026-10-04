local tests = {
	require("../tests/fantasy-presentation.spec"),
	require("../tests/visual-polish.spec"),
	require("../tests/art-config.spec"),
	require("../tests/audio-config.spec"),
	require("../tests/config.spec"),
	require("../tests/graphics-quality-rules.spec"),
	require("../tests/persistence-rules.spec"),
	require("../tests/profile-rules.spec"),
	require("../tests/reachability.spec"),
	require("../tests/receipt-rules.spec"),
	require("../tests/remote-authority.spec"),
	require("../tests/retention-rules.spec"),
	require("../tests/security-rules.spec"),
	require("../tests/simulation.spec"),
	require("../tests/score-rules.spec"),
	require("../tests/step-rules.spec"),
	require("../tests/step-sequence.spec"),
	require("../tests/viewport-rules.spec"),
}

for index, result in tests do
	assert(result == true, ("test module %d did not return true"):format(index))
end

print(("Rising Steps: %d pure Luau test files passed"):format(#tests))
