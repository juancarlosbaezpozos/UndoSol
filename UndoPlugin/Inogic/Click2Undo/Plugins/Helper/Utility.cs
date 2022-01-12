using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Xml;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Inogic.Click2Undo.Plugins.Helper
{
    internal static class Utility
    {
        internal static Dictionary<string, string> RetrieveLanguageLabelCollection(List<string> mapTypes,
            PluginConfig config, string localeId = null, int counter = 0)
        {
            var arg = "RetrieveLanguageLabelCollection";
            var result = new Dictionary<string, string>();

            try
            {
                config.Tracing.Trace($"Inside {arg}");
                var text = GetBaseLanguageCode(config);
                var entityCollection = new EntityCollection();
                var columnSet = new ColumnSet("name", "content");
                var empty = ((counter != 0)
                    ? GetWebResourceName(text, config)
                    : GetWebResourceName(string.Empty, config));
                var text2 = ReadWebResource(empty, columnSet, config);
                config.Tracing.Trace($"Content {text2}");
                if (!string.IsNullOrEmpty(text2))
                {
                    result = CreateLanguageCollection(text2, "/langlabels/maptype", mapTypes, config);
                }
                else if (counter == 0)
                {
                    result = RetrieveLanguageLabelCollection(mapTypes, config, text, 1);
                }
            }
            catch (InvalidPluginExecutionException ex)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message);
            }
            catch (FaultException<OrganizationServiceFault> ex2)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message);
            }
            catch (Exception ex3)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message);
            }

            return result;
        }

        private static string GetWebResourceName(string localeId, PluginConfig config)
        {
            var arg = "GetWebResourceName";
            string empty;

            try
            {
                config.Tracing.Trace($"Inside {arg}");
                if (string.IsNullOrEmpty(localeId) || string.IsNullOrWhiteSpace(localeId))
                {
                    localeId = GetUserUILanguageID(config);
                }

                empty = $"ikl_/Click2Undo/Languages/LanguageLabel_{localeId}.xml";
                config.Tracing.Trace($"{arg}: {empty}");
            }
            catch (InvalidPluginExecutionException ex)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message);
            }
            catch (FaultException<OrganizationServiceFault> ex2)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message);
            }
            catch (Exception ex3)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message);
            }

            return empty;
        }

        private static string GetUserUILanguageID(PluginConfig config)
        {
            var arg = "GetUserUILanguageID";
            var text = string.Empty;

            try
            {
                config.Tracing.Trace($"Inside {arg}");
                var empty2 = config.PluginContext.InitiatingUserId;
                var empty =
                    $"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' count='1'>\r\n                              <entity name='usersettings'>\r\n                                <attribute name='systemuserid' />\r\n                                <attribute name='uilanguageid' />\r\n                                <filter type='and'>\r\n                                  <condition attribute='systemuserid' operator='eq' value='{empty2}' />\r\n                                </filter>\r\n                              </entity>\r\n                            </fetch>";
                config.Tracing.Trace($"{arg}: Before Retrieve.");
                var entityCollection = config.Service.RetrieveMultiple(new FetchExpression(empty));
                config.Tracing.Trace($"{arg}: After Retrieve.");
                if (entityCollection != null && entityCollection.Entities != null &&
                    entityCollection.Entities.Count > 0)
                {
                    var entity = entityCollection.Entities.First();
                    text = entity.GetAttributeValue<int>("uilanguageid").ToString();
                    config.Tracing.Trace($"Language Id: {text}");
                }
            }
            catch (InvalidPluginExecutionException ex)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message);
            }
            catch (FaultException<OrganizationServiceFault> ex2)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message);
            }
            catch (Exception ex3)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message);
            }

            return text;
        }

        private static string ReadWebResource(string webResourceName, ColumnSet columnSet, PluginConfig config)
        {
            var arg = "ReadWebResource";
            var result = string.Empty;

            try
            {
                config.Tracing.Trace($"Inside {arg}");
                var entityCollection =
                    GetEntityCollection("webresource", "name", webResourceName, columnSet, config);
                if (entityCollection != null && entityCollection.Entities.Count > 0 &&
                    entityCollection.Entities[0].Attributes.Count > 0)
                {
                    result = ConvertContentToXMLString(entityCollection, config);
                }
            }
            catch (InvalidPluginExecutionException ex)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message);
            }
            catch (FaultException<OrganizationServiceFault> ex2)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message);
            }
            catch (Exception ex3)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message);
            }

            return result;
        }

        private static EntityCollection GetEntityCollection(string entityName, string attributeName,
            string attributeValue, ColumnSet cols, PluginConfig config)
        {
            var arg = "GetEntityCollection";
            EntityCollection entityCollection;

            try
            {
                config.Tracing.Trace($"Inside {arg}");
                entityCollection = new EntityCollection();
                var query = new QueryExpression
                {
                    EntityName = entityName,
                    ColumnSet = cols,
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression
                            {
                                AttributeName = attributeName,
                                Operator = ConditionOperator.Equal,
                                Values = { attributeValue }
                            }
                        }
                    }
                };
                config.Tracing.Trace($"{arg}: Before Retrieving.");
                entityCollection = config.Service.RetrieveMultiple(query);
                config.Tracing.Trace($"{arg}: After Retrieving.");
            }
            catch (InvalidPluginExecutionException ex)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message);
            }
            catch (FaultException<OrganizationServiceFault> ex2)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message);
            }
            catch (Exception ex3)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message);
            }

            return entityCollection;
        }

        private static string ConvertContentToXMLString(EntityCollection entCol, PluginConfig config)
        {
            var arg = "ConvertContentToXMLString";
            var result = string.Empty;

            try
            {
                config.Tracing.Trace($"Inside {arg}");
                var entity = entCol.Entities[0];
                if (entity.Attributes.Contains("content"))
                {
                    var bytes = Convert.FromBase64String(entity.Attributes["content"].ToString());
                    var empty = Encoding.UTF8.GetString(bytes);
                    result = WebUtility.HtmlDecode(empty);
                }
                else
                {
                    config.Tracing.Trace($"{arg}: Web Resource doesn't have content attribute");
                }
            }
            catch (InvalidPluginExecutionException ex)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message);
            }
            catch (FaultException<OrganizationServiceFault> ex2)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message);
            }
            catch (Exception ex3)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message);
            }

            return result;
        }

        private static Dictionary<string, string> CreateLanguageCollection(string webResourceContent, string nodeString,
            List<string> mapTypes, PluginConfig config)
        {
            var arg = "CreateLanguageCollection";
            Dictionary<string, string> dictionary = null;

            try
            {
                config.Tracing.Trace($"Inside {arg}");
                config.Tracing.Trace($"{arg}: Node String: {nodeString}");
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(webResourceContent);
                var xmlNodeList = xmlDocument.SelectNodes(nodeString);
                if (xmlNodeList.Count > 0)
                {
                    config.Tracing.Trace($"{arg}: Node List Count greater than 0");
                    dictionary = new Dictionary<string, string>();
                    foreach (XmlNode item in xmlNodeList)
                    {
                        if (item == null || item.Attributes == null || item.Attributes.Count <= 0)
                        {
                            config.Tracing.Trace($"{arg}: Attribute for maptype not found");
                            continue;
                        }

                        var mapTypeName = item.Attributes["name"].Value;
                        mapTypeName = ((!string.IsNullOrEmpty(mapTypeName) && !string.IsNullOrWhiteSpace(mapTypeName))
                            ? mapTypeName.Trim()
                            : mapTypeName);
                        config.Tracing.Trace($"{arg}: Map Type Name: {mapTypeName}");
                        if (!mapTypes.Any((string x) => x == mapTypeName))
                        {
                            continue;
                        }

                        config.Tracing.Trace($"{arg}: Map Type Name matched");
                        foreach (XmlNode item2 in item)
                        {
                            if (item2 == null || item2.Attributes == null || item2.Attributes.Count <= 0)
                            {
                                config.Tracing.Trace($"{arg}: Attribute data not found");
                                continue;
                            }

                            var value = item2.Attributes["name"].Value;
                            value = ((!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
                                ? value.Trim()
                                : value);
                            config.Tracing.Trace($"{arg}: Data Name: {value}");
                            var innerText = item2.InnerText;
                            innerText = ((!string.IsNullOrEmpty(innerText) && !string.IsNullOrWhiteSpace(innerText))
                                ? innerText.Trim()
                                : innerText);
                            config.Tracing.Trace($"{arg}: Data Name: {innerText}");
                            dictionary[value] = innerText;
                        }
                    }
                }
            }
            catch (InvalidPluginExecutionException ex)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message);
            }
            catch (FaultException<OrganizationServiceFault> ex2)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message);
            }
            catch (Exception ex3)
            {
                config.Tracing.Trace($"Exited from {arg}.");
                throw new InvalidPluginExecutionException(
                    (ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message);
            }

            return dictionary;
        }

        public static string GetBaseLanguageCode(PluginConfig config)
        {
            var arg = "GetBaseLanguageCode: ";
            var result = "1033";

            try
            {
                config.Tracing.Trace($"{arg}");
                var queryExpression = new QueryExpression("organization");
                queryExpression.ColumnSet = new ColumnSet("languagecode");
                var query = queryExpression;
                var entityCollection = config.Service.RetrieveMultiple(query);
                if (entityCollection != null && entityCollection.Entities.Count > 0)
                {
                    result = ((entityCollection[0].Contains("languagecode") &&
                               entityCollection[0].Attributes["languagecode"] != null)
                        ? entityCollection[0].GetAttributeValue<int>("languagecode").ToString()
                        : "1033");
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

            return result;
        }

        public static string GetUserLanguageCode(PluginConfig config)
        {
            var arg = "GetUserLanguageCode: ";
            var text = "1033";

            try
            {
                config.Tracing.Trace($"{arg}");
                var queryExpression = new QueryExpression("usersettings");
                queryExpression.ColumnSet = new ColumnSet("localeid");
                queryExpression.Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression("systemuserid", ConditionOperator.Equal,
                            config.PluginContext.InitiatingUserId)
                    }
                };
                var query = queryExpression;
                var entityCollection = config.Service.RetrieveMultiple(query);
                if (entityCollection != null && entityCollection.Entities.Count > 0)
                {
                    config.Tracing.Trace("User settings found.");
                    text = ((entityCollection[0].Contains("localeid") &&
                             entityCollection[0].Attributes["localeid"] != null)
                        ? entityCollection[0].GetAttributeValue<int>("localeid").ToString()
                        : "1033");
                }

                config.Tracing.Trace($"User's language code: {text}");
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

            return text;
        }
    }
}