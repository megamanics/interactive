﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Microsoft.DotNet.Interactive.Jupyter.ValueSharing;

public class Variable
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
}
public class VariablesResponseBody : IValueAdapterResponseBody
{
    [JsonPropertyName("variables")]
    public IReadOnlyList<Variable> Variables { get; }

    public VariablesResponseBody(IReadOnlyList<Variable> variables)
    {
        Variables = variables;
    }
}

[ValueAdapterMessageType(ValueAdapterMessageType.Response)]
[ValueAdapterCommand(ValueAdapterCommandTypes.Variables)]
public class VariablesResponse : ValueAdapterResponse<VariablesResponseBody>
{
    public VariablesResponse(bool success, VariablesResponseBody body, string message = null) : base(success, body, message)
    {
    }
}
