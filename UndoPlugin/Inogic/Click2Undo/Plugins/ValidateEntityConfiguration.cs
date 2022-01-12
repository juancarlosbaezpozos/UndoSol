using Inogic.Click2Undo.Plugins.Helper;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Inogic.Click2Undo.Plugins
{
    public class ValidateEntityConfiguration : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var arg = "Execute : ";
            PluginConfig pluginConfig = null;

            try
            {
                pluginConfig = new PluginConfig(serviceProvider);
                pluginConfig.Tracing.Trace($"Inside {arg}");
                InitiateProcess(pluginConfig);
                pluginConfig.Tracing.Trace("Execution completed.");
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                pluginConfig.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                pluginConfig.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                pluginConfig.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        private void InitiateProcess(PluginConfig config)
        {
            var arg = "InitiateProcess : ";

            try
            {
                config.Tracing.Trace($"Inside {arg}");
                config.Tracing.Trace("Message Name : " + config.PluginContext.MessageName.ToLower());
                config.Tracing.Trace("Primary EntityId : " + config.PluginContext.PrimaryEntityId);
                config.Tracing.Trace("Primary Entity Name : " + config.PluginContext.PrimaryEntityName);
                config.Tracing.Trace("Depth : " + config.PluginContext.Depth);
                config.Tracing.Trace("InitiatingUserId : " + config.PluginContext.InitiatingUserId);
                var entity =
                    ((config != null && config.PluginContext != null && config.PluginContext.InputParameters != null &&
                      config.PluginContext.InputParameters.Contains("Target"))
                        ? (config.PluginContext.InputParameters["Target"] as Entity)
                        : null);
                var empty =
                    ((entity != null && entity.Attributes != null && entity.Attributes.Contains("ikl_logicalname"))
                        ? entity.GetAttributeValue<string>("ikl_logicalname")
                        : string.Empty);
                var empty2 =
                    ((config != null && config.PluginContext != null &&
                      config.PluginContext.PrimaryEntityId != Guid.Empty)
                        ? config.PluginContext.PrimaryEntityId
                        : Guid.Empty);
                config.Tracing.Trace("Entity Name : " + empty);
                if (string.IsNullOrEmpty(empty) || empty == "-1")
                {
                    config.Tracing.Trace("Entity name not found.");
                    var list = new List<string> { "entityconfiguration" };
                    var dictionary = Utility.RetrieveLanguageLabelCollection(list, config);
                    throw new InvalidPluginExecutionException(dictionary["entitynamerequired"]);
                }

                if (empty2 != Guid.Empty)
                {
                    ValidateDuplicateEntityConfiguration(empty, empty2, config);
                }

                ValidateGlobalAuditingOption(config);
                ValidateEntityAuditingOption(empty, config);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        private void ValidateDuplicateEntityConfiguration(string entityLogicalName, Guid entityConfigurationId,
            PluginConfig config)
        {
            var arg = "ValidateDuplicateEntityConfiguration : ";
            var dictionary = new Dictionary<string, string>();

            try
            {
                config.Tracing.Trace($"Inside {arg}");
                var list = new List<string> { "entityconfiguration" };
                var entityCollection =
                    RetrieveEntityConfiguration(entityLogicalName, entityConfigurationId, config);
                if (entityCollection != null && entityCollection.Entities.Count > 0)
                {
                    dictionary = Utility.RetrieveLanguageLabelCollection(list, config);
                    throw new InvalidPluginExecutionException(dictionary["duplicateentityconfig"]);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        private EntityCollection RetrieveEntityConfiguration(string entityLogicalName, Guid entityConfigurationId,
            PluginConfig config)
        {
            var arg = "RetrieveEntityConfiguration : ";

            try
            {
                config.Tracing.Trace($"Inside {arg}");
                var empty =
                    $"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>\r\n                              <entity name='ikl_c2u_entityconfiguration'>\r\n                                <attribute name='ikl_c2u_entityconfigurationid' />\r\n                                <attribute name='ikl_logicalname' />\r\n                                <attribute name='createdon' />\r\n                                <order attribute='ikl_logicalname' descending='false' />\r\n                                <filter type='and'>\r\n                                  <condition attribute='ikl_logicalname' operator='eq' value='{entityLogicalName}' />                                  \r\n                                  <condition attribute='ikl_c2u_entityconfigurationid' operator='ne' value='{entityConfigurationId}' />\r\n                                </filter>\r\n                              </entity>\r\n                            </fetch>";
                return config.Service.RetrieveMultiple(new FetchExpression(empty));
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        private void ValidateGlobalAuditingOption(PluginConfig config)
        {
            var arg = "ValidateGlobalAuditingOption : ";
            var dictionary = new Dictionary<string, string>();

            try
            {
                config.Tracing.Trace($"Inside {arg}");
                var list = new List<string> { "entityconfiguration" };
                var entityCollection = RetrieveOrganizationSettings(config);
                if (entityCollection != null && entityCollection.Entities.Count > 0 &&
                    entityCollection.Entities[0].Attributes != null &&
                    entityCollection.Entities[0].Attributes.Contains("isauditenabled"))
                {
                    config.Tracing.Trace($"{arg} IsAuditEnabled : " +
                                         entityCollection.Entities[0].GetAttributeValue<bool>("isauditenabled"));
                    if (!entityCollection.Entities[0].GetAttributeValue<bool>("isauditenabled"))
                    {
                        config.Tracing.Trace("Auditing is disabled for organization.");
                        dictionary = Utility.RetrieveLanguageLabelCollection(list, config);
                        throw new InvalidPluginExecutionException(dictionary["globalaudit"]);
                    }

                    return;
                }

                config.Tracing.Trace("No Organization settings found.");
                dictionary = Utility.RetrieveLanguageLabelCollection(list, config);
                throw new InvalidPluginExecutionException(dictionary["organizationsetting"]);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        private EntityCollection RetrieveOrganizationSettings(PluginConfig config)
        {
            var arg = "RetrieveOrganizationSettings : ";

            try
            {
                config.Tracing.Trace($"Inside {arg}");
                var empty =
                    "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' >\r\n                                <entity name='organization' >\r\n                                    <attribute name='isauditenabled' />\r\n                                    <order attribute='createdon' descending='true' />                                                         \r\n                                </entity>\r\n                            </fetch>";
                return config.Service.RetrieveMultiple(new FetchExpression(empty));
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        private void ValidateEntityAuditingOption(string entityLogicalName, PluginConfig config)
        {
            var arg = "ValidateEntityAuditingOption : ";
            var dictionary = new Dictionary<string, string>();

            try
            {
                config.Tracing.Trace($"Inside {arg}");
                var list = new List<string> { "entityconfiguration" };
                var retrieveEntityResponse = RetrieveEntityMetadata(entityLogicalName, config);
                if (retrieveEntityResponse == null || retrieveEntityResponse.EntityMetadata == null)
                {
                    config.Tracing.Trace("No entity metadata found.");
                    dictionary = Utility.RetrieveLanguageLabelCollection(list, config);
                    throw new InvalidPluginExecutionException(dictionary["entitymetadata"]);
                }

                if (retrieveEntityResponse.EntityMetadata.IsAuditEnabled == null ||
                    !retrieveEntityResponse.EntityMetadata.IsAuditEnabled.Value)
                {
                    config.Tracing.Trace("Auditing is disabled for selected entity.");
                    dictionary = Utility.RetrieveLanguageLabelCollection(list, config);
                    throw new InvalidPluginExecutionException(dictionary["entityaudit"]);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        private RetrieveEntityResponse RetrieveEntityMetadata(string entityLogicalName, PluginConfig config)
        {
            var arg = "RetrieveEntityMetadata : ";

            try
            {
                config.Tracing.Trace($"Inside {arg}");
                var retrieveEntityRequest = new RetrieveEntityRequest();
                retrieveEntityRequest.RetrieveAsIfPublished = false;
                retrieveEntityRequest.LogicalName = entityLogicalName;
                retrieveEntityRequest.EntityFilters = EntityFilters.Attributes;
                return config.Service.Execute(retrieveEntityRequest) as RetrieveEntityResponse;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message);
            }
            catch (InvalidPluginExecutionException ex2)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message);
            }
            catch (Exception ex3)
            {
                config.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message);
            }
        }
    }
}