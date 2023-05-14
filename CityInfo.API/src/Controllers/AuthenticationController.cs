using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CityInfo.API.src.Controllers
{
    /// <summary>
    /// Controller for handling authentication operations.
    /// </summary>
    [Route("api/v{version:apiVersion}/authentication")]
    public class AuthenticationController : ControllerBase
    {
        /// <summary>
        /// Represents the request body for authentication.
        /// </summary>
        public class AuthenticationRequestBody
        {
            /// <summary>
            /// Gets or sets the user name.
            /// </summary>
            public string UserName { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the password.
            /// </summary>
            public string Password { get; set; } = string.Empty;
        }

        /// <summary>
        /// Represents the CityInfo API user.
        /// </summary>
        private class CityInfoAPIUser
        {
            /// <summary>
            /// Gets or sets the user ID.
            /// </summary>
            public int UserId { get; set; }

            /// <summary>
            /// Gets or sets the user name.
            /// </summary>
            public string UserName { get; set; } = string.Empty;

            /// <summary>
            /// Gets or sets the first name.
            /// </summary>
            public string? FirstName { get; set; }

            /// <summary>
            /// Gets or sets the last name.
            /// </summary>
            public string? LastName { get; set; }

            /// <summary>
            /// Gets or sets the city.
            /// </summary>
            public string? City { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="CityInfoAPIUser"/> class.
            /// </summary>
            /// <param name="userId">The user ID.</param>
            /// <param name="userName">The user name.</param>
            /// <param name="firstName">The first name.</param>
            /// <param name="lastName">The last name.</param>
            /// <param name="city">The city.</param>
            public CityInfoAPIUser(int userId, string userName, string firstName, string lastName, string city)
            {
                UserId = userId;
                UserName = userName;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="CityInfoAPIUser"/> class.
            /// </summary>
            /// <param name="userId">The user ID.</param>
            /// <param name="userName">The user name.</param>
            public CityInfoAPIUser(int userId, string userName)
            {
                UserId = userId;
                UserName = userName;
            }
        }

        private readonly ILogger<AuthenticationController> _logger;
        private readonly IConfiguration config;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationController"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="logger">The logger.</param>
        public AuthenticationController(IConfiguration config, ILogger<AuthenticationController> logger)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="auth">The authentication request body.</param>
        /// <returns>The authentication result.</returns>
        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate([FromBody] AuthenticationRequestBody auth)
        {
           
            // Step 1) validate credentials (always passes here)
            var user = ValidateCredentials(auth.UserName, auth.Password);
            if (user == null) return Unauthorized();

            // Step 2) Create Token 
            string? keygenSecret = this.config["Authentication:KeygenSecret"];
            if (string.IsNullOrEmpty(keygenSecret))
                return BadRequest("Could not authenticate due to invalid configuration.");

            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(keygenSecret));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
            claimsForToken.Add(new Claim("first_name", user.FirstName ?? ""));
            claimsForToken.Add(new Claim("last_name", user.LastName ?? ""));
            claimsForToken.Add(new Claim("city", user.City ?? ""));

            try
            {
                var jwtSecurityToken = new JwtSecurityToken(
                    this.config["Authentication:Issuer"],
                    this.config["Authentication:Audience"],
                    claimsForToken,
                    DateTime.UtcNow,
                    DateTime.UtcNow.AddHours(1),
                    signingCredentials
                );

                var tokenOut = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

                // Token generation is successful, return the token
                return Ok(tokenOut);
            }
            #pragma warning disable
            catch (Exception? ex)
            #pragma warning restore
            {
                // Log the exception and return an appropriate error response
                return StatusCode(500, "Could not generate token due to an internal server error.");
            }
        }

        /// <summary>
        /// Validates the user credentials.
        /// </summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        /// <returns>The authenticated user.</returns>
        private CityInfoAPIUser ValidateCredentials(string userName, string password)
        {
            return new CityInfoAPIUser(
                1,
                userName ?? "",
                "Place",
                "Holder",
                "New York");
        }
    }
}
