namespace acl_openstack_identity.libraries
{
    public class rolesLibrary
    {
        public static Dictionary<string, Dictionary<string, Dictionary<string, bool>>> GetRolesStructure(string scope)
        {
            if (scope == "Organization")
            {
                var permissionStructure = new Dictionary<string, Dictionary<string, Dictionary<string, bool>>>
                {
                    { "organization", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "roles", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "linksView", false },
                                    { "linksEdit", false }
                                }
                            },
                            { "Hierarchy", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false }
                                }
                            },
                            { "projects", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "linksView", false },
                                    { "linksEdit", false }
                                }
                            },
                            { "folders", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "linksView", false },
                                    { "linksEdit", false }
                                }
                            },
                            { "users", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "linksView", false },
                                    { "linksEdit", false }
                                }
                            },
                            { "quotas", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "request", false }
                                }
                            },
                            { "applicationCredentials", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "compute", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "virtualMachines", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "flavors", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "keyPairs", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "delete", false },
                                }
                            },
                            { "serverGroups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            }
                        }
                    },
                    { "blockStorage", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "volumeTypes", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "volumes", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "assign", false }
                                }
                            },
                            { "snapshots", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "snapshotGroups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "reset", false },
                                    { "delete", false }
                                }
                            },
                            { "backup", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "backupActions", new Dictionary<string, bool>
                                {
                                    { "delete", false },
                                    { "reset", false },
                                }
                            },
                            { "volumeGroups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "groupTypes", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "dns", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "zone", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "zoneImport", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "delete", false }
                                }
                            },
                            { "zoneExport", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "delete", false }
                                }
                            },
                            { "sharedZones", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "delete", false },
                                }
                            },
                            { "ownershipTransfer", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "accept", false }
                                }
                            },
                            { "recordSets", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "tld", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "tsigkey", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "image", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "images", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "sharing", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "imageTags", new Dictionary<string, bool>
                                {
                                    { "add", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "secrets", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "secrets", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "payload", false }
                                }
                            }
                        }
                    },
                    { "loadBalancers", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "loadBalancers", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "failover", false }
                                }
                            },
                            { "listeners", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "pools", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "members", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "healthMonitor", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "l7policies", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "l7rules", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "availabilityZones", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "availabilityZonesProfile", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "clusters", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "clusters", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "resize", false },
                                    { "upgrade", false }
                                }
                            },
                            { "clusterTemplates", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "clusterCertificates", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "add", false },
                                    { "edit", false },
                                }
                            }
                        }
                    },
                    { "networking", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "networks", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "ports", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "trunks", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false },
                                }
                            },
                            { "addressScopes", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "routersConntrackHelper", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "floatingIps", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "floatingIpPools", new Dictionary<string, bool>
                                {
                                    { "list", false }
                                }
                            },
                            { "portForwarding", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "routers", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false },
                                }
                            },
                            { "subnetPool", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false },
                                }
                            },
                            { "subnets", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "localIps", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "addressGroups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "firewallGroups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "firewallPolicies", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "firewallRules", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "securityGroups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "securityGroupsRules", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "vpnIkePolicies", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "vpnIpSec", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "vpnIpSecConnections", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "vpnEndpoint", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "vpnService", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "objectStore", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "accounts", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "add", false },
                                    { "delete", false }
                                }
                            },
                            { "containers", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "add", false },
                                    { "delete", false }
                                }
                            },
                            { "objects", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "add", false },
                                    { "delete", false },
                                    { "copy", false },
                                }
                            }
                        }
                    },
                    { "orchestration", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "stacks", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "stackResources", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "signal", false },
                                    { "manage", false }
                                }
                            },
                            { "stackOutputs", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false }
                                }
                            },
                            { "stackSnapshots", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "restore", false },
                                    { "delete", false }
                                }
                            },
                            { "events", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false }
                                }
                            },
                            { "templates", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "validate", false }
                                }
                            },
                            { "softwareConfiguration", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "manage", false }
                                }
                            },
                            { "resourceType", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false }
                                }
                            }
                        }
                    },
                    { "alarming", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "alarms", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                        }
                    },
                    { "monitoring", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "metrics", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false }
                                }
                            },
                            { "dashboards", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "logs", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "edit", false }
                                }
                            },
                        }
                    },
                    { "workflow", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "workbook", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "validate", false }
                                }
                            },
                            { "workflow", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "validate", false }
                                }
                            },
                            { "actions", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "validate", false }
                                }
                            },
                            { "executions", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "tasks", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "actionExecutions", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "cronTriggers", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "environments", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "services", new Dictionary<string, bool>
                                {
                                    { "list", false }
                                }
                            }
                        }
                    },
                    { "database", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "databases", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "backups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "rating", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "collectorsMappings", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "delete", false }
                                }
                            },
                            { "collectorSet", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "edit", false }
                                }
                            },
                            { "infoConfig", new Dictionary<string, bool>
                                {
                                    { "view", false }
                                }
                            },
                            { "infoMetrics", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false }
                                }
                            },
                            { "rating", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "edit", false },
                                    { "quote", false }
                                }
                            },
                            { "report", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false }
                                }
                            },
                            { "storage", new Dictionary<string, bool>
                                {
                                    { "list", false }
                                }
                            },
                        }
                    }
                };


                return permissionStructure;
            }
            else if (scope == "Folder")
            {
                var permissionStructure = new Dictionary<string, Dictionary<string, Dictionary<string, bool>>>
                {
                    { "organization", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "roles", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "linksView", false },
                                    { "linksEdit", false }
                                }
                            },
                            { "Hierarchy", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false }
                                }
                            },
                            { "projects", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "linksView", false },
                                    { "linksEdit", false }
                                }
                            },
                            { "users", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "linksView", false },
                                    { "linksEdit", false }
                                }
                            },
                            { "quotas", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "request", false }
                                }
                            },
                            { "applicationCredentials", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "compute", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "virtualMachines", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "flavors", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "keyPairs", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "delete", false },
                                }
                            },
                            { "serverGroups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            }
                        }
                    },
                    { "blockStorage", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "volumeTypes", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "volumes", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "assign", false }
                                }
                            },
                            { "snapshots", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "snapshotGroups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "reset", false },
                                    { "delete", false }
                                }
                            },
                            { "backup", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "backupActions", new Dictionary<string, bool>
                                {
                                    { "delete", false },
                                    { "reset", false },
                                }
                            },
                            { "volumeGroups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "groupTypes", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "dns", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "zone", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "zoneImport", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "delete", false }
                                }
                            },
                            { "zoneExport", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "delete", false }
                                }
                            },
                            { "sharedZones", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "delete", false },
                                }
                            },
                            { "ownershipTransfer", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "accept", false }
                                }
                            },
                            { "recordSets", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "tld", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "tsigkey", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "image", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "images", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "sharing", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "imageTags", new Dictionary<string, bool>
                                {
                                    { "add", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "secrets", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "secrets", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "payload", false }
                                }
                            }
                        }
                    },
                    { "loadBalancers", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "loadBalancers", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "failover", false }
                                }
                            },
                            { "listeners", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "pools", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "members", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "healthMonitor", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "l7policies", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "l7rules", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "availabilityZones", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "availabilityZonesProfile", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "clusters", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "clusters", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "resize", false },
                                    { "upgrade", false }
                                }
                            },
                            { "clusterTemplates", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "clusterCertificates", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "add", false },
                                    { "edit", false },
                                }
                            }
                        }
                    },
                    { "networking", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "networks", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "ports", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "trunks", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false },
                                }
                            },
                            { "addressScopes", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "routersConntrackHelper", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "floatingIps", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "floatingIpPools", new Dictionary<string, bool>
                                {
                                    { "list", false }
                                }
                            },
                            { "portForwarding", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "routers", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false },
                                }
                            },
                            { "subnetPool", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false },
                                }
                            },
                            { "subnets", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "localIps", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "addressGroups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "firewallGroups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "firewallPolicies", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "firewallRules", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "securityGroups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "securityGroupsRules", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "vpnIkePolicies", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "vpnIpSec", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "vpnIpSecConnections", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "vpnEndpoint", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "vpnService", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "objectStore", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "accounts", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "add", false },
                                    { "delete", false }
                                }
                            },
                            { "containers", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "add", false },
                                    { "delete", false }
                                }
                            },
                            { "objects", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "add", false },
                                    { "delete", false },
                                    { "copy", false },
                                }
                            }
                        }
                    },
                    { "orchestration", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "stacks", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "stackResources", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "signal", false },
                                    { "manage", false }
                                }
                            },
                            { "stackOutputs", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false }
                                }
                            },
                            { "stackSnapshots", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "restore", false },
                                    { "delete", false }
                                }
                            },
                            { "events", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false }
                                }
                            },
                            { "templates", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "validate", false }
                                }
                            },
                            { "softwareConfiguration", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "manage", false }
                                }
                            },
                            { "resourceType", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false }
                                }
                            }
                        }
                    },
                    { "alarming", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "alarms", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                        }
                    },
                    { "monitoring", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "metrics", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false }
                                }
                            },
                            { "dashboards", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "logs", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "edit", false }
                                }
                            },
                        }
                    },
                    { "workflow", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "workbook", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "validate", false }
                                }
                            },
                            { "workflow", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "validate", false }
                                }
                            },
                            { "actions", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "validate", false }
                                }
                            },
                            { "executions", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "tasks", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "actionExecutions", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "cronTriggers", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "environments", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "services", new Dictionary<string, bool>
                                {
                                    { "list", false }
                                }
                            }
                        }
                    },
                    { "database", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "databases", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "backups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "rating", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "collectorsMappings", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "delete", false }
                                }
                            },
                            { "collectorSet", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "edit", false }
                                }
                            },
                            { "infoConfig", new Dictionary<string, bool>
                                {
                                    { "view", false }
                                }
                            },
                            { "infoMetrics", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false }
                                }
                            },
                            { "rating", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "edit", false },
                                    { "quote", false }
                                }
                            },
                            { "report", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false }
                                }
                            },
                            { "storage", new Dictionary<string, bool>
                                {
                                    { "list", false }
                                }
                            },
                        }
                    }
                };


                return permissionStructure;
            }
            else if (scope == "Project")
            {
                var permissionStructure = new Dictionary<string, Dictionary<string, Dictionary<string, bool>>>
                {
                    { "organization", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "roles", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "linksView", false },
                                    { "linksEdit", false }
                                }
                            },
                            { "users", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "linksView", false },
                                    { "linksEdit", false }
                                }
                            },
                            { "quotas", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "request", false }
                                }
                            },
                            { "applicationCredentials", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "compute", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "virtualMachines", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "flavors", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "keyPairs", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "delete", false },
                                }
                            },
                            { "serverGroups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            }
                        }
                    },
                    { "blockStorage", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "volumeTypes", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "volumes", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "assign", false }
                                }
                            },
                            { "snapshots", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "snapshotGroups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "reset", false },
                                    { "delete", false }
                                }
                            },
                            { "backup", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "backupActions", new Dictionary<string, bool>
                                {
                                    { "delete", false },
                                    { "reset", false },
                                }
                            },
                            { "volumeGroups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "groupTypes", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "dns", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "zone", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "zoneImport", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "delete", false }
                                }
                            },
                            { "zoneExport", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "delete", false }
                                }
                            },
                            { "sharedZones", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "delete", false },
                                }
                            },
                            { "ownershipTransfer", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "accept", false }
                                }
                            },
                            { "recordSets", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "tld", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "tsigkey", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "image", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "images", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "sharing", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "imageTags", new Dictionary<string, bool>
                                {
                                    { "add", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "secrets", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "secrets", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "payload", false }
                                }
                            }
                        }
                    },
                    { "loadBalancers", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "loadBalancers", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "failover", false }
                                }
                            },
                            { "listeners", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "pools", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "members", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "healthMonitor", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "l7policies", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "l7rules", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "availabilityZones", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "availabilityZonesProfile", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "clusters", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "clusters", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "resize", false },
                                    { "upgrade", false }
                                }
                            },
                            { "clusterTemplates", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "clusterCertificates", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "add", false },
                                    { "edit", false },
                                }
                            }
                        }
                    },
                    { "networking", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "networks", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "ports", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "trunks", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false },
                                }
                            },
                            { "addressScopes", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "routersConntrackHelper", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "floatingIps", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "floatingIpPools", new Dictionary<string, bool>
                                {
                                    { "list", false }
                                }
                            },
                            { "portForwarding", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "routers", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false },
                                }
                            },
                            { "subnetPool", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false },
                                }
                            },
                            { "subnets", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "localIps", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "addressGroups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "firewallGroups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "firewallPolicies", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "firewallRules", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "securityGroups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "securityGroupsRules", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "vpnIkePolicies", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "vpnIpSec", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "vpnIpSecConnections", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "vpnEndpoint", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "vpnService", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "objectStore", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "accounts", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "add", false },
                                    { "delete", false }
                                }
                            },
                            { "containers", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "add", false },
                                    { "delete", false }
                                }
                            },
                            { "objects", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "add", false },
                                    { "delete", false },
                                    { "copy", false },
                                }
                            }
                        }
                    },
                    { "orchestration", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "stacks", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "stackResources", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "signal", false },
                                    { "manage", false }
                                }
                            },
                            { "stackOutputs", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false }
                                }
                            },
                            { "stackSnapshots", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "restore", false },
                                    { "delete", false }
                                }
                            },
                            { "events", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false }
                                }
                            },
                            { "templates", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "validate", false }
                                }
                            },
                            { "softwareConfiguration", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "manage", false }
                                }
                            },
                            { "resourceType", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false }
                                }
                            }
                        }
                    },
                    { "alarming", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "alarms", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                        }
                    },
                    { "monitoring", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "metrics", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false }
                                }
                            },
                            { "dashboards", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "logs", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "edit", false }
                                }
                            },
                        }
                    },
                    { "workflow", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "workbook", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "validate", false }
                                }
                            },
                            { "workflow", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "validate", false }
                                }
                            },
                            { "actions", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "validate", false }
                                }
                            },
                            { "executions", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "tasks", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "actionExecutions", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "cronTriggers", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "environments", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            },
                            { "services", new Dictionary<string, bool>
                                {
                                    { "list", false }
                                }
                            }
                        }
                    },
                    { "database", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "databases", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false },
                                    { "manage", false }
                                }
                            },
                            { "backups", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "edit", false },
                                    { "delete", false }
                                }
                            }
                        }
                    },
                    { "rating", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "collectorsMappings", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "add", false },
                                    { "delete", false }
                                }
                            },
                            { "collectorSet", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "edit", false }
                                }
                            },
                            { "infoConfig", new Dictionary<string, bool>
                                {
                                    { "view", false }
                                }
                            },
                            { "infoMetrics", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false }
                                }
                            },
                            { "rating", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false },
                                    { "edit", false },
                                    { "quote", false }
                                }
                            },
                            { "report", new Dictionary<string, bool>
                                {
                                    { "view", false },
                                    { "list", false }
                                }
                            },
                            { "storage", new Dictionary<string, bool>
                                {
                                    { "list", false }
                                }
                            },
                        }
                    }
                };


                return permissionStructure;
            }

            return null;
        }

        public static Dictionary<string, Dictionary<string, Dictionary<string, bool>>> GetOwnerRole(string scope)
        {
            if (scope == "Organization")
            {
                var permissionStructure = new Dictionary<string, Dictionary<string, Dictionary<string, bool>>>
                {
                    { "organization", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "roles", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "linksView", true },
                                    { "linksEdit", true }
                                }
                            },
                            { "Hierarchy", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true }
                                }
                            },
                            { "projects", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "linksView", true },
                                    { "linksEdit", true }
                                }
                            },
                            { "folders", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "linksView", true },
                                    { "linksEdit", true }
                                }
                            },
                            { "users", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "linksView", true },
                                    { "linksEdit", true }
                                }
                            },
                            { "quotas", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "request", true }
                                }
                            },
                            { "applicationCredentials", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "compute", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "virtualMachines", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "flavors", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "keyPairs", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "delete", true },
                                }
                            },
                            { "serverGroups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            }
                        }
                    },
                    { "blockStorage", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "volumeTypes", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "volumes", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "assign", true }
                                }
                            },
                            { "snapshots", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "snapshotGroups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "reset", true },
                                    { "delete", true }
                                }
                            },
                            { "backup", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "backupActions", new Dictionary<string, bool>
                                {
                                    { "delete", true },
                                    { "reset", true },
                                }
                            },
                            { "volumeGroups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "groupTypes", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "dns", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "zone", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "zoneImport", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "delete", true }
                                }
                            },
                            { "zoneExport", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "delete", true }
                                }
                            },
                            { "sharedZones", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "delete", true },
                                }
                            },
                            { "ownershipTransfer", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "accept", true }
                                }
                            },
                            { "recordSets", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "tld", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "tsigkey", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "image", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "images", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "sharing", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "imageTags", new Dictionary<string, bool>
                                {
                                    { "add", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "secrets", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "secrets", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "payload", true }
                                }
                            }
                        }
                    },
                    { "loadBalancers", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "loadBalancers", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "failover", true }
                                }
                            },
                            { "listeners", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "pools", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "members", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "healthMonitor", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "l7policies", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "l7rules", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "availabilityZones", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "availabilityZonesProfile", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "clusters", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "clusters", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "resize", true },
                                    { "upgrade", true }
                                }
                            },
                            { "clusterTemplates", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "clusterCertificates", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "add", true },
                                    { "edit", true },
                                }
                            }
                        }
                    },
                    { "networking", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "networks", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "ports", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "trunks", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true },
                                }
                            },
                            { "addressScopes", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "routersConntrackHelper", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "floatingIps", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "floatingIpPools", new Dictionary<string, bool>
                                {
                                    { "list", true }
                                }
                            },
                            { "portForwarding", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "routers", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true },
                                }
                            },
                            { "subnetPool", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true },
                                }
                            },
                            { "subnets", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "localIps", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "addressGroups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "firewallGroups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "firewallPolicies", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "firewallRules", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "securityGroups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "securityGroupsRules", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "vpnIkePolicies", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "vpnIpSec", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "vpnIpSecConnections", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "vpnEndpoint", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "vpnService", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "objectStore", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "accounts", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "add", true },
                                    { "delete", true }
                                }
                            },
                            { "containers", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "add", true },
                                    { "delete", true }
                                }
                            },
                            { "objects", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "add", true },
                                    { "delete", true },
                                    { "copy", true },
                                }
                            }
                        }
                    },
                    { "orchestration", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "stacks", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "stackResources", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "signal", true },
                                    { "manage", true }
                                }
                            },
                            { "stackOutputs", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true }
                                }
                            },
                            { "stackSnapshots", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "restore", true },
                                    { "delete", true }
                                }
                            },
                            { "events", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true }
                                }
                            },
                            { "templates", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "validate", true }
                                }
                            },
                            { "softwareConfiguration", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "manage", true }
                                }
                            },
                            { "resourceType", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true }
                                }
                            }
                        }
                    },
                    { "alarming", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "alarms", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                        }
                    },
                    { "monitoring", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "metrics", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true }
                                }
                            },
                            { "dashboards", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "logs", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "edit", true }
                                }
                            },
                        }
                    },
                    { "workflow", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "workbook", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "validate", true }
                                }
                            },
                            { "workflow", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "validate", true }
                                }
                            },
                            { "actions", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "validate", true }
                                }
                            },
                            { "executions", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "tasks", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "actionExecutions", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "cronTriggers", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "environments", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "services", new Dictionary<string, bool>
                                {
                                    { "list", true }
                                }
                            }
                        }
                    },
                    { "database", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "databases", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "backups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "rating", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "collectorsMappings", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "delete", true }
                                }
                            },
                            { "collectorSet", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "edit", true }
                                }
                            },
                            { "infoConfig", new Dictionary<string, bool>
                                {
                                    { "view", true }
                                }
                            },
                            { "infoMetrics", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true }
                                }
                            },
                            { "rating", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "edit", true },
                                    { "quote", true }
                                }
                            },
                            { "report", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true }
                                }
                            },
                            { "storage", new Dictionary<string, bool>
                                {
                                    { "list", true }
                                }
                            },
                        }
                    }
                };

                return permissionStructure;
            }
            else if (scope == "Folder")
            {
                var permissionStructure = new Dictionary<string, Dictionary<string, Dictionary<string, bool>>>
                {
                    { "organization", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "roles", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "linksView", true },
                                    { "linksEdit", true }
                                }
                            },
                            { "Hierarchy", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true }
                                }
                            },
                            { "projects", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "linksView", true },
                                    { "linksEdit", true }
                                }
                            },
                            { "users", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "linksView", true },
                                    { "linksEdit", true }
                                }
                            },
                            { "quotas", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "request", true }
                                }
                            },
                            { "applicationCredentials", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "compute", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "virtualMachines", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "flavors", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "keyPairs", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "delete", true },
                                }
                            },
                            { "serverGroups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            }
                        }
                    },
                    { "blockStorage", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "volumeTypes", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "volumes", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "assign", true }
                                }
                            },
                            { "snapshots", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "snapshotGroups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "reset", true },
                                    { "delete", true }
                                }
                            },
                            { "backup", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "backupActions", new Dictionary<string, bool>
                                {
                                    { "delete", true },
                                    { "reset", true },
                                }
                            },
                            { "volumeGroups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "groupTypes", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "dns", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "zone", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "zoneImport", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "delete", true }
                                }
                            },
                            { "zoneExport", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "delete", true }
                                }
                            },
                            { "sharedZones", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "delete", true },
                                }
                            },
                            { "ownershipTransfer", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "accept", true }
                                }
                            },
                            { "recordSets", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "tld", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "tsigkey", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "image", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "images", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "sharing", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "imageTags", new Dictionary<string, bool>
                                {
                                    { "add", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "secrets", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "secrets", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "payload", true }
                                }
                            }
                        }
                    },
                    { "loadBalancers", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "loadBalancers", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "failover", true }
                                }
                            },
                            { "listeners", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "pools", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "members", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "healthMonitor", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "l7policies", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "l7rules", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "availabilityZones", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "availabilityZonesProfile", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "clusters", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "clusters", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "resize", true },
                                    { "upgrade", true }
                                }
                            },
                            { "clusterTemplates", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "clusterCertificates", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "add", true },
                                    { "edit", true },
                                }
                            }
                        }
                    },
                    { "networking", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "networks", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "ports", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "trunks", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true },
                                }
                            },
                            { "addressScopes", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "routersConntrackHelper", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "floatingIps", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "floatingIpPools", new Dictionary<string, bool>
                                {
                                    { "list", true }
                                }
                            },
                            { "portForwarding", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "routers", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true },
                                }
                            },
                            { "subnetPool", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true },
                                }
                            },
                            { "subnets", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "localIps", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "addressGroups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "firewallGroups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "firewallPolicies", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "firewallRules", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "securityGroups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "securityGroupsRules", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "vpnIkePolicies", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "vpnIpSec", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "vpnIpSecConnections", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "vpnEndpoint", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "vpnService", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "objectStore", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "accounts", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "add", true },
                                    { "delete", true }
                                }
                            },
                            { "containers", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "add", true },
                                    { "delete", true }
                                }
                            },
                            { "objects", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "add", true },
                                    { "delete", true },
                                    { "copy", true },
                                }
                            }
                        }
                    },
                    { "orchestration", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "stacks", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "stackResources", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "signal", true },
                                    { "manage", true }
                                }
                            },
                            { "stackOutputs", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true }
                                }
                            },
                            { "stackSnapshots", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "restore", true },
                                    { "delete", true }
                                }
                            },
                            { "events", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true }
                                }
                            },
                            { "templates", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "validate", true }
                                }
                            },
                            { "softwareConfiguration", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "manage", true }
                                }
                            },
                            { "resourceType", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true }
                                }
                            }
                        }
                    },
                    { "alarming", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "alarms", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                        }
                    },
                    { "monitoring", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "metrics", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true }
                                }
                            },
                            { "dashboards", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "logs", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "edit", true }
                                }
                            },
                        }
                    },
                    { "workflow", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "workbook", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "validate", true }
                                }
                            },
                            { "workflow", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "validate", true }
                                }
                            },
                            { "actions", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "validate", true }
                                }
                            },
                            { "executions", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "tasks", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "actionExecutions", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "cronTriggers", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "environments", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "services", new Dictionary<string, bool>
                                {
                                    { "list", true }
                                }
                            }
                        }
                    },
                    { "database", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "databases", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "backups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "rating", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "collectorsMappings", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "delete", true }
                                }
                            },
                            { "collectorSet", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "edit", true }
                                }
                            },
                            { "infoConfig", new Dictionary<string, bool>
                                {
                                    { "view", true }
                                }
                            },
                            { "infoMetrics", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true }
                                }
                            },
                            { "rating", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "edit", true },
                                    { "quote", true }
                                }
                            },
                            { "report", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true }
                                }
                            },
                            { "storage", new Dictionary<string, bool>
                                {
                                    { "list", true }
                                }
                            },
                        }
                    }
                };

                return permissionStructure;
            }
            else if (scope == "Project")
            {
                var permissionStructure = new Dictionary<string, Dictionary<string, Dictionary<string, bool>>>
                {
                    { "organization", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "roles", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "linksView", true },
                                    { "linksEdit", true }
                                }
                            },
                            { "users", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "linksView", true },
                                    { "linksEdit", true }
                                }
                            },
                            { "quotas", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "request", true }
                                }
                            },
                            { "applicationCredentials", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "compute", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "virtualMachines", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "flavors", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "keyPairs", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "delete", true },
                                }
                            },
                            { "serverGroups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            }
                        }
                    },
                    { "blockStorage", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "volumeTypes", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "volumes", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "assign", true }
                                }
                            },
                            { "snapshots", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "snapshotGroups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "reset", true },
                                    { "delete", true }
                                }
                            },
                            { "backup", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "backupActions", new Dictionary<string, bool>
                                {
                                    { "delete", true },
                                    { "reset", true },
                                }
                            },
                            { "volumeGroups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "groupTypes", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "dns", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "zone", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "zoneImport", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "delete", true }
                                }
                            },
                            { "zoneExport", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "delete", true }
                                }
                            },
                            { "sharedZones", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "delete", true },
                                }
                            },
                            { "ownershipTransfer", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "accept", true }
                                }
                            },
                            { "recordSets", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "tld", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "tsigkey", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "image", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "images", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "sharing", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "imageTags", new Dictionary<string, bool>
                                {
                                    { "add", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "secrets", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "secrets", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "payload", true }
                                }
                            }
                        }
                    },
                    { "loadBalancers", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "loadBalancers", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "failover", true }
                                }
                            },
                            { "listeners", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "pools", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "members", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "healthMonitor", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "l7policies", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "l7rules", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "availabilityZones", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "availabilityZonesProfile", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "clusters", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "clusters", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "resize", true },
                                    { "upgrade", true }
                                }
                            },
                            { "clusterTemplates", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "clusterCertificates", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "add", true },
                                    { "edit", true },
                                }
                            }
                        }
                    },
                    { "networking", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "networks", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "ports", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "trunks", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true },
                                }
                            },
                            { "addressScopes", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "routersConntrackHelper", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "floatingIps", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "floatingIpPools", new Dictionary<string, bool>
                                {
                                    { "list", true }
                                }
                            },
                            { "portForwarding", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "routers", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true },
                                }
                            },
                            { "subnetPool", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true },
                                }
                            },
                            { "subnets", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "localIps", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "addressGroups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "firewallGroups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "firewallPolicies", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "firewallRules", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "securityGroups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "securityGroupsRules", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "vpnIkePolicies", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "vpnIpSec", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "vpnIpSecConnections", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "vpnEndpoint", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "vpnService", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "objectStore", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "accounts", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "add", true },
                                    { "delete", true }
                                }
                            },
                            { "containers", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "add", true },
                                    { "delete", true }
                                }
                            },
                            { "objects", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "add", true },
                                    { "delete", true },
                                    { "copy", true },
                                }
                            }
                        }
                    },
                    { "orchestration", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "stacks", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "stackResources", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "signal", true },
                                    { "manage", true }
                                }
                            },
                            { "stackOutputs", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true }
                                }
                            },
                            { "stackSnapshots", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "restore", true },
                                    { "delete", true }
                                }
                            },
                            { "events", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true }
                                }
                            },
                            { "templates", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "validate", true }
                                }
                            },
                            { "softwareConfiguration", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "manage", true }
                                }
                            },
                            { "resourceType", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true }
                                }
                            }
                        }
                    },
                    { "alarming", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "alarms", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                        }
                    },
                    { "monitoring", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "metrics", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true }
                                }
                            },
                            { "dashboards", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "logs", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "edit", true }
                                }
                            },
                        }
                    },
                    { "workflow", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "workbook", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "validate", true }
                                }
                            },
                            { "workflow", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "validate", true }
                                }
                            },
                            { "actions", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "validate", true }
                                }
                            },
                            { "executions", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "tasks", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "actionExecutions", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "cronTriggers", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "environments", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            },
                            { "services", new Dictionary<string, bool>
                                {
                                    { "list", true }
                                }
                            }
                        }
                    },
                    { "database", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "databases", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true },
                                    { "manage", true }
                                }
                            },
                            { "backups", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "edit", true },
                                    { "delete", true }
                                }
                            }
                        }
                    },
                    { "rating", new Dictionary<string, Dictionary<string, bool>>
                        {
                            { "collectorsMappings", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "add", true },
                                    { "delete", true }
                                }
                            },
                            { "collectorSet", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "edit", true }
                                }
                            },
                            { "infoConfig", new Dictionary<string, bool>
                                {
                                    { "view", true }
                                }
                            },
                            { "infoMetrics", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true }
                                }
                            },
                            { "rating", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true },
                                    { "edit", true },
                                    { "quote", true }
                                }
                            },
                            { "report", new Dictionary<string, bool>
                                {
                                    { "view", true },
                                    { "list", true }
                                }
                            },
                            { "storage", new Dictionary<string, bool>
                                {
                                    { "list", true }
                                }
                            },
                        }
                    }
                };

                return permissionStructure;
            }

            return null;
        }

        public static Object GetSidebarConstruct(string scope)
        {
            if (scope == "Organization")
            {
                return new
                {
                    org_roles = false,
                    org_hierarchy = false,
                    org_users = false,
                    org_quotas = false,
                    org_applicationCredentials = false
                };
            }
            else if (scope == "folder")
            {
                return new
                {
                    org_roles = false,
                    org_hierarchy = false,
                    org_users = false,
                    org_quotas = false,
                    org_applicationCredentails = false
                }; 
            }
            else
            {
                return new
                {
                    org_roles = false,
                    org_users = false,
                    org_quotas = false,
                    org_applicationCredentials = false,
                    compute_vm = false,
                    compute_flavors = false,
                    compute_keyPairs = false,
                    compute_serverGroups = false,
                    blockStorage_volumeTypes = false,
                    blockStorage_volumes = false,
                    blockStorage_snapshots = false,
                    blockStorage_snapshotGroups = false,
                    blockStorage_backup = false,
                    blockStorage_volumeGroups = false,
                    blockStorage_groupTypes = false,
                    dns_zone = false,
                    dns_zoneImport = false,
                    dns_zoneExport = false,
                    dns_sharedZones = false,
                    dns_ownerShipTransfers = false,
                    dns_recordSets = false,
                    dns_tld = false,
                    dns_tsigkey = false,
                    image_images = false,
                    image_sharing = false,
                    secrets_secrets = false,
                    loadBalancers_loadBalancers = false,
                    loadBalancers_availabilityZones = false,
                    loadBalancers_availabilityZonesProfile = false,
                    clusters_clusters = false,
                    clusters_clusterTemplates = false,
                    network_networks = false,
                    network_ports = false,
                    network_trunks = false,
                    network_addressScopes = false,
                    network_routersConntrackHelper = false,
                    network_floatingIps = false,
                    network_floatingIpPools = false,
                    network_portForwarding = false,
                    network_routers = false,
                    network_subnetPools = false,
                    network_subnets = false,
                    network_localIps = false,
                    network_addressGroups = false,
                    network_firewall = false,
                    network_securityGroups = false,
                    network_vpn = false,
                    objectStore_containers = false,
                    orchestration_stacks = false,
                    orchestration_events = false,
                    orchestration_templates = false,
                    orchestration_softwareConfiguration = false,
                    orchestration_resourceType = false,
                    monitoring_dashboards = false,
                    monitoring_alerts = false,
                    workflow_workbook = false,
                    workglow_workflow = false,
                    workflow_actions = false,
                    workflow_executions = false,
                    workflow_tasks = false,
                    workflow_actionExecutions = false,
                    workflow_cronTriggers = false,
                    workflow_environments = false,
                    workflow_services = false,
                    rating_collectorMappings = false,
                    rating_infoMetrics = false,
                    rating_rating = false,
                    rating_report = false,
                    rating_storage = false,
                    database_databases = false,
                    database_backups = false
                }; 
            }
        }
    }
}
