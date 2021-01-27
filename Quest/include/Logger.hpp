#pragma once 

#include "beatsaber-hook/shared/utils/utils.h"
#include "beatsaber-hook/shared/utils/il2cpp-utils.hpp"

#define VERSION "0.1.0"
#define ID "Weather"

namespace Weather
{
    class GenericLogger
    {
        public:
            static inline ModInfo modInfo = {ID, VERSION};
            static Logger& getLogger() {
                static auto logger = new Logger(modInfo, LoggerOptions(false, true));
                return *logger;
            }
            static const void Log(std::string Message)
            {
                getLogger().debug(Message);
            }
    };
}