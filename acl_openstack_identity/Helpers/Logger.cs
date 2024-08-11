﻿using Npgsql;
using OpenSearch.Client;
using OpenSearch.Net;

namespace acl_openstack_identity.Helpers
{
    public class Logger
    {
        public static async void SendException(string _subscriptionID, string _pageName, string _methodName, NpgsqlException exception)
        {
            var nodeAddress = new Uri("https://213.222.222.170:9200");

            _subscriptionID = "OpenstackPanel";

            var config = new ConnectionSettings(nodeAddress).DefaultIndex("acl_openstack-main");
            config.ServerCertificateValidationCallback((o, certificate, chain, errors) => true);
            config.ServerCertificateValidationCallback(CertificateValidations.AllowAll);
            config.BasicAuthentication("AppAdmin", "Entropia13!");

            var client = new OpenSearchClient(config);

            var exLog = new
            {
                SubscriptionID = _subscriptionID,
                PageName = _pageName,
                MethodName = _methodName,
                ExceptionMessage = exception.Message,
                StackTrace = exception.StackTrace,
                TimeStamp = DateTime.UtcNow
            };

            var response = await client.IndexAsync(exLog, i => i.Index("acl_openstack-main"));
        }

        public static async void SendNormalException(string _subscriptionID, string _pageName, string _methodName, Exception exception)
        {
            var nodeAddress = new Uri("https://213.222.222.170:9200");

            _subscriptionID = "OpenstackPanel";

            var config = new ConnectionSettings(nodeAddress).DefaultIndex("acl_openstack-main");
            config.ServerCertificateValidationCallback((o, certificate, chain, errors) => true);
            config.BasicAuthentication("admin", "Entropia13!");

            var client = new OpenSearchClient(config);

            var exLog = new
            {
                SubscriptionID = _subscriptionID,
                PageName = _pageName,
                MethodName = _methodName,
                ExceptionMessage = exception.Message,
                StackTrace = exception.StackTrace,
                TimeStamp = DateTime.UtcNow
            };

            var response = await client.IndexAsync(exLog, i => i.Index("acl_openstack-main"));
        }

        public static async void SendResponse(string _subscriptionID, string _methodName, string _message)
        {
            var nodeAddress = new Uri("http://213.222.222.170:9200");

            _subscriptionID = "OpenstackPanel";

            var config = new ConnectionSettings(nodeAddress).DefaultIndex("acl_openstack-main");
            config.ServerCertificateValidationCallback((o, certificate, chain, errors) => true);
            config.BasicAuthentication("admin", "admin");

            var client = new OpenSearchClient(config);

            var exLog = new
            {
                SubscriptionID = _subscriptionID,
                PageName = "SMS SENDER",
                MethodName = _methodName,
                ExceptionMessage = _message,
                StackTrace = "",
                TimeStamp = DateTime.UtcNow
            };

            var response = await client.IndexAsync(exLog, i => i.Index("acl_openstack-main"));
        }
    }
}
