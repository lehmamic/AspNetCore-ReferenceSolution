using Nuke.Common.Utilities;
using System;

namespace Jenkins.Configuration
{
    public static class JenkinsCustomWriterExtensions
    {
        public static IDisposable WriteBlock(this CustomFileWriter writer, string text)
        {
            return DelegateDisposable
                .CreateBracket(
                    () => writer.WriteLine(string.IsNullOrWhiteSpace(text)
                        ? "{"
                        : $"{text} {{"),
                    () => writer.WriteLine("}"))
                .CombineWith(writer.Indent());
        }
/*
        // public static void WriteArray(this CustomFileWriter writer, string property, string[] values)
        // {
        //     if (!values?.Any() ?? true)
        //         return;
        //
        //     if (values.Length <= 1)
        //     {
        //         writer.WriteLine($"{property} = {values.Single().DoubleQuote()}");
        //         return;
        //     }
        //
        //     writer.WriteLine($"{property} = \"\"\"");
        //     using (writer.Indent())
        //     {
        //         foreach (var value in values)
        //             writer.WriteLine(value);
        //     }
        //
        //     writer.WriteLine("\"\"\".trimIndent()");
        // }*/
    }
}
