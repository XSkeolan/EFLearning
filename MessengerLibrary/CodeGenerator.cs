using MessengerDAL;
using MessengerDAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MessengerLibrary
{
    public class CodeGenerator
    {
        private readonly MessengerContext _messengerContext;

        public CodeGenerator(MessengerContext messengerContext)
        {
            _messengerContext = messengerContext;
        }

        private string Generate()
        {
            Random rnd = new Random();
            string generatedCode = string.Empty;

            for (int i = 0; i < 6; i++)
            {
                generatedCode += rnd.Next(0, 10).ToString();
            }

            return generatedCode;
        }

        public async Task<ConfirmationCode> GenerateForUser(Guid userId)
        {
            string hashedCode;
            string generatedCode;

            do
            {
                generatedCode = Generate();
                hashedCode = Password.GetHashedPassword(generatedCode);
            }
            while (_messengerContext.ConfirmationCodes.Any(code => code.Code == hashedCode && !code.IsUsed && !code.IsDeleted));

            ConfirmationCode code = new ConfirmationCode
            {
                Code = hashedCode,
                DateStart = DateTime.UtcNow,
                IsUsed = false,
                UserId = userId
            };

            await _messengerContext.ConfirmationCodes.AddAsync(code);
            await _messengerContext.SaveChangesAsync();

            return code;
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