﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GhostTracker.Services
{
    /// <summary>
    ///     Статический класс <c>LanguageDictionary</c> содержит словарь соответствий между названиями языков и их кодами.
    /// </summary>
    /// <remarks>
    ///     Содержит методы для получения названия языка по его коду.
    ///     В словаре хранятся поддерживаемые языки приложения.
    /// </remarks>
    public static class LanguageDictionary
    {
        public static readonly Dictionary<string, string> LanguageMap = new Dictionary<string, string>
        {
            {"English", "EN"},
            {"Русский", "RU"}
        };

        public static string GetLanguageNameByCode(string code)
        {
            // Ищем в словаре первую пару, значение которой соответствует заданному коду
            var languageEntry =
                LanguageMap.FirstOrDefault(x => x.Value.Equals(code, StringComparison.OrdinalIgnoreCase));

            // Если такая пара найдена, возвращаем ключ, иначе null или любое другое указанное значение
            return languageEntry.Key;
        }
    }
}