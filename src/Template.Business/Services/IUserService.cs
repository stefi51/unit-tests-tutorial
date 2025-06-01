using Template.Business.DTOs;

namespace Template.Business.Services;

public interface IUserService
{
        Task<UserDto?> GetUser(Guid id);
        Task<IEnumerable<UserDto>> GetUsers();
        Task CreateUser(CreateUserDto user);
        Task DeleteUser(Guid userId);
        
        Task UpdateName(Guid userId, string name);
}