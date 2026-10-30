#!/usr/bin/env bash
# Vendors the shared Genre Creator core into the GDMonetization SDK checkout.
#   tools~/sync-to-sdk.sh <sdk-repo>           copy core files into the SDK
#   tools~/sync-to-sdk.sh <sdk-repo> --check   exit 1 if the SDK copy has drifted (use before every SDK release)
# Copies only the shared files: GenreHost*.cs are host-owned and never touched, and .meta files are never written
# (their GUIDs must stay stable for existing SDK users). The package's editor namespace is rewritten to the SDK's
# so the SDK copy and an installed standalone package can coexist without type ambiguity.
set -euo pipefail
sdk=${1:?usage: sync-to-sdk.sh <sdk-repo> [--check]}; mode=${2:-}
here=$(cd "$(dirname "$0")/.." && pwd)
src=$here/Editor/GenreCreator
dst=$sdk/Assets/GDMonetization/Editor/Scripts/GenreCreator
core=(GenreAssetCreator GenreCodeGenerator GenreCreatorWindow GenreDefinition GenreExcelParser IdentifierValidator)
version=$(sed -n 's/.*"version": *"\([^"]*\)".*/\1/p' "$here/package.json")

[ -d "$dst" ] || { echo "not an SDK checkout: $dst missing" >&2; exit 2; }
for h in GenreHost.cs GenreHost.Editor.cs; do
  [ -f "$dst/$h" ] || { echo "SDK is missing host file $h — add the SDK's GenreHost files first" >&2; exit 2; }
done

render() { # $1 = core file name -> synced content on stdout
  { echo "// AUTO-SYNCED from gd-analytics-genre-creator v$version (Editor/GenreCreator/$1.cs)."
    echo "// DO NOT EDIT HERE: change it in that repo and run tools~/sync-to-sdk.sh. Host-specific code lives in GenreHost*.cs."
    sed 's/GameDistrict\.MeticaAnalytics\.Editor/GDMonetization.Editor/g' "$src/$1.cs"
  }
}

drift=0
for f in "${core[@]}"; do
  if [ "$mode" = --check ]; then
    render "$f" | cmp -s - "$dst/$f.cs" || { echo "DRIFT: $f.cs"; drift=1; }
  else
    render "$f" > "$dst/$f.cs"; echo "synced $f.cs (v$version)"
  fi
done
[ "$mode" = --check ] && { [ $drift = 0 ] && echo "SDK copy matches genre-creator v$version"; exit $drift; }
exit 0
