using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RazorTypescript
{
    public class ViewTsObject
    {
        public string ViewPath { get; set; }
        public string TypescriptPath { get; set; }
        public string TypescriptFileName  {
            get {
                var nameParts = TypescriptPath.Split(Path.DirectorySeparatorChar);
                return nameParts.Last() == "Index.ts" ? nameParts[nameParts.Length - 2] + nameParts[nameParts.Length - 1] : nameParts[nameParts.Length - 1];
            }
        }

        public string ViewFileName => ViewPath.Split(Path.DirectorySeparatorChar).Last();
        public string TypescriptFileNameNoExtension => TypescriptFileName.Split('.').ToList().RemoveLast(1).Aggregate((i, j) => $"{i}.{j}");
        public string ViewFileNameNoExtension => ViewFileName.Split('.').ToList().RemoveLast(1).Aggregate((i, j) => $"{i}.{j}");
        public string TypescriptImports => Relations.Any() ? Relations.Select(i => i.Import(TypescriptPath)).Aggregate((i, j) => $"{i}{Environment.NewLine}{j}") : "";
        public string TypescriptTemplate => $@"
{TypescriptImports}
{ElementsWithId.InitTypescriptPropertieName()}{ElementsByDataAttribute.InitTypescriptPropertieName()}
export class {TypescriptFileNameNoExtension} {{
{Relations.GetTypescriptModules()}{ElementsWithId.GetTypescriptProperties()}{ElementsByDataAttribute.GetTypescriptProperties()}
    public constructor() {{{Relations.InitTypescriptModules()}{ElementsWithId.InitTypescriptProperties()}{ElementsByDataAttribute.InitTypescriptProperties()}
        console.error('{TypescriptFileNameNoExtension} not implemented');
    }}
}}
".TrimStart(Environment.NewLine.ToArray());
        public List<ViewTsObject> Relations { get; set; }
        public List<ElementWithId> ElementsWithId { get; set; }
        public List<ElementByDataAttribute> ElementsByDataAttribute { get; set; }
        public ViewTsObject()
        {
            Relations = new List<ViewTsObject>();
            ElementsWithId = new List<ElementWithId>();
            ElementsByDataAttribute = new List<ElementByDataAttribute>();
        }

        public string Import(string tsPath)
        {
            var from = tsPath.Split(Path.DirectorySeparatorChar).ToList().RemoveLast(1).Aggregate((i, j) => i + Path.DirectorySeparatorChar + j);
            var relativePath = from.ShortestRelativePath(TypescriptPath).Replace(Path.DirectorySeparatorChar, '/').Replace(".ts", "");
            relativePath = relativePath.First() == '.' ? relativePath : "./" + relativePath; 
            return $"import {{ {TypescriptFileNameNoExtension} }} from \"{relativePath}\";";
        }

        public override string ToString()
        {
            return TypescriptPath + Environment.NewLine + TypescriptTemplate;
            //return ToString(0);
        }
        public string ToString(int depth)
        {
            if (depth > 2)
                return "";
            var retString = $"{new string(' ', depth*4)}{TypescriptPath}" + Environment.NewLine;
            foreach (var rel in Relations)
            {
                retString += rel.ToString(depth + 1);
            }
            return retString;
        }
    }
}
