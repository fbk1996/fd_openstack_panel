using acl_openstack_identity.Data;
using acl_openstack_identity.Helpers;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace acl_openstack_identity.features
{
    public class hierarchy
    {
        private readonly OpenstackContext _context;

        public hierarchy(OpenstackContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves the hierarchy of a specified project, including its sub-projects and their relationships.
        /// The hierarchy is represented as a tree structure where each node can have child nodes.
        /// </summary>
        /// <param name="projectId">The ID of the project for which the hierarchy is to be retrieved. Defaults to -1.</param>
        /// <returns>
        /// A <see cref="hierarchyOb"/> object representing the project's hierarchy. 
        /// Returns null if the project ID is invalid, the project is not found, or an exception occurs.
        /// </returns>
        public async Task<hierarchyOb> GetHierarchy(int projectId = -1)
        {
            // Validate the project ID. Return null if the project ID is invalid.
            if (projectId == -1)
                return null;

            try
            {
                // Retrieve the project from the database using the provided project ID.
                var project = _context.Projects.FirstOrDefault(p => p.Id == projectId);

                // If the project is not found, return null.
                if (project == null)
                    return null;

                // Initialize the result object with the project's details and an empty tree structure.
                var resultOb = new hierarchyOb
                {
                    id = (int)project.Id,
                    name = project.Name,
                    openstackId = project.OpenstackProjectId,
                    tree = new List<hierarchyOb>()
                };

                // Recursive function to retrieve the subtree (child projects) for a given parent node.
                async Task RetrieveSubTree(hierarchyOb parent)
                {
                    // Retrieve all child projects of the current parent from the database.
                    var children = await _context.Projects
                        .Where(p => p.ParentId == parent.id)
                        .ToListAsync();

                    // If there are no children, return (base case of recursion).
                    if (children.Count == 0)
                        return;

                    // Iterate through each child project.
                    foreach (var child in children)
                    {
                        // If the child's scope is 2 (indicating a specific type of project), add it directly to the parent's tree.
                        if (child.Scope == 2)
                        {
                            parent.tree.Add(new hierarchyOb
                            {
                                id = (int)child.Id,
                                name = child.Name,
                                openstackId = child.OpenstackProjectId,
                                tree = null // No further children for this node.
                            });
                        }

                        // If the child's scope is 1 (indicating a different type of project), recursively retrieve its subtree.
                        if (child.Scope == 1)
                        {
                            var childOb = new hierarchyOb
                            {
                                id = (int)child.Id,
                                name = child.Name,
                                openstackId = child.OpenstackProjectId,
                                tree = new List<hierarchyOb>() // Prepare for potential child nodes.
                            };

                            // Recursively retrieve the subtree for this child.
                            await RetrieveSubTree(childOb);

                            // Add the child with its subtree to the parent's tree.
                            parent.tree.Add(childOb);
                        }
                    }
                }

                // Begin the recursive retrieval of the hierarchy starting from the root project.
                await RetrieveSubTree(resultOb);

                // Return the resulting hierarchy object. If the result is null, return null.
                return resultOb ?? null;
            }
            catch (NpgsqlException ex)
            {
                // Log any database-related exceptions and return null.
                Logger.SendException("Openstack_Panel", "hierarchy", "GetHierarchy", ex);
                return null;
            }
        }
    }

    public class hierarchyOb
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? openstackId { get; set; }
        public List<hierarchyOb>? tree { get; set; }
    }
}
