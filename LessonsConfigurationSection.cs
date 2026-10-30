using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Keytr
{
    public class LessonsConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("lessons")]
        public LessonCollection LessonItems
        {
            get { return (LessonCollection)base["lessons"];
            }
        }
    }

    [ConfigurationCollection(typeof(LessonElement))]
    public class LessonCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new LessonElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LessonElement)element).Path;
        }

        public LessonElement this[int index]
        {
            get
            {
                return (LessonElement)BaseGet(index);
            }
        }
    }

        public class LessonElement : ConfigurationElement
        {
            [ConfigurationProperty("LessonType", DefaultValue = "", IsKey = true, IsRequired = true)]
            public string LessonType
            {
                get
                {
                    return ((string)base["LessonType"]);
                }
                set
                {
                    base["LessonType"] = value;
                }

            }

            [ConfigurationProperty("Path", DefaultValue = "", IsKey = true, IsRequired = true)]
            public string Path
            {
                get
                {
                    return ((string)base["Path"]);
                }

                set
                {
                    base["Path"] = (string)value;
                }
            }
        }
    }

