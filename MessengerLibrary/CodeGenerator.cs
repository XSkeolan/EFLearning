using MessengerDAL;
using MessengerDAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MessengerLibrary
{
    public class CodeGenerator
    {
        public CodeGenerator()
        {

        }

        public string Generate()
        {
            Random rnd = new Random();
            string generatedCode = string.Empty;

            for (int i = 0; i < 6; i++)
            {
                generatedCode += rnd.Next(0, 10).ToString();
            }

            return generatedCode;
        }

        public async void SetPreviousCodeAsInvalid(Guid userId)
        {
            ConfirmationCode? code = await _messengerContext.ConfirmationCodes.OrderBy(c => c.DateStart).FirstOrDefaultAsync(c => c.UserId == userId);
            if(code != null)
            {
                code.IsUsed = true;
                _messengerContext.SaveChanges();
            }
        }
    }
}