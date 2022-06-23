using MessengerDAL;
using MessengerDAL.Models;
using Messenger.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Messenger.Options;

namespace Messenger.Repositories
{
    public class ConfirmationCodeRepository : BaseRepository<ConfirmationCode>, IConfirmationCodeRepository
    {
        private readonly TimeSpan _validTime;
        public ConfirmationCodeRepository(MessengerContext context, IOptions<CodeOptions> codeOptions) : base(context)
        {
            _validTime = TimeSpan.FromSeconds(codeOptions.Value.ValidCodeTime);
        }

        public Task<ConfirmationCode?> GetUnsedCodeByUser(Guid userId)
        {
            return null!;
        }

        public Task<IEnumerable<ConfirmationCode>> GetUnusedValidCode()
        {
            return GetByConditions(c => !c.IsUsed && !c.IsDeleted && c.DateStart >= DateTime.UtcNow.Add(-_validTime));
        }

        public async Task<bool> UnUsedCodeExists(string code)
        {
            return await _dbSet.AnyAsync(c => c.Code == code && !c.IsDeleted && !c.IsUsed);
        }
    }
}
