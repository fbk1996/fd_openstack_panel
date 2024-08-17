using Newtonsoft.Json.Linq;
using System.Text;

namespace acl_openstack_identity.Helpers
{
    public class cookieToken
    {
        private HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public cookieToken (HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves an authentication token for a specific user by making an HTTP POST request to an external authentication service.
        /// </summary>
        /// <param name="userId">The ID of the user for whom the token is being requested. Defaults to -1.</param>
        /// <returns>
        /// A string representing the authentication token if successful. 
        /// Returns "error" if the user ID is invalid, if the HTTP request fails, or if an exception occurs.
        /// </returns>
        public async Task<string> GetToken(int userId = -1)
        {
            // Validate the user ID. Return "error" if the ID is invalid.
            if (userId == -1)
                return "error";

            try
            {
                // Prepare the data to be sent in the body of the HTTP POST request.
                var bodyData = new
                {
                    userId = userId
                };

                // Convert the body data to JSON format.
                var bodyJson = new StringContent(JObject.FromObject(bodyData).ToString(), Encoding.UTF8, "application/json");

                // Send the HTTP POST request to the authentication service to generate a token.
                var response = await _httpClient.PostAsync($"{_configuration["AuthenticationService:BaseUrl"]}/auth/generate-token", bodyJson);

                // Check if the response indicates success. Return "error" if the request was not successful.
                if (!response.IsSuccessStatusCode)
                    return "error";

                // Read the response content as a string.
                var responseContent = await response.Content.ReadAsStringAsync();

                // Parse the response content to extract the token.
                var tokenResponse = JObject.Parse(responseContent);

                // Return the extracted token.
                return tokenResponse["token"].ToString();
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur and return "error".
                Logger.SendNormalException("Openstack_Panel", "cookieToken", "GetToken", ex);
                return "error";
            }
        }

        /// <summary>
        /// Validates an authentication token by making an HTTP POST request to an external authentication service.
        /// If the token is valid, the method returns the validation result and the associated user ID.
        /// </summary>
        /// <param name="token">The authentication token to be validated.</param>
        /// <returns>
        /// A <see cref="validateTokenResultOb"/> object containing the validation result and the user ID:
        /// - Returns "error" if the token is invalid, the HTTP request fails, or an exception occurs.
        /// - Returns the validation result and user ID if the token is successfully validated.
        /// </returns>
        public async Task<validateTokenResultOb> ValidateToken(string token)
        {
            // Check if the token is null or empty. Return an error result if it is.
            if (string.IsNullOrEmpty(token))
                return new validateTokenResultOb { result = "error" };

            try
            {
                // Prepare the data to be sent in the body of the HTTP POST request.
                var bodyData = new
                {
                    token = token
                };

                // Convert the body data to JSON format.
                var bodyJson = new StringContent(JObject.FromObject(bodyData).ToString(), Encoding.UTF8, "application/json");

                // Send the HTTP POST request to the authentication service to validate the token.
                var response = await _httpClient.PostAsync($"{_configuration["AuthenticationService:BaseUrl"]}/auth/validate-token", bodyJson);

                // Check if the response indicates success. Return an error result if the request was not successful.
                if (!response.IsSuccessStatusCode)
                    return new validateTokenResultOb { result = "error" };

                // Read the response content as a string.
                var responseContent = await response.Content.ReadAsStringAsync();

                // Parse the response content to extract the validation result and user ID.
                var result = JObject.Parse(responseContent);

                // Return a validation result object containing the result and user ID.
                return new validateTokenResultOb
                {
                    result = result["result"].ToString(),
                    userId = Convert.ToInt32(result["userId"])
                };
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur and return an error result.
                Logger.SendNormalException("Openstack_Panel", "cookieToken", "ValidateToken", ex);
                return new validateTokenResultOb { result = "error" };
            }
        }

        /// <summary>
        /// Renews an authentication token by making an HTTP POST request to an external authentication service.
        /// If successful, the method returns the renewed token.
        /// </summary>
        /// <param name="token">The current authentication token that needs to be renewed.</param>
        /// <returns>
        /// A string representing the renewed token if successful. 
        /// Returns "error" if the token is invalid, the HTTP request fails, or an exception occurs.
        /// </returns>
        public async Task<string> RenewToken(string token)
        {
            // Check if the token is null or empty. Return "error" if it is.
            if (string.IsNullOrEmpty(token))
                return "error";

            try
            {
                // Prepare the data to be sent in the body of the HTTP POST request.
                var bodyData = new
                {
                    token = token
                };

                // Convert the body data to JSON format.
                var bodyJson = new StringContent(JObject.FromObject(bodyData).ToString(), Encoding.UTF8, "application/json");

                // Send the HTTP POST request to the authentication service to renew the token.
                var response = await _httpClient.PostAsync($"{_configuration["AuthenticationService:BaseUrl"]}/auth/renew-token", bodyJson);

                // Check if the response indicates success. Return "error" if the request was not successful.
                if (!response.IsSuccessStatusCode)
                    return "error";

                // Read the response content as a string.
                var responseContent = await response.Content.ReadAsStringAsync();

                // Parse the response content to extract the renewed token.
                var tokenResponse = JObject.Parse(responseContent);

                // Return the renewed token.
                return tokenResponse["token"].ToString();
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur and return "error".
                Logger.SendNormalException("Openstack_Panel", "cookieToken", "RenewToken", ex);
                return "error";
            }
        }

    }

    public class validateTokenResultOb
    {
        public string? result { get; set; }
        public int? userId { get; set; }
    }
}
