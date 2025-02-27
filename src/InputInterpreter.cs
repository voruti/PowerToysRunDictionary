﻿// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Text.Json;

namespace Dictionary;

public class InputInterpreter
{
    private readonly Dictionary<string, List<string>> _dictionary;

    public InputInterpreter()
    {
        var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var file = Path.Combine(documents, "PowerToys", "dictionary.json");
        var json = File.ReadAllText(file);

        _dictionary = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json) ??
                      new Dictionary<string, List<string>>();
    }

    public List<DictionarySearchResult> QueryDictionary(string str)
    {
        return _dictionary.Keys
            .ToList()
            .FindAll(stringKey => stringKey.ToLower().StartsWith(str.ToLower()))
            .OrderBy(stringKey =>
                (stringKey.Length - str.Length).ToString("D4") + _dictionary.GetValueOrDefault(stringKey))
            .SelectMany(stringKey => (_dictionary.GetValueOrDefault(stringKey) ?? new List<string>())
                .Select(value => new DictionarySearchResult(stringKey,
                    value,
                    (int) Math.Round((double) str.Length / stringKey.Length * 200))))
            .ToList();
    }
}