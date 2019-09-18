using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsartChart
{
    public enum SectionType
    {
        UNKNOWN,
        INT8,
        INT16,
        INT32,
        UINT8,
        UINT16,
        UINT32,
        Float,
        Double
    }
    public class Section
    {
        public bool Read { get; set; }
        public SectionType Type { get; set; }
        public string Name { get; set; }
        public uint Addr { get; set; }
        public ushort Size { get; set; }
    }
}
