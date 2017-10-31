using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace StyleFileCreator.App.Utils
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DoesExistFileNameAttribute : ValidationAttribute
    {
        public string FileFoundMessage = "Sorry but there is already an image with this name please rename your image";
        public DoesExistFileNameAttribute()
            : base("Please enter a name for your image")
        {
        }

        public override bool IsValid(object value)
        {
            if (value != null)
            {
                string fileName = value.ToString();
                if (File.Exists(fileName))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
