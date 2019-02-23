using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteCore.Helper
{
    public class ResponseMessage
    {
        public bool Status { get; set; }

        public string Code { get; set; }

        public string ReturnMessage { get; set; }

    }

    public class ResponseMessage<T>
    {
        public bool Status { get; set; }

        public string Code { get; set; }

        public string ReturnMessage { get; set; }

        public T ReturnResult { get; set; }
    }
}
