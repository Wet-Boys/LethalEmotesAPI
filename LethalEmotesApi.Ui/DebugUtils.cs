using System.Collections.Generic;
using System.Text;

namespace LethalEmotesApi.Ui;

internal static class DebugUtils
{
    public static string ToPrettyString<T>(this IEnumerable<T> enumerable)
    {
        var builder = new StringBuilder();
        var depth = 0;

        builder.AppendLine("{");
        depth++;

        int i = 0;
        foreach (var value in enumerable)
        {
            if (value is null)
                continue;
            
            for (int j = 0; j < depth; j++)
                builder.Append("    ");

            builder.AppendLine($"[{i}]: {value.ToString()}");
            i++;
        }

        builder.AppendLine("}");

        return builder.ToString();
    }
}