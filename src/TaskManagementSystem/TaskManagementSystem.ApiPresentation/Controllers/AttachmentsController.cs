using Contracts.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Service.Contract;
using Shared.DataTransferObjects.Attachment;

namespace TaskManagementSystem.ApiPresentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttachmentsController : ControllerBase
{
    private readonly ILoggerManager _loggerManager;
    private readonly IServiceManager _serviceManager;

    public AttachmentsController(ILoggerManager loggerManager, IServiceManager serviceManager)
    {
        _loggerManager = loggerManager;
        _serviceManager = serviceManager;
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile([FromForm] CreateAttachmentDto createAttachment)
    {
        try
        {
            var uploadFileResponse = await _serviceManager.AttachmentService.UploadAttachmentAsync(createAttachment);

            return StatusCode((int)uploadFileResponse.StatusCode, uploadFileResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{Id:int}")]
    public async Task<IActionResult> RemoveAttachment(int Id, bool isSoftDelete = true)
    {
        try
        {
            var removeAttachmentResponse = await _serviceManager.AttachmentService.RemoveAttachment(Id, isSoftDelete);

            return StatusCode((int)removeAttachmentResponse.StatusCode, removeAttachmentResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [NonAction]
    [HttpPost("upload-multiple")]
    public async Task<IActionResult> UploadMultiple([FromForm] IEnumerable<CreateAttachmentDto> createAttachments)
    {
        try
        {
            var uploadMultipleResponse = await _serviceManager.AttachmentService.UploadMultipleAttachments(createAttachments);

            return StatusCode((int)uploadMultipleResponse.StatusCode, uploadMultipleResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("upload-multiple2")]
    public async Task<IActionResult> UploadMultiple2([FromForm] UploadMultipleAttachmentDto createAttachments)
    {
        try
        {
            var uploadMultipleResponse = await _serviceManager.AttachmentService.UploadMultipleAttachments2(createAttachments);

            return StatusCode((int)uploadMultipleResponse.StatusCode, uploadMultipleResponse);
        }
        catch (Exception ex)
        {
            await _loggerManager.LogError(ex, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}
