using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace JiraExport
{
    public interface IJiraItem
    {
        string EpicParent { get; }

        string Key { get; }

        string Parent { get; }

        List<JiraRevision> Revisions { get; set; }

        List<string> SubItems { get; }

        string Type { get; }
    }
}