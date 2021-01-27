#pragma once

#include <unordered_set>
#include "beatsaber-hook/shared/utils/utils.h"
#include "beatsaber-hook/shared/utils/il2cpp-utils.hpp"
#include "beatsaber-hook/shared/config/config-utils.hpp"

static struct Config_t {
    std::unordered_set<std::string> enabledEffects;
    float audioSFXVolume = 1.0f;
    bool enabledInMenu = true;
    bool enabledInGameplay = false;
    bool enableShaderAdditions = true;
} Config;

void ReloadConfig();

Configuration& getConfig();