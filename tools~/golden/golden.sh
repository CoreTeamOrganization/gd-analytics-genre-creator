#!/usr/bin/env bash
# Usage: golden.sh <standalone|sdk> <dir with GenreDefinition.cs + GenreCodeGenerator.cs [+ GenreHost.cs]> [--pre]
# Prints the generator output for a fixed sample. Compile uses the Roslyn + .NET 6 runtime bundled with Unity 2022.3,
# so no dotnet install is needed. Compare against <host>.expected.txt; `--pre` is for code that predates GenreHost.
set -euo pipefail
host=$1; dir=$2; pre=${3:-}
here=$(cd "$(dirname "$0")" && pwd)
unity=${UNITY_CONTENTS:-$(ls -d /Applications/Unity/Hub/Editor/2022.3*/Unity.app/Contents | tail -1)}
rt=$unity/NetCoreRuntime/shared/Microsoft.NETCore.App/6.0.21
out=$(mktemp -d)
defs=(); [ "$host" = sdk ] && defs+=(-define:GOLDEN_SDK); [ "$pre" = --pre ] && defs+=(-define:GOLDEN_PRE)
srcs=("$dir/GenreDefinition.cs" "$dir/GenreCodeGenerator.cs" "$here/Program.cs"); [ "$pre" = --pre ] || srcs+=("$dir/GenreHost.cs")
refs=(); for f in "$rt"/*.dll; do refs+=(-r:"$f"); done
"$unity/NetCoreRuntime/dotnet" "$unity/DotNetSdkRoslyn/csc.dll" -nologo -nowarn:CS8981 -langversion:latest -out:"$out/golden.dll" "${defs[@]+"${defs[@]}"}" "${refs[@]}" "${srcs[@]}" >&2
echo '{"runtimeOptions":{"tfm":"net6.0","framework":{"name":"Microsoft.NETCore.App","version":"6.0.0"}}}' > "$out/golden.runtimeconfig.json"
"$unity/NetCoreRuntime/dotnet" exec --runtimeconfig "$out/golden.runtimeconfig.json" "$out/golden.dll"
rm -rf "$out"
