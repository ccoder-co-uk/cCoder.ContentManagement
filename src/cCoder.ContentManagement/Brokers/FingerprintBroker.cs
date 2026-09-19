// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Security.Cryptography;
using System.Text;

namespace cCoder.ContentManagement.Brokers;

internal sealed class FingerprintBroker : IFingerprintBroker
{
    public string Compute(string value) =>
        Convert.ToHexString(
            inArray: SHA256.HashData(
                source: Encoding.UTF8.GetBytes(s: value)));
}