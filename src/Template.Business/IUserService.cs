using Template.Business.DTOs;

namespace Template.Business;

public interface IUserService
{
        Task<UserDto?> GetUser(Guid id);
        Task<IEnumerable<UserDto>> GetUsers();
        Task CreateUser(CreateUserDto user);
        Task DeleteUser(Guid userId);
}