using AutoMapper;
using WellTrack.Services.Identity.Application.DTOs;
using WellTrack.Services.Identity.Domain.Entities;
using WellTrack.Services.Identity.Repositories;
using WellTrack.SharedKernel.Exceptions;
using WellTrack.SharedKernel.Wrappers;
using WellTrack.SharedKernel.Enum;

namespace WellTrack.Services.Identity.Application.Services
{
	public class UserService : IUserService
	{
		private readonly IMapper mapper;
		private readonly IUserRepository userRepository;

		public UserService(IMapper mapper,IUserRepository userRepository)
		{
			this.mapper = mapper;
			this.userRepository = userRepository;
		}

		public async Task ChangePasswordAsync(Guid id, ChangePasswordDto dto)
		{
			var user = await userRepository.GetByIdAsync(id);

			if(user==null){
				throw new NotFoundException(nameof(user), id);
			}

			if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash)){
				throw new ValidationException(new Dictionary<string, string[]>
				{
					{ "CurrentPassword", new[] { "Current password is incorrect" } }
				});
			}

			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

			await userRepository.UpdateAsync(user);
		}

		public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
		{
			var existingUser = await userRepository.GetByEmailAsync(createUserDto.Email);
			if(existingUser!=null){
				throw new ValidationException(new Dictionary<string, string[]>
				{
					{ "Email", new[] { "Email already exists" } }
				});
			}

			var user = mapper.Map<User>(createUserDto);
			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
			user.Status = StatusType.Active;

			user=await userRepository.AddAsync(user);

			return mapper.Map<UserDto>(user);
		}

		public async Task DeactivateUserAsync(Guid id)
		{
			var user = await userRepository.GetByIdAsync(id);
			if(user==null){

				throw new NotFoundException(nameof(user), id);

			}
			user.Status = StatusType.Inactive;

			await userRepository.UpdateAsync(user);

		}

		public async Task ActiveUserAsync(Guid id){
			var user = await userRepository.GetByIdAsync(id);

			if(user==null){
				throw new NotFoundException(nameof(user), id);
			}

			user.Status = StatusType.Active;

			await userRepository.UpdateAsync(user);
		}

		public async Task DeleteUserAsync(Guid id)
		{
			var user = await userRepository.GetByIdAsync(id);

			if(user==null){
				throw new NotFoundException(nameof(user), id);
			}

			await userRepository.DeleteAsync(user);

		}

		public async Task<UserDto> GetUserByIdAsync(Guid id)
		{
			var user = await userRepository.GetByIdAsync(id);

			if(user==null){
				throw new NotFoundException(nameof(user), id);
			}

			var userDto = mapper.Map<UserDto>(user);
			return userDto;
		}

		public async Task<PagedResult<UserDto>> GetUsersAsync(int page, int pageSize, string? role)
		{
			var users = await userRepository.GetAllAsync();

			if (!string.IsNullOrEmpty(role))
			{
				users = users.Where(u => u.Role.ToString() == role);
			}

			var totalCount = users.Count();
			var skipPages = (page - 1) * pageSize;

			var pagedUsers = users.Skip(skipPages).Take(pageSize).ToList();

			var userDtos = mapper.Map<List<UserDto>>(pagedUsers);

			
			var result= new PagedResult<UserDto>()
			{
				Items = userDtos,
				TotalCount = totalCount,
				Page = page,
				PageSize = pageSize
			};

			return result;
		}

		public async Task<UserDto> UpdateProfileAsync(Guid id, UpdateProfileDto dto)
		{
			var user = await userRepository.GetByIdAsync(id);

			if(user==null){
				throw new NotFoundException(nameof(user), id);
			}

			if (!string.IsNullOrWhiteSpace(dto.Name)) user.Name = dto.Name;
			if (!string.IsNullOrWhiteSpace(dto.Phone)) user.Phone = dto.Phone;

			await userRepository.UpdateAsync(user);

			var userDto = mapper.Map<UserDto>(user);
			return userDto;
		}

		public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
		{
			var user = await userRepository.GetByIdAsync(id);

			if (user == null)
			{
				throw new NotFoundException(nameof(user), id);
			}

			// ✅ Update properties in place — keeps Id, Email, PasswordHash, CreatedAt intact
			user.Name = updateUserDto.Name;
			user.Phone = updateUserDto.Phone;
			user.Role = updateUserDto.Role;
			user.Status = updateUserDto.Status;

			await userRepository.UpdateAsync(user);

			return mapper.Map<UserDto>(user);
		}

	}
}
