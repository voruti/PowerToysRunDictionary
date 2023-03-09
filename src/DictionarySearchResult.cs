// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Dictionary;

public class DictionarySearchResult
{
    public DictionarySearchResult(string keyString, string resultString, int score)
    {
        KeyString = keyString;
        ResultString = resultString;
        Score = score;
    }

    public string KeyString { get; }

    public string ResultString { get; }

    public int Score { get; }
}