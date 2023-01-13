using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSRClientV2
{
    class ClassRMI
    {
        String className;
        List<Method> methodList;


        public String ClassName
        {
            get => className;
            set => className = value;
        }

        public List<Method> ClassMethods
        {
            get => methodList;
            set => methodList = value;
        }
    }
}
