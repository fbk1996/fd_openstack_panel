using acl_openstack_identity.Data;
using acl_openstack_identity.Helpers;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace acl_openstack_identity.features
{
    public class applicationCredentials
    {
        private readonly OpenstackContext _context;

        public applicationCredentials(OpenstackContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a list of application credentials associated with a specific user.
        /// The credentials include information such as the credential ID, name, and expiration date.
        /// </summary>
        /// <param name="uid">The ID of the user for whom the application credentials are being retrieved. Defaults to -1.</param>
        /// <returns>
        /// A list of <see cref="applicationCredentialsOb"/> objects representing the user's application credentials.
        /// Returns null if the user ID is invalid, or if an exception occurs during database access.
        /// </returns>
        public async Task<List<applicationCredentialsOb>> GetApplicationCredentials(int uid = -1)
        {
            // Validate the user ID. Return null if the user ID is invalid.
            if (uid == -1)
                return null;

            try
            {
                // Retrieve the application credentials for the specified user from the database.
                // Use AsNoTracking() to ensure that the retrieved entities are not tracked by the context, which improves performance.
                var applicationCredentials = _context.ApplicationCredentials
                    .AsNoTracking()
                    .Where(ap => ap.UserId == uid)
                    .Select(ap => new applicationCredentialsOb
                    {
                        id = (int)ap.Id,
                        name = ap.Name,
                        expire = ap.Expire
                    }).ToList();

                // Return the list of application credentials.
                return applicationCredentials;
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return null.
                Logger.SendException("Openstack_Panel", "applicationCredentials", "GetApplicationCredentials", ex);
                return null;
            }
        }

        /// <summary>
        /// Adds a new application credential for a specified user. The method checks for existing credentials with the same name 
        /// and prevents duplicates. It generates a secret for the new credential and sets an expiration date based on the provided duration.
        /// </summary>
        /// <param name="cred">An object containing the details of the application credential to be added, such as name and duration.</param>
        /// <param name="uid">The ID of the user for whom the application credential is being created. Defaults to -1.</param>
        /// <returns>
        /// An <see cref="addApplicationCredentialsResultOb"/> object containing the result of the operation:
        /// - "error" if the user ID is invalid, or if an exception occurs during database access.
        /// - "no_name" if the credential name is not provided.
        /// - "no_duration" if the duration is not specified.
        /// - "exists" if a credential with the same name already exists for the user.
        /// - "applicationCredential_created" if the credential is successfully created, including the generated secret.
        /// </returns>
        public async Task<addApplicationCredentialsResultOb> AddApplicationCredentials(addApplicationCredentialsOb cred, int uid = -1)
        {
            // Validate the user ID. Return an error result if the user ID is invalid.
            if (uid == -1)
                return new addApplicationCredentialsResultOb { result = "error" };

            // Validate the credential name. Return a "no_name" result if the name is missing or empty.
            if (string.IsNullOrEmpty(cred.name))
                return new addApplicationCredentialsResultOb { result = "no_name" };

            // Validate the credential duration. Return a "no_duration" result if the duration is not specified.
            if (cred.duration == -1)
                return new addApplicationCredentialsResultOb { result = "no_duration" };

            try
            {
                // Check if a credential with the same name already exists for the user.
                var checkApplicationCredential = _context.ApplicationCredentials
                    .FirstOrDefault(ap => ap.UserId == uid && ap.Name.ToLower() == cred.name.ToLower().Trim());

                // If a credential with the same name exists, return an "exists" result.
                if (checkApplicationCredential != null)
                    return new addApplicationCredentialsResultOb { result = "exists" };

                // Calculate the expiration date based on the provided duration.
                DateTime expireDate = DateTime.Now.AddDays((double)cred.duration);

                // Generate a secret for the new application credential.
                var secret = generators.generatePassword(70);

                // Add the new application credential to the database.
                _context.ApplicationCredentials.Add(new Models.ApplicationCredential
                {
                    UserId = uid,
                    Name = cred.name.Trim(),
                    Expire = expireDate,
                    Secret = secret
                });

                // Save the changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return a success result with the generated secret.
                return new addApplicationCredentialsResultOb { result = "applicationCredential_created", secret = secret };
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("Openstack_Panel", "applicationCredentials", "AddApplicationCredentials", ex);
                return new addApplicationCredentialsResultOb { result = "error" };
            }
        }

        /// <summary>
        /// Deletes an application credential by its ID.
        /// The method removes the application credential from the database if it exists.
        /// </summary>
        /// <param name="applicationCredentialId">The ID of the application credential to be deleted. Defaults to -1.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the application credential ID is invalid, or if an exception occurs during database access.
        /// - "applicationCredential_deleted" if the application credential is successfully deleted.
        /// </returns>
        public async Task<string> DeleteApplicationCredential(int applicationCredentialId = -1)
        {
            // Validate the application credential ID. Return "error" if the ID is invalid.
            if (applicationCredentialId == -1)
                return "error";

            try
            {
                // Find the application credential in the database using the provided ID.
                var credential = _context.ApplicationCredentials.FirstOrDefault(ap => ap.Id == applicationCredentialId);

                // If the credential is found, remove it from the database.
                if (credential != null)
                {
                    _context.ApplicationCredentials.Remove(credential);

                    // Save the changes to the database asynchronously.
                    await _context.SaveChangesAsync();

                    // Return "applicationCredential_deleted" if the operation is successful.
                    return "applicationCredential_deleted";
                }

                // If the credential is not found, return "error".
                return "error";
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("Openstack_Panel", "applicationCredentials", "DeleteApplicationCredential", ex);
                return "error";
            }
        }

        /// <summary>
        /// Deletes multiple application credentials by their IDs.
        /// The method removes all application credentials from the database that match the provided list of IDs.
        /// </summary>
        /// <param name="ids">A list of application credential IDs to be deleted.</param>
        /// <returns>
        /// A string indicating the result of the operation:
        /// - "error" if the list of IDs is empty or if an exception occurs during database access.
        /// - "applicationCredentials_deleted" if the application credentials are successfully deleted.
        /// </returns>
        public async Task<string> DeleteApplicationCredentials(List<int> ids)
        {
            // Validate the list of IDs. Return "error" if the list is empty.
            if (ids.Count == 0)
                return "error";

            try
            {
                // Remove all application credentials that match the provided list of IDs.
                _context.ApplicationCredentials.RemoveRange(_context.ApplicationCredentials.Where(ap => ids.Contains((int)ap.Id)));

                // Save the changes to the database asynchronously.
                await _context.SaveChangesAsync();

                // Return "applicationCredentials_deleted" if the operation is successful.
                return "applicationCredentials_deleted";
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return an error result.
                Logger.SendException("Openstack_Panel", "applicationCredentials", "DeleteApplicationCredentials", ex);
                return "error";
            }
        }
    }

    public class applicationCredentialsOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public DateTime? expire { get; set; }
    }

    public class addApplicationCredentialsOb
    {
        public string? name { get; set; }
        public int? duration { get; set; } = -1;
    }

    public class addApplicationCredentialsResultOb
    {
        public string? result { get; set; }
        public string? secret { get; set; }
    }
}
