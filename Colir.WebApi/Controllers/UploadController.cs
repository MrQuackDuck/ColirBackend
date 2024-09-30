using Colir.BLL.Interfaces;
using Colir.BLL.RequestModels.Attachment;
using Colir.Communication.Enums;
using Colir.Communication.RequestModels.Upload;
using Colir.Communication.ResponseModels;
using Colir.Exceptions.NotEnoughPermissions;
using Colir.Exceptions.NotFound;
using Colir.Interfaces.ApiRelatedServices;
using Colir.Interfaces.Controllers;
using Colir.Misc.ExtensionMethods;
using DAL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Colir.Controllers;

[ApiController]
[Authorize]
[Route("API/[controller]/[action]")]
public class UploadController : ControllerBase, IUploadController
{
    private readonly IAttachmentService _attachmentService;
    private readonly IUnitOfWork _unitOfWork;

    public UploadController(IAttachmentService attachmentService, IUnitOfWork unitOfWork)
    {
        _attachmentService = attachmentService;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc cref="IUploadController.UploadAttachments"/>
    [HttpPost]
    [RequestFormLimits(MultipartBodyLengthLimit = 120_000_000)]
    [RequestSizeLimit(120_000_000)]
    public async Task<ActionResult<List<long>>> UploadAttachments([FromForm] UploadAttachmentsModel model)
    {
        try
        {
            var totalFilesSize = model.Files.Sum(file => file.Length);
            var roomFreeStorage = _unitOfWork.RoomRepository.RoomFileManager.GetFreeStorageSize(model.RoomGuid);

            if (totalFilesSize > roomFreeStorage)
            {
                throw new ArgumentException("Not enough space in the room!");
            }

            // Upload the attachments
            var listOfAttachmentIds = new List<long>();
            foreach (var file in model.Files)
            {
                var request = new RequestToUploadAttachment
                {
                    IssuerId = this.GetIssuerId(),
                    RoomGuid = model.RoomGuid,
                    File = file
                };

                var attachment = await _attachmentService.UploadAttachmentAsync(request);
                listOfAttachmentIds.Add(attachment.Id);
            }

            // Run the task that will check if these attachments are actually attached to any message after some time
            _ = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                foreach (var attachmentId in listOfAttachmentIds)
                {
                    try
                    {
                        // If the attachment is not attached to any message, delete it
                        if (!await _attachmentService.CheckIfAttachmentIsAttachedToAnyMessageAsync(attachmentId))
                        {
                            _unitOfWork.RoomRepository.RoomFileManager.DeleteFile(
                                (await _unitOfWork.AttachmentRepository.GetByIdAsync(attachmentId)).Path);

                            await _unitOfWork.AttachmentRepository.DeleteByIdAsync(attachmentId);
                        }
                    }
                    catch (AttachmentNotFoundException) { /* ignored */ }
                }
            });

            return Ok(listOfAttachmentIds);
        }
        catch (ArgumentException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.NotEnoughSpace));
        }
        catch (RoomNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.RoomNotFound));
        }
        catch (UserNotFoundException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.UserNotFound));
        }
        catch (IssuerNotInRoomException)
        {
            return BadRequest(new ErrorResponse(ErrorCode.IssuerNotInTheRoom));
        }
    }
}