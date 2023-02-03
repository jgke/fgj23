mainbin := FGJ23/bin/Debug/net6.0/BuildSimpleProject

dep := Nez/patch-applied MonoGame/patch-applied

SOURCES = $(shell find FGJ23 -path FGJ23/obj -prune -o -type f -name "*.cs")

.PHONY: all
all: $(mainbin)

Makefile: ;
%.cs: ;
%.txt: ;

$(mainbin): $(dep) $(SOURCES)
	dotnet build

Nez/patch-applied:
	cd Nez; git am --ignore-space-change ../patches/0000-Nez-Add-horizontal-scrolling.patch && git am --ignore-space-change ../patches/0001-Patched-Nez.patch && git am --ignore-space-change ../patches/0003-add-nez-android-csproj.patch && git am --ignore-space-change ../patches/0004-nez-andr-fix.patch
MonoGame/patch-applied:
	cd MonoGame;  git am --ignore-space-change ../patches/0002-Unlocked-monogame.patch
