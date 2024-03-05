using System;
using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace ConsoleApp4.Plugin
{
    internal class DateandTimePlugin
    {
        [KernelFunction, Description("Give the Exact Date  as per user Request")]
        public static string Date()
        { 
            return DateTime.Now.ToString("M/d/yyyy");
        }

        [KernelFunction, Description("Give the Exact time as per user Request")]
        public static string Time()
        {
            return  DateTime.Now.TimeOfDay.ToString(); 
        }
    }
}
