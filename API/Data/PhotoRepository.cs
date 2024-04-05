using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class PhotoRepository : IPhotoRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public PhotoRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Photo> GetPhoto(int id)
    {
        return await _context.Photos.FindAsync(id);
    }

    public async Task<PagedList<PhotoDto>> GetUnapprovedPhotos(PaginationParams paginationParams)
    {
        var photos = _context.Photos
            .Where(x => !x.IsApproved)
            .ProjectTo<PhotoDto>(_mapper.ConfigurationProvider)
            .IgnoreQueryFilters()
            .AsQueryable();

        return await PagedList<PhotoDto>.CreateAsync(photos, paginationParams.PageNumber, paginationParams.PageSize);
    }

    public void RemovePhoto(Photo photo)
    {
        _context.Photos.Remove(photo);
    }
}
