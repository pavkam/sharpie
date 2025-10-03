  PROJECTS=(
    "Sharpie/Sharpie.csproj;Sharpie-Curses;sharpie"
    "NativeLibraries/Sharpie.NativeLibraries.NCurses.csproj;Sharpie-Libs-NCurses;ncurses"
    "NativeLibraries/Sharpie.NativeLibraries.PdCurses.csproj;Sharpie-Libs-PdCurses;pdcurses"
    "NativeLibraries/Sharpie.NativeLibraries.PdCursesMod.csproj;Sharpie-Libs-PdCursesMod;pdcursesmod"
  )

  for LIB in "${PROJECTS[@]}"
  do
    SPLIT=(${LIB//;/ })
    LIB_PATH=${SPLIT[0]}
    LIB_PKG=${SPLIT[1]}
    LIB_MONIKER=${SPLIT[2]}

    VERSION=`cat $LIB_PATH | sed -n "s/\s*<OverallVersion>\(.*\)<\/OverallVersion>$/\1/p"`

    echo "version=$VERSION"
    DEP=`wget -q https://www.nuget.org/api/v2/package/$LIB_PKG/$VERSION -O /dev/null || echo NO`

    if [ "$DEP" = "NO" ]; then
      echo "${LIB_MONIKER}_deployed=no"
    else
      echo "${LIB_MONIKER}_deployed=yes"
    fi
  done
