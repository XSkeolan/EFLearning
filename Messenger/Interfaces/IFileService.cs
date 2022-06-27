namespace Messenger.Interfaces
{
    public interface IFileService
    {
        Task<Guid> UploadFile(IFormFile byteFile);
    }
}