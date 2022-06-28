using Messenger.Interfaces;
using MessengerDAL.Models;
using Microsoft.Extensions.Options;

namespace Messenger.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IMessageFileRepository _messageFileRepository;
        private readonly string _filesPath;

        public FileService(IOptions<Options.FileOptions> options, IFileRepository fileRepository, IMessageFileRepository messageFileRepository)
        {
            _filesPath = options.Value.StoredFilesPath;
            _fileRepository = fileRepository;
            _messageFileRepository = messageFileRepository;
        }

        public async Task<Guid> UploadFile(IFormFile byteFile)
        {
            if (byteFile.Length == 0)
            {
                throw new ArgumentException(ResponseErrors.FILE_IS_EMPTY);
            }

            var filePath = Path.Combine("D:\\Image", Path.GetRandomFileName());

            using (var stream = System.IO.File.Create(filePath))
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

        public async Task<MessengerDAL.Models.File> SendAttachment(Guid messageId, IFormFile file)
        {
            if (file.Length == 0)
            {
                throw new ArgumentException(ResponseErrors.FILE_IS_EMPTY);
            }

            var filePath = Path.Combine(_filesPath, Path.GetRandomFileName());

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            MessengerDAL.Models.File newFile = new MessengerDAL.Models.File
            {
                Server = "http://localhost:5037/",
                Path = filePath
            };
            await _fileRepository.CreateAsync(newFile);

            MessageFile messageFile = new MessageFile
            {
                MessageId = messageId,
                FileId = newFile.Id
            };
            await _messageFileRepository.CreateAsync(messageFile);

            return newFile;
        }

        public async Task<IEnumerable<byte[]>> GetMessageAttachments(Guid messageId)
        {
            List<Guid> filesId = new List<Guid>((await _messageFileRepository.GetMessageFiles(messageId)).Select(x => x.FileId));
            List<byte[]> files = new List<byte[]>();
            foreach (Guid fileId in filesId)
            {
                MessengerDAL.Models.File file = await _fileRepository.FindByIdAsync(fileId) ?? throw new InvalidOperationException(ResponseErrors.FILE_NOT_FOUND);
                using (var memoryStream = new MemoryStream())
                {
                    System.IO.File.OpenRead(file.Path).CopyTo(memoryStream);
                    files.Add(memoryStream.ToArray());
                }
            }
            return files;
        }
    }
}
