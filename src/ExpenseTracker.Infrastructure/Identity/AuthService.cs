using ExpenseTracker.Application.Auth;
using ExpenseTracker.Application.Common.Exceptions;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Infrastructure.Persistence;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Infrastructure.Identity
{
    public class AuthService(UserManager<AppUser> userManager, AppDbContext db, JwtTokenGenerator tokenGenerator, IValidator<RegisterRequest> registerValidator, IValidator<LoginRequest> loginValidator) : IAuthService
    {
        private const string InvalidCredentials = "Invalid email or password.";

        private static readonly (string Name, string Color)[] DefaultCategories =
            [
                ("Food", "#FF7043"),
                ("Transport", "#42A5F5"),
                ("Bills", "#AB47BC"),
                ("Shopping", "#EC407A"),
                ("Entertainment", "#FFCA28"),
                ("Health", "#66BB6A")
            ];

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
        {
            await registerValidator.ValidateAndThrowAsync(request, ct);

            if (await userManager.FindByEmailAsync(request.Email) is not null)
            {
                throw new ConflictException("An account with this email already exists.");
            }

            var user = new AppUser
            {
                Id = Guid.CreateVersion7(),
                UserName = request.Email,
                Email = request.Email
            };

            // The user and their default categories are saved together: both succeed, or neither does
            await using var transaction = await db.Database.BeginTransactionAsync(ct);

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                // Turn Identity's errors (e.g. "Passwords must have atleast one digit") into a normal 400
                throw new ValidationException(result.Errors.Select(error => new ValidationFailure(error.Code.StartsWith("Password") ? nameof(request.Password) : nameof(request.Email), error.Description)));
            }

            db.Categories.AddRange(DefaultCategories.Select(c => new Category(user.Id, c.Name, c.Color)));
            await db.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);

            return CreateResponse(user);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
        {
            await loginValidator.ValidateAndThrowAsync(request, ct);

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                throw new UnauthorizedException(InvalidCredentials);
            }

            if (await userManager.IsLockedOutAsync(user))
            {
                throw new UnauthorizedException("Too many failed attempts. Try again in a few minutes.");
            }

            if (!await userManager.CheckPasswordAsync(user, request.Password))
            {
                await userManager.AccessFailedAsync(user);         // Counts towards lockout.
                throw new UnauthorizedException(InvalidCredentials);
            }

            await userManager.ResetAccessFailedCountAsync(user);

            return CreateResponse(user);
        }

        private AuthResponse CreateResponse(AppUser user)
        {
            var (token, expiresAtUtc) = tokenGenerator.Generate(user);
            return new AuthResponse(token, expiresAtUtc, user.Id, user.Email!);
        }
    }
}
