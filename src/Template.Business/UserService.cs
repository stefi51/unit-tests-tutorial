using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Template.Business.DTOs;
using Template.Domain;
using Template.Domain.Repository;

namespace Template.Business;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;

    public UserService(IMapper mapper, IRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<UserDto?> GetUser(Guid id)
    {
        return await _repository.QueryAsNoTracking<User>()
            .Where(x => x.Id == id)
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserDto>> GetUsers()
    {
        return await _repository.QueryAsNoTracking<User>()
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task CreateUser(CreateUserDto reqDto)
    {
        var user = await _repository.QueryAsNoTracking<User>().SingleOrDefaultAsync(x => x.Email == reqDto.Email);
        if (user != null)
        {
            throw new Exception("User already exists");
        }

        var userEntity = _mapper.Map<User>(reqDto);
        _repository.Add(userEntity);
        await _repository.SaveChangesAsync();
    }

    public async Task DeleteUser(Guid userId)
    {
        var user = await _repository.Query<User>().SingleOrDefaultAsync(x => x.Id == userId);

        if (user is not null)
        {
            _repository.Delete(user);
            await _repository.SaveChangesAsync();
        }
        throw new Exception("User not found");
    }
}