using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorTypescript
{
    public class GetAllViews
    {
        private const string VIEW_EXTENSION = ".cshtml";
        public List<ViewTsObject> AllViewTsObjects { get; set; }
        private string _inDir;
        private string _outDir;
        public GetAllViews(string inDir, string outDir)
        {
            _inDir = inDir;
            _outDir = outDir;
            AllViewTsObjects = new List<ViewTsObject>();
        }

        public GetAllViews Scan()
        {
            return Scan(_inDir, _outDir);
        }

        private GetAllViews Scan(string inPath, string outPath)
        {
            var dirs = Directory.GetDirectories(inPath);
            foreach(var dir in dirs.Select(i => i.Split(Path.DirectorySeparatorChar).Last()))
            {
                Scan(Path.Combine(inPath, dir), Path.Combine(outPath, dir));
            }
            var files = Directory
                .GetFiles(inPath)
                .Where(i => i.EndsWith(VIEW_EXTENSION))
                .Select(i => i.Split(Path.DirectorySeparatorChar).Last())
                .ToList();
            foreach(var file in files)
            {
                AllViewTsObjects.Add(new ViewTsObject
                {
                    TypescriptPath = Path.Combine(outPath, file.Replace(".cshtml", ".ts")),
                    ViewPath = Path.Combine(inPath, file)
                });
            }
            return this;
        }
        public GetAllViews CreateRelations()
        {
            foreach(var ob in AllViewTsObjects)
            {
                var content = File.ReadAllText(ob.ViewPath);
                var splited = content.Split('"').Where(i => AllViewTsObjects.Any(j => i.Contains(j.TypescriptFileNameNoExtension)));
                foreach(var relations in splited)
                {
                    var vTs = AllViewTsObjects.First(i => relations.Contains(i.TypescriptFileNameNoExtension));
                    if (ob.ViewPath != vTs.ViewPath && ob.Relations.All(i => i.ViewPath != vTs.ViewPath))
                        ob.Relations.Add(vTs);
                }
            }
            return this;
        }
        public GetAllViews GetElemenentsWithId()
        {
            foreach (var element in AllViewTsObjects)
            {
                var html = new HtmlAgilityPack.HtmlDocument();
                html.LoadHtml(File.ReadAllText(element.ViewPath));
                var elemehtsWithId = html.DocumentNode.Descendants().Where(i => !string.IsNullOrWhiteSpace(i.Id));
                foreach(var ele in elemehtsWithId)
                {
                    var e = new ElementWithId
                    {
                        Id = ele.Id,
                        Name = ele.Id.Split(new[] { '.', '@', '(', ')' }).Aggregate((i, j) => $"{i}{j}").SpaggetyToCamil() + "Id"
                    };
                    if (!element.ElementsWithId.Any(i => i.Name == e.Name))
                        element.ElementsWithId.Add(e);
                }
            }
            return this;
        }

        public GetAllViews GetElementsByDataAttribute()
        {
            foreach (var element in AllViewTsObjects)
            {
                var html = new HtmlAgilityPack.HtmlDocument();
                html.LoadHtml(File.ReadAllText(element.ViewPath));
                var elemehtsWithId = html.DocumentNode.Descendants().Where(i => i.Attributes.Any(j => j.Name.StartsWith("data-")));
                foreach (var ele in elemehtsWithId)
                {

                    var e = ele.Attributes.Where(i => i.Name.StartsWith("data-")).Select(i => new ElementByDataAttribute
                    {
                        Attribute = i.Name,
                        Value = i.Value,
                        Name = i.Name.SpaggetyToCamil()
                    });

                    foreach (var ee in e)
                    {
                        //Console.WriteLine($"{ee.Name} = [{ee.Attribute}={ee.Value]");
                        if (!element.ElementsByDataAttribute.Any(i => i.Attribute == ee.Attribute))
                            element.ElementsByDataAttribute.Add(ee);
                    }
                }
            }
            return this;
        }

        public async Task WriteTypescript()
        {
            var Tasks = AllViewTsObjects.Select(i => {
                Directory.CreateDirectory(i.TypescriptPath.Split(Path.DirectorySeparatorChar).ToList().RemoveLast(1).Aggregate((h, j) => h + Path.DirectorySeparatorChar + j));
                return File.WriteAllTextAsync(i.TypescriptPath, i.TypescriptTemplate, Encoding.UTF8);
                });
            await Task.WhenAll(Tasks);
        }
        public async Task WriteTypescriptIfNotExist()
        {
            var Tasks = AllViewTsObjects
                .Where(i => !File.Exists(i.TypescriptPath))
                .Select(i => {
                    Directory.CreateDirectory(i.TypescriptPath.Split(Path.DirectorySeparatorChar).ToList().RemoveLast(1).Aggregate((h, j) => h + Path.DirectorySeparatorChar + j));
                    return File.WriteAllTextAsync(i.TypescriptPath, i.TypescriptTemplate, Encoding.UTF8);
                });
            await Task.WhenAll(Tasks);
        }

        public override string ToString()
        {
            var retString = "";
            foreach(var ob in AllViewTsObjects)
            {
                retString += ob.ToString();
            }
            return retString;
        }
    }
}
