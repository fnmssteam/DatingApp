using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IPhotoRepository
{
    Task<Photo> GetPhoto(int id);
    Task<PagedList<PhotoDto>> GetUnapprovedPhotos(PaginationParams paginationParams);
    void RemovePhoto(Photo photo);
}
