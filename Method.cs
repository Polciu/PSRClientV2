using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSRClientV2
{
    class Method
    {
        String methodName;
        String returnName;
        String memberType;
        List<Parameter> parameterList;

        public String MethodName
        {
            get => methodName;
            set => methodName = value;
        }

        public String ReturnName
        {
            get => returnName;
            set => returnName = value;
        }

        public String MemberType
        {
            get => memberType;
            set => memberType = value;
        }

        public List<Parameter> ParameterList
        {
            get => parameterList;
            set => parameterList = value;
        }
    }
}
