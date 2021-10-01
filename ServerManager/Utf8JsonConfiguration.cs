using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;
using Utf8Json.Resolvers;

namespace ServerManager
{
    public static class Utf8JsonConfiguration
    {
        static public IJsonFormatterResolver Resolver { get; } =
            CompositeResolver.Create(EnumResolver.UnderlyingValue, StandardResolver.Default);
    }
}
