using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PSRClientV2
{

    internal class NetworkController
    {
        static TcpClient client = new TcpClient();
        NetworkStream networkStream = null;
        List<ClassRMI> rmiList = new List<ClassRMI>();

        public void connectToServer()
        {
            try
            {
                Console.WriteLine("Connecting...");
                client.Connect("127.0.0.1", 55001);
                networkStream = client.GetStream();

            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e.StackTrace);
            }
            finally
            {
                //client.Close();
                //Console.WriteLine("Exit...");
            }
           
        }

        public void sendGetMethods()
        {
            JObject obj = new JObject();
            obj["Type"] = "methods";


            Byte[] bytes = Encoding.ASCII.GetBytes(obj.ToString());

            sendMessage(bytes);
        }

        public void sendMethodInvoke(int classId, int methodId, List<string> methodParams)
        {
            string className = rmiList[classId].ClassName;
            string method = rmiList[classId].ClassMethods[methodId].MethodName;

            JObject json = new JObject();
            json["Type"] = "invoke";
            json["Class"] = className;
            json["Method"] = method;

            JArray paramArray = new JArray();
            for (int i = 0; i < methodParams.Count; i++)
            {
                JObject paramObj = new JObject();
                paramObj["ParamName"] = rmiList[classId].ClassMethods[methodId].ParameterList[i].ParamName;
                paramObj["ParamValue"] = methodParams[i];
                paramArray.Add(paramObj);
            }

            json["Params"] = paramArray;

            Byte[] bytes = Encoding.ASCII.GetBytes(json.ToString());
            sendMessage(bytes);

        }

        public void sendMessage(Byte[] bytes)
        {
            //Console.WriteLine(System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length));
            networkStream.Write(bytes, 0, bytes.Length);
            networkStream.Flush();
        }

        public void receiveMessage()
        {
            //List<ClassRMI> rmiList = new List<ClassRMI>();
            NetworkStream stream = client.GetStream();
            Byte[] data = new byte[7000];
            stream.Read(data, 0, data.Length);


            //Console.WriteLine(System.Text.Encoding.UTF8.GetString(data, 0, data.Length));
            String receivedData = System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
            JObject json = JObject.Parse(receivedData);

            if(json.ContainsKey("classes"))
            {
                rmiList = deserializeMethodList(json);
                
                for(int i = 0; i < rmiList.Count; i++)
                {
                    Console.WriteLine(".........................");
                    Console.WriteLine("INDEX: " + i);
                    Console.WriteLine("CLASS NAME: " + rmiList[i].ClassName);
                    //Console.WriteLine(rmiList[i].ClassName);

                    for(int j = 0; j < rmiList[i].ClassMethods.Count; j++)
                    {
                        Console.WriteLine(j + " METHOD DATA:");
                        Console.WriteLine("Method name: " + rmiList[i].ClassMethods[j].MethodName);
                        Console.WriteLine("Member type: " + rmiList[i].ClassMethods[j].MemberType);
                        Console.WriteLine("Return name: " + rmiList[i].ClassMethods[j].ReturnName);

                        for(int k = 0; k < rmiList[i].ClassMethods[j].ParameterList.Count; k++)
                        {
                            Console.WriteLine("PARAMETER DATA:");
                            Console.Write("Param name: " + rmiList[i].ClassMethods[j].ParameterList[k].ParamName);
                            Console.Write("Metadata token: " + rmiList[i].ClassMethods[j].ParameterList[k].MetadataToken);
                            Console.Write("Parameter type: " + rmiList[i].ClassMethods[j].ParameterList[k].ParameterType);
                            Console.Write("Param position: " + rmiList[i].ClassMethods[j].ParameterList[k].ParamPosition);
                            Console.Write("Param member: " + rmiList[i].ClassMethods[j].ParameterList[k].ParamMember);
                        }
                    }
                }
            }



            if(json.ContainsKey("Result"))
            {
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine("Method invoked.");
                Console.WriteLine("Result: " + json["Result"].ToString());
                Console.WriteLine("---------------------------------------------");
            }
        }

        public void displayMethods()
        {
            for (int i = 0; i < rmiList.Count; i++)
            {
                Console.WriteLine("INDEX: " + i);
                Console.WriteLine("CLASS NAME: " + rmiList[i].ClassName);
                //Console.WriteLine(rmiList[i].ClassName);

                for (int j = 0; j < rmiList[i].ClassMethods.Count; j++)
                {
                    Console.WriteLine(j + " METHOD DATA:");
                    Console.WriteLine("Method name: " + rmiList[i].ClassMethods[j].MethodName);
                    Console.WriteLine("Member type: " + rmiList[i].ClassMethods[j].MemberType);
                    Console.WriteLine("Return name: " + rmiList[i].ClassMethods[j].ReturnName);

                    for (int k = 0; k < rmiList[i].ClassMethods[j].ParameterList.Count; k++)
                    {
                        Console.WriteLine("PARAMETER DATA:");
                        Console.WriteLine("Param name: " + rmiList[i].ClassMethods[j].ParameterList[k].ParamName);
                        Console.WriteLine("Metadata token: " + rmiList[i].ClassMethods[j].ParameterList[k].MetadataToken);
                        Console.WriteLine("Parameter type: " + rmiList[i].ClassMethods[j].ParameterList[k].ParameterType);
                        Console.WriteLine("Param position: " + rmiList[i].ClassMethods[j].ParameterList[k].ParamPosition);
                        Console.WriteLine("Param member: " + rmiList[i].ClassMethods[j].ParameterList[k].ParamMember + "\n");
                    }
                }
            }
        }

        public List<ClassRMI> deserializeMethodList(JObject json)
        {
            List<ClassRMI> rmiList = new List<ClassRMI>();

            JArray objArray = new JArray();
            objArray = (JArray)json["classes"];
            
            for(int i = 0; i < objArray.Count; i++)
            {
                ClassRMI rmiObj = new ClassRMI();
                rmiObj.ClassMethods = new List<Method>();
                //JObject classObj = (JObject)objArray[i]["name"];
                //JArray parameterArray = (JArray)objArray[i]["params"];
                String className = objArray[i]["name"].ToString();
                JArray methodArray = (JArray)objArray[i]["methodList"];

                rmiObj.ClassName = className;

                for (int k = 0; k < methodArray.Count; k++)
                {
                    Method classMethod = new Method();
                    List<Parameter> methodParams = new List<Parameter>();
                    String methodName = methodArray[k]["methodName"].ToString();
                    String returnName = methodArray[k]["returnType"].ToString();
                    String memberType = methodArray[k]["memberType"].ToString();

                    classMethod.MethodName = methodName;
                    classMethod.ReturnName = returnName;
                    classMethod.MemberType = memberType;


                    JArray parameterArray = (JArray) methodArray[k]["params"];

                    for (int j = 0; j < parameterArray.Count; j++)
                    {
                        String paramName = parameterArray[j]["paramName"].ToString();
                        String paramMember = parameterArray[j]["paramMember"].ToString();
                        String metadataToken = parameterArray[j]["metadataToken"].ToString();
                        String parameterType = parameterArray[j]["parameterType"].ToString();
                        String paramPosition = parameterArray[j]["paramPosition"].ToString();

                        Parameter param = new Parameter();
                        param.ParamName = paramName;
                        param.ParamMember = paramMember;
                        param.MetadataToken = metadataToken;
                        param.ParameterType = parameterType;
                        param.ParamPosition = paramPosition;

                        methodParams.Add(param);
                        
                    }

                    classMethod.ParameterList = methodParams;
                    rmiObj.ClassMethods.Add(classMethod);
                }
                rmiList.Add(rmiObj);
            }

            return rmiList;
        }


        public List<ClassRMI> ClassRMI
        {
            get => rmiList;
            set => rmiList = value;
        }

    }
}
