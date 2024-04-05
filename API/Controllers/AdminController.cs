using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _uow;

    public AdminController(UserManager<AppUser> userManager, IUnitOfWork uow)
    {
        _userManager = userManager;
        _uow = uow;
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await _userManager.Users
            .OrderBy(u => u.UserName)
            .Select(u => new 
            {
                u.Id,
                Username = u.UserName,
                Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
            })
            .ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, [FromQuery]string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role.");

        var selectedRoles = roles.Split(",").ToArray();

        var user = await _userManager.FindByNameAsync(username);

        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);

        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (!result.Succeeded) return BadRequest("Failed to add to roles");

        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if (!result.Succeeded) return BadRequest("Failed to remove from roles");

        return Ok(await _userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult<PagedList<PhotoDto>>> GetPhotosForModeration([FromQuery]PaginationParams paginationParams)
    {
        var photos = await _uow.PhotoRepository.GetUnapprovedPhotos(paginationParams);

        Response.AddPaginationHeader(new PaginationHeader(photos.CurrentPage, photos.PageSize, photos.TotalCount, photos.TotalPages));

        return photos;
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("approve-photo/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        var photo = await _uow.PhotoRepository.GetPhoto(photoId);

        if (photo == null) return NotFound();

        if (photo.IsApproved) return BadRequest("Photo is already approved");

        photo.IsApproved = true;

        var user = await _uow.UserRepository.GetUserByIdAsync(photo.AppUserId);

        if(!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;

        if (await _uow.Complete()) return Ok();

        return BadRequest("Failed to approve photo");
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("reject-photo/{photoId}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        var photo = await _uow.PhotoRepository.GetPhoto(photoId);

        if (photo == null) return NotFound();

        if (photo.IsApproved) return BadRequest("You cannot reject a photo that's already approved");

        _uow.PhotoRepository.RemovePhoto(photo);

        if (await _uow.Complete()) return Ok();

        return BadRequest("Failed to reject photo");
    }
}
