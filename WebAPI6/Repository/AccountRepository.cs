using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebAPI6.Data;
using WebAPI6.Helper;
using WebAPI6.Models;

namespace WebAPI6.Repository
    {
    public class AccountRepository : IAccountRepository
        {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly NIDEC_IOTContext context;

        public AccountRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager,NIDEC_IOTContext context)
            {

            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.roleManager = roleManager;
            this.context=context;
            }
        public async Task<TokenModel> SignInAsync(SignInModel model)
            {
            var user = await userManager.FindByEmailAsync(model.Email);
            var passwoldValid = await userManager.CheckPasswordAsync(user, model.Password);
            if (user == null || !passwoldValid)
                {
                return new TokenModel
                    {
                    Empty = ""
                    };
                }
            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (!result.Succeeded)
                {
                 return new TokenModel
                    {
                    Empty = ""
                    };
                }
            var authClaims = new List<Claim>
                {
                new Claim(ClaimTypes.Email,model.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                };
            var userRole = await userManager.GetRolesAsync(user);
            foreach (var role in userRole)
                {
                authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                }
            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
            var secretKey = configuration["JWT:Secret"];
            Console.WriteLine($"Secret Key Length: {secretKey.Length}");

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:ValidIssuer"],
                audience: configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddSeconds(20),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
                );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
                {
                Id = Guid.NewGuid(),
                JwtId = token.Id,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddHours(1),
                };
            await context.AddAsync(refreshTokenEntity);
            await context.SaveChangesAsync();
            return new TokenModel
                {
                AccessToken= accessToken,
                RefreshToken= refreshToken,
                };
            }

        private string GenerateRefreshToken()
            {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
                {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
                }
            }
        



        public async Task<IdentityResult> SignUpAsync(SignUpModel model)
            {
            var user = new ApplicationUser
                {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email
                };

            var result = await userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
                {
                if (!await roleManager.RoleExistsAsync(AppRole.Customer))
                    {
                    await roleManager.CreateAsync(new IdentityRole(AppRole.Customer));
                    }
                await userManager.AddToRoleAsync(user, AppRole.Customer);
                }
            return result;
            }

        //public async Task<ApiResponse> RenewToken(TokenModel model)
        //    {
        //    try
        //        {
        //        var jwtTokenHandler = new JwtSecurityTokenHandler();
        //        var secretKeyBytes = Encoding.UTF8.GetBytes(configuration["JWT:Secret"]);
        //        var tokenValidateParam = new TokenValidationParameters
        //            {
        //            //tự cấp token
        //            ValidateIssuer = false,
        //            ValidateAudience = false,

        //            //ký vào token
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

        //            ClockSkew = TimeSpan.Zero,

        //            ValidateLifetime = false //ko kiểm tra token hết hạn
        //            };

        //        //check 1: AccessToken valid format
        //        var tokenInVerification = jwtTokenHandler.ValidateToken(model.AccessToken, tokenValidateParam, out var validatedToken);

        //        //check 2: Check alg
        //        if (validatedToken is JwtSecurityToken jwtSecurityToken)
        //            {
        //            var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
        //            if (!result)//false
        //                {
        //                return new ApiResponse
        //                    {
        //                    Success = false,
        //                    Message = "Invalid token"
        //                    };
        //                }
        //            }

        //        //check 3: Check accessToken expire?
        //        var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

        //        var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
        //        if (expireDate > DateTime.UtcNow)
        //            {
        //            return new ApiResponse
        //                {
        //                Success = false,
        //                Message = "Access token has not yet expired"
        //                };
        //            }

        //        //check 4: Check refreshtoken exist in DB
        //        var storedToken = context.RefreshToken.FirstOrDefault(x => x.Token == model.RefreshToken);
        //        if (storedToken == null)
        //            {
        //            return new ApiResponse
        //                {
        //                Success = false,
        //                Message = "Refresh token does not exist"
        //                };
        //            }

        //        //check 5: check refreshToken is used/revoked?
        //        if (storedToken.IsUsed)
        //            {
        //            return new ApiResponse
        //                {
        //                Success = false,
        //                Message = "Refresh token has been used"
        //                };
        //            }
        //        if (storedToken.IsRevoked)
        //            {
        //            return new ApiResponse
        //                {
        //                Success = false,
        //                Message = "Refresh token has been revoked"
        //                };
        //            }

        //        //check 6: AccessToken id == JwtId in RefreshToken
        //        var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
        //        if (storedToken.JwtId != jti)
        //            {
        //            return new ApiResponse
        //                {
        //                Success = false,
        //                Message = "Token doesn't match"
        //                };
        //            }

        //        //Update token is used
        //        storedToken.IsRevoked = true;
        //        storedToken.IsUsed = true;
        //        context.Update(storedToken);
        //        await context.SaveChangesAsync();

        //        //create new token
        //        var user = await context.Users.SingleOrDefaultAsync(nd => nd.Id == storedToken.UserId.ToString());
        //        var token = await SignInAsync(user);

        //        return new ApiResponse
        //            {
        //            Success = true,
        //            Message = "Renew token success",
        //            Data = token
        //            };
        //        }
        //    catch (Exception ex)
        //        {
        //        return new ApiResponse
        //            {
        //            Success = false,
        //            Message = "Something went wrong"
        //            };
        //        }
        //    }
        public async Task<ApiResponse> RenewToken(TokenModel model)
            {
            try
                {
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var secretKeyBytes = Encoding.UTF8.GetBytes(configuration["JWT:Secret"]);
                var tokenValidateParam = new TokenValidationParameters
                    {
                    //tự cấp token
                    ValidateIssuer = false,
                    ValidateAudience = false,

                    //ký vào token
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                    ClockSkew = TimeSpan.Zero,

                    ValidateLifetime = false //ko kiểm tra token hết hạn
                    };

                //check 1: AccessToken valid format
                var tokenInVerification = jwtTokenHandler.ValidateToken(model.AccessToken, tokenValidateParam, out var validatedToken);

                //check 2: Check alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                    {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512Signature, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)//false
                        {
                        return new ApiResponse
                            {
                            Success = false,
                            Message = "Invalid token"
                            };
                        }
                    }

                //check 3: Check accessToken expire?
                var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                if (expireDate > DateTime.UtcNow)
                    {
                    return new ApiResponse
                        {
                        Success = false,
                        Message = "Access token has not yet expired"
                        };
                    }

                //check 4: Check refreshtoken exist in DB
                var storedToken = context.RefreshToken.FirstOrDefault(x => x.Token == model.RefreshToken);
                if (storedToken == null)
                    {
                    return new ApiResponse
                        {
                        Success = false,
                        Message = "Refresh token does not exist"
                        };
                    }

                //check 5: check refreshToken is used/revoked?
                if (storedToken.IsUsed)
                    {
                    return new ApiResponse
                        {
                        Success = false,
                        Message = "Refresh token has been used"
                        };
                    }
                if (storedToken.IsRevoked)
                    {
                    return new ApiResponse
                        {
                        Success = false,
                        Message = "Refresh token has been revoked"
                        };
                    }

                //check 6: AccessToken id == JwtId in RefreshToken
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                    {
                    return new ApiResponse
                        {
                        Success = false,
                        Message = "Token doesn't match"
                        };
                    }

                //Update token is used
                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;
                context.Update(storedToken);
                await context.SaveChangesAsync();

                //create new token
                var user = await userManager.FindByEmailAsync(tokenInVerification.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value);
                var signInModel = new SignInModel { Email = user.Email, Password =user.PasswordHash }; // You need to provide a dummy password here
                var token = await SignInAsync(signInModel);

                return new ApiResponse
                    {
                    Success = true,
                    Message = "Renew token success",
                    Data = token
                    };
                }
            catch (Exception ex)
                {
                return new ApiResponse
                    {
                    Success = false,
                    Message = "Something went wrong"
                    };
                }
            }

        private DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
            {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();

            return dateTimeInterval;
            }

        }
    }
