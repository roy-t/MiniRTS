using System.Collections.Generic;
using System.IO;

namespace ModelExtension
{
    // Quick hack to get rid of dependency, TODO: clean-up and rewrite!

    public sealed class IniParser
    {
        public IniData ReadFile(string file)
        {
            var data = new IniData();

            Section currentSection = null;
            foreach (var line in File.ReadAllLines(file))
            {
                if (line.StartsWith(';'))
                    continue;

                var sectionParts = line.Split('[', ']');
                if (sectionParts.Length == 3)
                {
                    if (currentSection != null)
                    {
                        data.Sections.Add(currentSection.Key, currentSection);
                    }

                    currentSection = new Section(sectionParts[1]);
                }
                else
                {
                    var propertyParts = line.Split('=');
                    if (propertyParts.Length == 2)
                    {
                        currentSection.Properties.Add(propertyParts[0], propertyParts[1]);
                    }
                }
            }

            if (currentSection != null)
            {
                data.Sections.Add(currentSection.Key, currentSection);
            }

            return data;
        }
    }


    public sealed class IniData
    {
        public IniData()
        {
            this.Sections = new Dictionary<string, Section>();
        }

        public Dictionary<string, Section> Sections { get; }
    }

    public sealed class Section
    {
        public Section(string key)
        {
            this.Key = key;
            this.Properties = new Dictionary<string, string>();
        }

        public string Key { get; }

        public Dictionary<string, string> Properties { get; }
    }
}
