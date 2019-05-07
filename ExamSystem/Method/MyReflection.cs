using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExamSystem.Method
{
    public class MyReflection
    {
        /// <summary>
        /// 调用对象事件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="EventName"></param>
        public static void CallObjectEvent(Object obj, string EventName)
        {
            //建立一个类型，AssemblyQualifiedName拿出有效的名字     
            Type t = Type.GetType(obj.GetType().AssemblyQualifiedName);
            //参数对象      
            object[] p = new object[1];
            //产生方法      
            MethodInfo m = t.GetMethod(EventName, BindingFlags.NonPublic | BindingFlags.Instance);
            //参数赋值。传入函数      
            //获得参数资料  
            ParameterInfo[] para = m.GetParameters();
            //根据参数的名字，拿参数的空值。  
            p[0] = Type.GetType(para[0].ParameterType.BaseType.FullName).GetProperty("Empty");
            //调用      
            m.Invoke(obj, p);
            return;
        }
    }
}
