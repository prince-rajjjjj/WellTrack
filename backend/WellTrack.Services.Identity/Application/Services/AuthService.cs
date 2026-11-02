using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;// Added for SecureSocketOptions
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WellTrack.Services.Identity.Application.DTOs;
using WellTrack.Services.Identity.Domain.Entities;
using WellTrack.Services.Identity.Repositories;
using WellTrack.SharedKernel.Enum;
using WellTrack.SharedKernel.Exceptions;
using WellTrack.SharedKernel.HttpClients;
using WellTrack.SharedKernel.Wrappers;

namespace WellTrack.Services.Identity.Application.Services
{
	public class AuthService : IAuthService
	{
		private readonly IConfiguration configuration;
		private readonly IMapper mapper;
		private readonly IUserRepository userRepository;
		private readonly ILogAuditService logAuditService;
		private readonly INotificationClient notify;

		public AuthService(IConfiguration configuration, IMapper mapper, IUserRepository userRepository, ILogAuditService auditService, INotificationClient notify)
		{
			this.configuration = configuration;
			this.mapper = mapper;
			this.userRepository = userRepository;
			this.logAuditService = auditService;
			this.notify = notify;
		}

		public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
		{
			var user = await userRepository.GetByEmailAsync(loginDto.Email);

			if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
			{
				throw new UnauthorizedAccessException("Invalid email or password");
			}

			if (user.Status != StatusType.Active)
			{
				throw new ForbiddenException("User account is not active");
			}

			var token = GenerateJwtToken(user);
			var expiresAt = DateTime.UtcNow.AddHours(8);

			await logAuditService.CreateLogAsync(user.Id, "Login", "Auth");

			return new LoginResponseDto()
			{
				Token = token,
				Email = user.Email,
				Name = user.Name,
				Role = user.Role.ToString(),
				ExpiresAt = expiresAt
			};
		}

		public async Task LogoutAsync(Guid id)
		{
			var user = await userRepository.GetByIdAsync(id);

			if (user == null)
			{
				throw new UnauthorizedAccessException("Login First");
			}

			await logAuditService.CreateLogAsync(user.Id, "Logout", "Auth");
		}

		public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
		{
			var user = await userRepository.GetByIdAsync(userId);

			if (user == null)
			{
				throw new NotFoundException(nameof(user), userId);
			}

			if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
			{
				throw new UnauthorizedAccessException("Current password is incorrect");
			}

			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);

			await userRepository.UpdateAsync(user);
			await logAuditService.CreateLogAsync(user.Id, "Change Password", "Auth");

			await notify.NotifyAsync(
					userId,
					"Your password was changed successfully",
					"Security");
		}


		public async Task<UserDto> GetProfileAsync(Guid userId)
		{
			var user = await userRepository.GetByIdAsync(userId);

			if (user == null)
			{
				throw new NotFoundException(nameof(user), userId);
			}

			return mapper.Map<UserDto>(user);
		}

		private string GenerateJwtToken(User user)
		{
			var jwtKey = configuration["Jwt:Key"] ?? "YourSuperSecretKeyForJWTTokenGeneration12345";
			var jwtIssuer = configuration["Jwt:Issuer"] ?? "WellTrack";
			var jwtAudience = configuration["Jwt:Audience"] ?? "WellTrackUsers";

			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim(ClaimTypes.Role, user.Role.ToString()),
				new Claim(ClaimTypes.Name, user.Name),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			var token = new JwtSecurityToken(
				issuer: jwtIssuer,
				audience: jwtAudience,
				claims: claims,
				expires: DateTime.UtcNow.AddHours(8),
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}