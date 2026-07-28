using System;
using System.Collections.Generic;
using System.Text;


namespace FontExamine
{
    public static class FluentIconHelper
    {
        public enum FluentSymbols
        {
            Add_12,
            Add_16,
            Airplane_20,
            AppGeneric_24,
            Apps_16,
            Apps_20,
            AppsList_24,
            ArrowClockwise_20,
            ArrowClockwise_24,
            ArrowImport_24,
            ArrowSync_12,
            Attach_16,
            Delete_20,
            Delete_24,
            Globe_20,
            Globe_24,
            Save_20,
            Save_24
        }
        public static readonly Dictionary<FluentSymbols, string> Regular = new()
{
 {FluentSymbols.Add_12,"\uf107"},
 {FluentSymbols.Add_16,"\uf108"},
 {FluentSymbols.Airplane_20,"\uf10f"},
 {FluentSymbols.AppGeneric_24,"\uf124"},
 {FluentSymbols.Apps_16,"\uf132"},
 {FluentSymbols.Apps_20,"\uf133"},
 {FluentSymbols.AppsList_24,"\uf138"},
 {FluentSymbols.ArrowClockwise_20,"\uf13d"},
 {FluentSymbols.ArrowClockwise_24,"\uf13e"},
 {FluentSymbols.ArrowImport_24,"\uf15a"},
 {FluentSymbols.ArrowSync_12,"\uf18f"},
 {FluentSymbols.Attach_16,"\uf1a8"},
 {FluentSymbols.Delete_20,"\uf34c"},
 {FluentSymbols.Delete_24,"\uf34d"},
 {FluentSymbols.Globe_20,"\uf45a"},
 {FluentSymbols.Globe_24,"\uf45b"},
 {FluentSymbols.Save_20,"\uf67f"},
 {FluentSymbols.Save_24,"\uf680"}};

        // Similar code can be generated for filled icons if needed
        public static readonly Dictionary<FluentSymbols, string> Filled = new()
{
 { FluentSymbols.Add_12 , "\uf107" },
 { FluentSymbols.Add_16 , "\uf108" },
 { FluentSymbols.Airplane_20 , "\uf10f" },
 { FluentSymbols.AppGeneric_24 , "\uf124" },
 { FluentSymbols.Apps_16 , "\uf132" },
 { FluentSymbols.Apps_20 , "\uf133" },
 { FluentSymbols.AppsList_24 , "\uf138" },
 { FluentSymbols.ArrowClockwise_20 , "\uf13d" },
 { FluentSymbols.ArrowClockwise_24 , "\uf13e" },
 { FluentSymbols.ArrowImport_24 , "\uf15a" },
 { FluentSymbols.ArrowSync_12 , "\uf18f" },
 { FluentSymbols.Attach_16 , "\uf1a8" },
 { FluentSymbols.Delete_20 , "\uf34c" },
 { FluentSymbols.Delete_24 , "\uf34d" },
 { FluentSymbols.Globe_20 , "\uf45e" },
 { FluentSymbols.Globe_24 , "\uf45f" },
 { FluentSymbols.Save_20 , "\uf689" },
 { FluentSymbols.Save_24 , "\uf68a" }};
    }  // End of generated code

}
