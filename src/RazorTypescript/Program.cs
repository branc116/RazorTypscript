using System;
using System.Threading.Tasks;

namespace RazorTypescript
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await new GetAllViews(args[0], args[1])
                .Scan()
                .CreateRelations()
                .GetElemenentsWithId()
                .GetElementsByDataAttribute()
                .WriteTypescript();
        }
    }
}
