using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Template.Business.DTOs;
using Template.Business.ExternalServices;
using Template.Domain;
using Template.Domain.Repository;

namespace Template.Business.Services;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly IPaymentService _paymentService;

    public UserService(IMapper mapper, IRepository repository, IPaymentService paymentService)
    {
        _mapper = mapper;
        _repository = repository;
        _paymentService = paymentService;
    }

    public async Task<UserDto?> GetUser(Guid id)
    {
        var user = await _repository.QueryAsNoTracking<User>()
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();

        return _mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<UserDto>> GetUsers()
    {
        var users = await _repository.QueryAsNoTracking<User>()
            .ToListAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task CreateUser(CreateUserDto reqDto)
    {
        var user = await _repository.QueryAsNoTracking<User>()
            .SingleOrDefaultAsync(x => x.Email == reqDto.Email);
        if (user != null)
        {
            throw new Exception($"User with email:{reqDto.Email} already exists");
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
            // third party service
            var hasPendingPayments = await _paymentService.HasPendingPayments(user.Email);
            if (hasPendingPayments)
            {
                throw new Exception($"User with email:{user.Email} has pending payments");
            }
            _repository.Delete(user);
            _repository.SaveChangesAsync();
        }
        else
        {
            throw new Exception($"User with UserId:{userId} does not exist.");
        }
    }

    public async Task UpdateName(Guid userId, string name)
    {
        var user = await _repository.Query<User>().SingleOrDefaultAsync(x => x.Id == userId);

        if (user is not null)
        {
            user.Name = name;
            _repository.Update(user);
            _repository.SaveChangesAsync();
        }
        else
        {
            throw new Exception($"User with UserId:{userId} does not exist.");
        }
    }
}