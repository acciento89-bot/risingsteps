#!/bin/sh
set -eu
mastery_repo=$(CDPATH= cd -- "$(dirname -- "$0")/../.." && pwd)
mastery_mono=${RISING_MASTERY_MONO:-/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/Resources/Scripting/MonoBleedingEdge/bin/mono}
mastery_csc=${RISING_MASTERY_CSC:-/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/Resources/Scripting/MonoBleedingEdge/lib/mono/4.5/csc.exe}
mastery_temp=$(mktemp -d "${TMPDIR:-/tmp}/rising-mastery-archive.XXXXXX")
trap 'rm -rf -- "$mastery_temp"' EXIT HUP INT TERM
set -- "$mastery_repo/tools/regressions/MasteryArchiveTests.cs"
for mastery_source in "$mastery_repo/Assets/Scripts/Core/Mastery"/Mastery*.cs; do
 [ -f "$mastery_source" ] || continue
 case "$mastery_source" in *Unity*|*Runtime*) continue;; esac
 set -- "$@" "$mastery_source"
done
"$mastery_mono" "$mastery_csc" -nologo -langversion:latest "-out:$mastery_temp/MasteryArchiveTests.exe" "$@"
"$mastery_mono" "$mastery_temp/MasteryArchiveTests.exe"
