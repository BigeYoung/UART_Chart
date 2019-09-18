using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsartChart
{
    public enum SectionType
    {
        Func,
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
        public bool read { get; set; }
        public string name { get; set; }
        public SectionType type { get; set; }
        public long address { get; set; }
        public UInt16 length { get; set; }
    }
}
