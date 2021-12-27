using System;

namespace ModuleCore
{
    [Serializable]
    public class OutputDocSetting
    {
        public string DocType { get; set; }

        public long MaxSize { get; set; }
    }
}
