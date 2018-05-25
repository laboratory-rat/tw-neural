using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Error
{
    public class ErrorModel : Exception
    {
        public EnumErrors Type { get; set; }

        public ErrorModel(EnumErrors type, string message) : base(message)
        {
            Type = type;
        }

        public override string ToString()
        {
            string result = $"Type: {Type.ToString()}{Environment.NewLine}Message:{Message}{Environment.NewLine}";
            result += base.ToString();
            return result;
        }

    }

}
