// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// using System;
namespace Community.PowerToys.Run.Plugin.Dictionary
{
    public class DictionarySearchResult
    {
        public string KeyString { get; }

        public string ResultString { get; }

        public int Score { get; }

        public DictionarySearchResult(string keyString, string resultString, int score)
        {
            KeyString = keyString;
            ResultString = resultString;
            Score = score;
        }

        /*public override bool Equals(object obj)
        {
            return obj is DictionarySearchResult result &&
                   KeyString == result.KeyString &&
                   ResultString == result.ResultString;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(KeyString, ResultString);
        }*/
    }
}
