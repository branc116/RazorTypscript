using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RazorTypescript
{
    public static class Extesions
    {
        public static string ShortestRelativePath(this string fromFile, string toFile)
        {
            return Path.GetRelativePath(fromFile, toFile);
        }
        public static IEnumerable<T> RemoveLast<T>(this List<T> c, int removeCount)
        {
            return c.Take(c.Count - removeCount);
        }
        public static string GetTypescriptProperties<T>(this List<T> list) where T : IConvertable
        {
            return list.Any() ?
                Environment.NewLine + list
                .Select(i => i.GetTypescriptProperties())
                .Aggregate((i, j) => i + Environment.NewLine + j) + Environment.NewLine :
                "";
        }
        public static string InitTypescriptProperties<T>(this List<T> list) where T: IConvertable
        {
            return list.Any() ?
                Environment.NewLine + list.Select(i => i.InitTypescriptProperties())
                .Aggregate((i, j) => i + Environment.NewLine + j) + Environment.NewLine :
                "";
        }
        public static string InitTypescriptPropertieName<T>(this List<T> list) where T : IConvertable
        {
            return list.Any() ?
                Environment.NewLine + list.Select(i => i.InitTypescriptPropertieName())
                .Aggregate((i, j) => i + Environment.NewLine + j) + Environment.NewLine :
                "";
        }
        public static string InitViewProperties<T>(this List<T> list) where T: IConvertable
        {
            return list.Any() ?
                "<script>" + Environment.NewLine +
                list.Select(i => i.InitViewProperties())
                .Aggregate((i, j) => i + Environment.NewLine + j) +
                Environment.NewLine + "</script>" + Environment.NewLine :
                "";
        }
        public static string GetTypescriptModules(this List<ViewTsObject> list)
        {
            return list.Any() ?
                Environment.NewLine + list
                .Select(i => $"    public {i.TypescriptFileNameNoExtension}: {i.TypescriptFileNameNoExtension};")
                .Aggregate((i, j) => i + Environment.NewLine + j) + Environment.NewLine :
                "";
        }
        public static string InitTypescriptModules(this List<ViewTsObject> list)
        {
            return list.Any() ?
                Environment.NewLine + list.Select(i => $"        this.{i.TypescriptFileNameNoExtension} = new {i.TypescriptFileNameNoExtension}();")
                .Aggregate((i, j) => i + Environment.NewLine + j) + Environment.NewLine :
                "";
        }
        public static string SpaggetyToCamil(this string str)
        {
            var retString = str.Split('-').Aggregate((h, j) => {
                var f = j.First();
                var second = (f <= 'z' && f >= 'a' ? ((char)(f + 'A' - 'a')) : f) + j.Substring(1) ;
                return h + second;
            });
            retString = retString.Replace(";", "")
                .Replace(":", "");

            return retString[0] >= '0' && retString[0] <= '9' ? '_' + retString : retString;
        }
    }
}
