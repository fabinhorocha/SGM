using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tenaris.Confab.SAP.Connector
{
    public class RFC_DB_FLD
    {
        public string FIELDNAME { get; set; }

        public int OFFSET { get; set; }

        public int LENGTH { get; set; }

        public INTTYPE TYPE { get; set; }

        public string FIELDTEXT { get; set; }
    }

    public enum INTTYPE
    {
        C = 'C', // Character String
        N = 'N', // Character String with Digits Only
        D = 'D', // Date (Date: YYYYMMDD)
        T = 'T'  // Time (Time: HHMMSS)
    }
}
