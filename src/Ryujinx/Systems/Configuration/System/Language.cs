using Ryujinx.Common.Utilities;
using Ryujinx.HLE.HOS.SystemState;
using System.Text.Json.Serialization;

namespace Ryujinx.Ava.Systems.Configuration.System
{
    [JsonConverter(typeof(TypedStringEnumConverter<Language>))]
    public enum Language
    {
        Japanese,
        AmericanEnglish,
        French,
        German,
        Italian,
        Spanish,
        Chinese,
        Korean,
        Dutch,
        Portuguese,
        Russian,
        Taiwanese,
        BritishEnglish,
        CanadianFrench,
        LatinAmericanSpanish,
        SimplifiedChinese,
        TraditionalChinese,
        BrazilianPortuguese,
    }

    public static class LanguageEnumHelper
    {
        extension(SystemLanguage hle)
        {
            public Language Ui => (Language)hle;
        }

        extension(Language ui)
        {
            public SystemLanguage Horizon => (SystemLanguage)ui;
        }
    }
}
