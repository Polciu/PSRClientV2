using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSRClientV2
{
    class Parameter
    {
        String paramName;
        String paramMember;
        String metadataToken;
        String parameterType;
        String paramPosition;

        public String ParamName
        {
            get => paramName;
            set => paramName = value;
        }

        public String ParamMember
        {
            get => paramMember;
            set => paramMember = value;
        }

        public String MetadataToken
        {
            get => metadataToken;
            set => metadataToken = value;
        }

        public String ParameterType
        {
            get => parameterType;
            set => parameterType = value;
        }

        public String ParamPosition
        {
            get => paramPosition;
            set => paramPosition = value;
        }

    }
}
