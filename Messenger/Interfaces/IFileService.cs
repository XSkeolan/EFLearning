namespace Messenger.Interfaces
{
    public interface IFileService
    {
        Task<IEnumerable<byte[]>> GetMessageAttachments(Guid messageId);
        Task<MessengerDAL.Models.File> SendAttachment(Guid messageId, IFormFile file);
        Task<Guid> UploadFile(IFormFile byteFile);
    }
}