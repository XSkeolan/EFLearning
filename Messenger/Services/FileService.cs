using Messenger.Interfaces;

namespace Messenger.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;

        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<Guid> UploadFile(IFormFile byteFile)
        {
            if (byteFile.Length == 0)
            {
                throw new ArgumentException(ResponseErrors.FILE_IS_EMPTY);
            }

            var filePath = Path.Combine("D:\\Image", Path.GetRandomFileName());

            using (var stream = File.Create(filePath))
            {
                await byteFile.CopyToAsync(stream);
            }

            MessengerDAL.Models.File file = new MessengerDAL.Models.File
            {
                Server = "http://localhost:5037/",
                Path = filePath
            };

            await _fileRepository.CreateAsync(file);
            return file.Id;
        }
    }
}
