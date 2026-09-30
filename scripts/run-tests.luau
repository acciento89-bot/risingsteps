local tests = {
	require("../tests/art-config.spec"),
	require("../tests/audio-config.spec"),
	require("../tests/config.spec"),
	require("../tests/profile-rules.spec"),
	require("../tests/reachability.spec"),
	require("../tests/receipt-rules.spec"),
	require("../tests/remote-authority.spec"),
	require("../tests/retention-rules.spec"),
	require("../tests/simulation.spec"),
	require("../tests/score-rules.spec"),
	require("../tests/step-rules.spec"),
	require("../tests/step-sequence.spec"),
}

for index, result in tests do
	assert(result == true, ("test module %d did not return true"):format(index))
end

print(("Rising Steps: %d pure Luau test files passed"):format(#tests))
