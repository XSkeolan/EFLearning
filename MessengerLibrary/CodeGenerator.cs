using MessengerDAL;
using MessengerDAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MessengerLibrary
{
    public class CodeGenerator
    {
        public static string Generate()
        {
            Random rnd = new Random();
            string generatedCode = string.Empty;

            for (int i = 0; i < 6; i++)
            {
                generatedCode += rnd.Next(0, 10).ToString();
            }

            return generatedCode;
        }
    }
}