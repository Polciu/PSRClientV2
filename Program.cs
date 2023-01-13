using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSRClientV2
{
    class Program
    {
        static void Main(string[] args)
        {
            NetworkController nc = new NetworkController();
            nc.connectToServer();
            nc.sendGetMethods();
            nc.receiveMessage();
            List<ClassRMI> methodList = nc.ClassRMI;

            while(true)
            {
                string classChoice = "";
                string methodChoice = "";
                int classId = 0;
                int methodId = 0;

                Console.WriteLine("----------------------------------------------------------------");
                Console.WriteLine("LIST OF METHODS");
                nc.displayMethods();
                Console.WriteLine("----------------------------------------------------------------");

                Console.WriteLine("\n\n---------------------------------------------------------------");
                Console.WriteLine("Wybierz indeks klasy do wywołania.");
                classChoice = Console.ReadLine().ToString();

                if (int.Parse(classChoice) >= nc.ClassRMI.Count && int.Parse(classChoice) < 0) 
                {
                    Console.WriteLine("Niepoprawny indeks.");
                    continue;
                }
                classId = int.Parse(classChoice);

                Console.WriteLine("\n\n----------------------------------------------------------");
                Console.WriteLine("Wybierz indeks metody do wywołania.");
                methodChoice = Console.ReadLine().ToString();

                if(int.Parse(methodChoice) >= nc.ClassRMI[int.Parse(classChoice)].ClassMethods.Count)
                {
                    Console.WriteLine("Niepoprawny indeks.");
                    continue;
                }
                methodId = int.Parse(methodChoice);


                List<string> paramList = new List<string>();
                for(int i = 0; i < methodList[classId].ClassMethods[methodId].ParameterList.Count; i++)
                {
                    Console.WriteLine("------------------------------------------------");
                    Console.WriteLine("Podaj wartość parametru: " + methodList[classId].ClassMethods[methodId].ParameterList[i].ParamName
                        + " (" + methodList[classId].ClassMethods[methodId].ParameterList[i].ParameterType + ")");
                    string param = Console.ReadLine();
                    paramList.Add(param);
                }

                nc.sendMethodInvoke(classId, methodId, paramList);
                nc.receiveMessage();

            }
            


            Console.ReadKey();
        }
    }
}
