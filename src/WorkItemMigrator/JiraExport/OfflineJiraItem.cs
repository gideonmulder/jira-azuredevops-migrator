using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraExport
{
    public class OfflineJiraItem : IJiraItem
    {
        public string EpicParent { get; set; }

        public string Key { get; set; }

        public string Parent { get; set; }

        public List<JiraRevision> Revisions { get; set; } = new List<JiraRevision>();

        public List<string> SubItems { get; set; } = new List<string>();

        public string Type { get; set; }
    }
}