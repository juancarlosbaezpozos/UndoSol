using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Xml;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace Inogic.Click2Undo.Workflows.Helper
{
    internal static class Utility
    {
        internal static EntityCollection RetrieveAuditDetails(int recordsPerPage, string recordId, string startDate,
            string endDate, string pagingCookie, string selectedAttributeMasks, bool descendingOrder, bool getCount,
            bool getFields, string operation, string objectTypeCode, string action, bool isUndoActionTriggered,
            ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "RetrieveAuditDetails: ";
            var empty = string.Empty;
            EntityCollection entityCollection = null;
            var empty2 = string.Empty;
            try
            {
                SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                SetTrace($"RecordId: {recordId}", ref traceLog, wfConfig);
                SetTrace($"Start Date: {startDate}", ref traceLog, wfConfig);
                SetTrace($"End Date: {endDate}", ref traceLog, wfConfig);
                SetTrace($"Operation: {operation}", ref traceLog, wfConfig);
                SetTrace($"Object Type Code: {objectTypeCode}", ref traceLog, wfConfig);
                SetTrace($"Action: {action}", ref traceLog, wfConfig);
                SetTrace($"IsUndoActionTriggered: {isUndoActionTriggered}", ref traceLog, wfConfig);
                SetTrace($"pagingCookie: {pagingCookie}", ref traceLog, wfConfig);
                SetTrace($"getCount: {getCount.ToString()}", ref traceLog, wfConfig);
                SetTrace($"getFields: {getFields.ToString()}", ref traceLog, wfConfig);
                SetTrace($"descendingOrder: {descendingOrder.ToString()}", ref traceLog, wfConfig);
                SetTrace($"selectedAttributes: {selectedAttributeMasks.ToString()}", ref traceLog, wfConfig);
                pagingCookie = ((pagingCookie != string.Empty && pagingCookie != null)
                    ? pagingCookie.Replace("<", "&lt;").Replace("\"", "&quot;").Replace(">", "&gt;")
                    : pagingCookie);
                SetTrace($"pagingCookie: {pagingCookie}", ref traceLog, wfConfig);
                empty = ((getCount && operation != "3")
                    ? "<fetch version='1.0' output-format='xml-platform' mapping='logical' aggregate='true'>\r\n                            <entity name='audit' >\r\n                            <attribute name='auditid' aggregate='count' alias='count' />\r\n                            <attribute name='attributemask' groupby='true' alias='attributemask' />"
                    : ((operation == "3" && getCount)
                        ? (empty +
                           "<fetch version='1.0' output-format='xml-platform' mapping='logical' returntotalrecordcount='true' >\r\n                                    <entity name='audit' >\r\n                                    <attribute name='objectid' distinct='true' />\r\n                                    <attribute name='objecttypecode' />")
                        : ((!getFields)
                            ? ("<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' " +
                               pagingCookie + " count='" + ((recordsPerPage > 0) ? recordsPerPage.ToString() : "250") +
                               "'>\r\n                             <entity name='audit'>\r\n                             <all-attributes/>")
                            : (empty +
                               "<fetch version='1.0' output-format='xml-platform' mapping='logical' aggregate='true' >\r\n                            <entity name='audit' >\r\n                            <attribute name='attributemask' groupby='true' alias='attributemask' />"))));
                if (operation == "3" && !getCount)
                {
                    empty = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' " +
                            pagingCookie + " count='" + ((recordsPerPage > 0) ? recordsPerPage.ToString() : "250") +
                            "'>\r\n                             <entity name='audit'>\r\n                                <attribute name='attributemask' />\r\n<attribute name='auditid' />\r\n<attribute name='objecttypecode' />\r\n<attribute name='action' /><attribute name='objectid' distinct='true' />";
                }

                if (!getCount && !getFields)
                {
                    empty = ((!descendingOrder)
                        ? (empty + "<order attribute='createdon' descending='false' />")
                        : (empty + "<order attribute='createdon' descending='true' />"));
                }

                empty = empty +
                        "<filter type='and'>                     \r\n                                <condition attribute='operation' operator='eq' value='" +
                        operation + "' />";
                empty = (isUndoActionTriggered
                    ? (empty +
                       "<condition attribute='action' operator='in' > \r\n                                    <value>2</value>\r\n                                    <value>13</value> \r\n                                    <value>4</value>\r\n                                    <value>5</value>\r\n                                   </condition>")
                    : ((operation != "2")
                        ? (empty + $"<condition attribute='action' operator='eq' value='{action}' /> ")
                        : (empty +
                           "<condition attribute='action' operator='in' > \r\n                                    <value>2</value>\r\n                                    <value>13</value> \r\n                                   </condition>")));
                if (operation == "3" && !string.IsNullOrEmpty(objectTypeCode))
                {
                    empty += $"<condition attribute='objecttypecode' operator='eq' value='{objectTypeCode}' />";
                }
                else if (!string.IsNullOrEmpty(recordId))
                {
                    empty += $"<condition attribute='objectid' operator='eq' value='{recordId}' />";
                }

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    empty +=
                        $"<condition attribute='createdon' operator='on-or-after' value='{startDate}' />\r\n                                 <condition attribute='createdon' operator='on-or-before' value='{endDate}' />";
                }
                else if (!isUndoActionTriggered)
                {
                    empty2 = GetDefaultPeriod(ref traceLog, wfConfig);
                    if (string.IsNullOrEmpty(empty2))
                    {
                        SetTrace("Default period not found.", ref traceLog, wfConfig);
                        return null;
                    }

                    empty += $"<condition attribute='createdon' operator='last-x-days' value='{empty2}' />";
                }

                if (operation == "2" && !getFields && !string.IsNullOrEmpty(selectedAttributeMasks))
                {
                    var text = string.Empty;
                    if (selectedAttributeMasks.Split(',').Length != 0)
                    {
                        var array = selectedAttributeMasks.Split(',').Distinct().ToArray();
                        if (array != null && array.Length != 0)
                        {
                            for (var i = 0; i < array.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(array[i]))
                                {
                                    text +=
                                        $"<condition attribute='attributemask' operator='like' value='%,{array[i]},%' />";
                                }
                            }
                        }

                        empty += ((text != string.Empty) ? ("<filter type='or'>" + text + "</filter>") : string.Empty);
                    }
                }

                empty +=
                    "</filter >\r\n                             </entity >\r\n                           </fetch > ";
                SetTrace($"FetchXml: {empty}", ref traceLog, wfConfig);
                SetTrace("Before the FetchXml execute", ref traceLog, wfConfig);
                entityCollection = wfConfig.Service.RetrieveMultiple(new FetchExpression(empty));
                SetTrace("After the FetchXml execute", ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return entityCollection;
        }

        internal static int GetRecordsPerPage(ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetRecordsPerPage: ";
            var result = 0;
            try
            {
                SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                var text = string.Concat(
                    "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>\r\n                   <entity name='usersettings'>\r\n                   <all-attributes/>\r\n                   <filter type='and'>\r\n                   <condition attribute='systemuserid' operator='eq' value='",
                    wfConfig.WorkflowContext.UserId,
                    "' />\r\n                   </filter>\r\n                   </entity>\r\n                   </fetch>");
                SetTrace($"fetchXml: {text} ", ref traceLog, wfConfig);
                var entityCollection = wfConfig.Service.RetrieveMultiple(new FetchExpression(text));
                if (entityCollection != null && entityCollection.Entities.Count > 0)
                {
                    result = (entityCollection.Entities[0].Attributes.Contains("paginglimit")
                        ? entityCollection.Entities[0].GetAttributeValue<int>("paginglimit")
                        : 0);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return result;
        }

        internal static string GetDefaultPeriod(ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetDefaultPeriod: ";
            var empty = string.Empty;
            Entity entity = null;
            try
            {
                SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                entity = RetrieveC2UConfiguration(ref traceLog, wfConfig);
                if (entity == null || entity.Attributes == null || !entity.Attributes.Contains("ikl_defaultperiod"))
                {
                    SetTrace("Defaul period not found.", ref traceLog, wfConfig);
                    return empty;
                }

                return entity.GetAttributeValue<int>("ikl_defaultperiod").ToString();
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        internal static Entity RetrieveC2UConfiguration(ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "RetrieveC2UConfiguration: ";
            var empty = string.Empty;
            EntityCollection entityCollection = null;
            Entity entity = null;
            var dictionary = new Dictionary<string, string>();
            List<string> list = null;
            try
            {
                SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                empty =
                    "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>\r\n                                <entity name = 'ikl_click2undoconfiguration' > \r\n                                 <attribute name = 'ikl_click2undoconfigurationid' />  \r\n                                 <attribute name = 'ikl_name' />                                     \r\n                                 <attribute name = 'ikl_defaultperiod' />     \r\n                                 <order attribute = 'ikl_name' descending = 'false' />        \r\n                                </entity >\r\n                            </fetch >";
                entityCollection = wfConfig.Service.RetrieveMultiple(new FetchExpression(empty));
                if (entityCollection == null || entityCollection.Entities.Count <= 0)
                {
                    SetTrace("Click2Undo configuration not found.", ref traceLog, wfConfig);
                    list = new List<string> { "c2uconfiguration" };
                    dictionary = RetrieveLanguageLabelCollection(list, ref traceLog, wfConfig);
                    throw new InvalidPluginExecutionException(dictionary["c2uconfigurationnotfound"]);
                }

                return entityCollection.Entities[0];
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        internal static AuditDetail GetAuditDetails(Guid auditId, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetAuditDetails: ";
            AuditDetail auditDetail = null;
            try
            {
                SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                var retrieveAuditDetailsRequest = new RetrieveAuditDetailsRequest();
                retrieveAuditDetailsRequest.AuditId = auditId;
                var retrieveAuditDetailsResponse =
                    (RetrieveAuditDetailsResponse)wfConfig.Service.Execute(retrieveAuditDetailsRequest);
                return retrieveAuditDetailsResponse.AuditDetail;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        internal static AttributeAuditDetail GetAttributeAuditDetail(Guid auditEntityId, ref StringBuilder traceLog,
            WorkflowConfig wfConfig)
        {
            var arg = "GetAttributeAuditDetail: ";
            AuditDetail auditDetail = null;
            AttributeAuditDetail attributeAuditDetail = null;
            try
            {
                SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                if (auditEntityId == Guid.Empty)
                {
                    SetTrace("Audit entity id not found.", ref traceLog, wfConfig);
                    return null;
                }

                auditDetail = GetAuditDetails(auditEntityId, ref traceLog, wfConfig);
                if (auditDetail == null || auditDetail.AuditRecord == null)
                {
                    SetTrace("Audit details not found.", ref traceLog, wfConfig);
                    return null;
                }

                SetTrace("Audit details found.", ref traceLog, wfConfig);
                attributeAuditDetail = auditDetail as AttributeAuditDetail;
                if (attributeAuditDetail == null || attributeAuditDetail.AuditRecord == null ||
                    attributeAuditDetail.AuditRecord.Attributes == null ||
                    attributeAuditDetail.AuditRecord.Attributes.Count <= 0)
                {
                    SetTrace("Audit attribute details not found.", ref traceLog, wfConfig);
                    return null;
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return attributeAuditDetail;
        }

        internal static EntityMetadata RetrieveEntityMetadata(string entityLogicalName, ref StringBuilder traceLog,
            WorkflowConfig wfConfig)
        {
            var arg = "RetrieveEntityMetadata: ";
            EntityMetadata entityMetadata = null;
            try
            {
                SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                var retrieveEntityRequest = new RetrieveEntityRequest();
                retrieveEntityRequest.RetrieveAsIfPublished = false;
                retrieveEntityRequest.LogicalName = entityLogicalName;
                retrieveEntityRequest.EntityFilters = EntityFilters.All;
                return ((RetrieveEntityResponse)wfConfig.Service.Execute(retrieveEntityRequest))?.EntityMetadata;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        internal static bool IsValidatForUpdate(EntityMetadata entityMetadata, string attributeName,
            ref string fieldDisplayName, bool getDisplayName, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "IsValidatForUpdate: ";
            var flag = false;
            try
            {
                SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                var list = entityMetadata.Attributes
                    .Where((AttributeMetadata attribute) => attribute.LogicalName == attributeName).ToList();
                if (list != null && list.Count > 0 && list[0].IsValidForUpdate == true)
                {
                    flag = true;
                }
                else if (list != null && list.Count > 0)
                {
                    fieldDisplayName = list[0].DisplayName.UserLocalizedLabel.Label;
                }

                SetTrace($"IsValidForUpdate: {flag}", ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return flag;
        }

        internal static bool IsValidatForUpdateAndAttributeNumber(EntityMetadata entityMetadata, string attributeName,
            string[] attributeMaskColl, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "IsValidatForUpdate: ";
            var flag = false;
            try
            {
                SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                var list = entityMetadata.Attributes
                    .Where((AttributeMetadata attribute) => attribute.LogicalName == attributeName).ToList();
                if (list != null && list.Count > 0 && list[0].IsValidForUpdate == true)
                {
                    flag = true;
                }

                if (attributeMaskColl != null && attributeMaskColl.Length != 0 && list != null && list.Count > 0 &&
                    !attributeMaskColl.Contains(list[0].ColumnNumber.ToString()))
                {
                    flag = false;
                }

                SetTrace($"IsValidForUpdate: {flag}", ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return flag;
        }

        internal static Dictionary<string, string> RetrieveLanguageLabelCollection(List<string> mapTypes,
            ref StringBuilder traceLog, WorkflowConfig wfConfig, string localeId = null, int counter = 0)
        {
            var arg = "RetrieveLanguageLabelCollection: ";
            var dictionary = new Dictionary<string, string>();
            EntityCollection entityCollection = null;
            ColumnSet columnSet = null;
            var empty = string.Empty;
            var text = "1033";
            try
            {
                SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                text = GetBaseLanguageCode(ref traceLog, wfConfig);
                entityCollection = new EntityCollection();
                columnSet = new ColumnSet("name", "content");
                empty = ((counter != 0)
                    ? GetWebResourceName(text, ref traceLog, wfConfig)
                    : GetWebResourceName(string.Empty, ref traceLog, wfConfig));
                var text2 = ReadWebResource(empty, columnSet, ref traceLog, wfConfig);
                if (!string.IsNullOrEmpty(text2))
                {
                    dictionary =
                        CreateLanguageCollection(text2, "/langlabels/maptype", mapTypes, ref traceLog, wfConfig);
                }
                else if (counter == 0)
                {
                    dictionary = RetrieveLanguageLabelCollection(mapTypes, ref traceLog, wfConfig, text, 1);
                }

                if (dictionary != null)
                {
                    SetTrace($"languageCollection length: {dictionary.Count}", ref traceLog, wfConfig);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return dictionary;
        }

        public static string GetBaseLanguageCode(ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetBaseLanguageCode: ";
            var result = "1033";
            EntityCollection entityCollection = null;
            try
            {
                SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                var queryExpression = new QueryExpression("organization");
                queryExpression.ColumnSet = new ColumnSet("languagecode");
                var query = queryExpression;
                entityCollection = wfConfig.Service.RetrieveMultiple(query);
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
                SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return result;
        }

        private static string GetWebResourceName(string localeId, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetWebResourceName: ";
            var empty = string.Empty;
            try
            {
                SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                if (string.IsNullOrEmpty(localeId) || string.IsNullOrWhiteSpace(localeId))
                {
                    localeId = GetUserUILanguageID(ref traceLog, wfConfig);
                }

                empty = $"ikl_/Click2Undo/Languages/LanguageLabel_{localeId}.xml";
                SetTrace($"Web resource name: {empty}", ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return empty;
        }

        private static string GetUserUILanguageID(ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetUserUILanguageID: ";
            var text = string.Empty;
            var empty = string.Empty;
            var empty2 = Guid.Empty;
            try
            {
                SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                empty2 = wfConfig.WorkflowContext.InitiatingUserId;
                empty =
                    $"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' count='1'>\r\n                              <entity name='usersettings'>\r\n                                <attribute name='systemuserid' />\r\n                                <attribute name='uilanguageid' />\r\n                                <filter type='and'>\r\n                                  <condition attribute='systemuserid' operator='eq' value='{empty2}' />\r\n                                </filter>\r\n                              </entity>\r\n                            </fetch>";
                SetTrace("Before Retrieve.", ref traceLog, wfConfig);
                var entityCollection = wfConfig.Service.RetrieveMultiple(new FetchExpression(empty));
                SetTrace("After Retrieve.", ref traceLog, wfConfig);
                if (entityCollection != null && entityCollection.Entities != null &&
                    entityCollection.Entities.Count > 0)
                {
                    var entity = entityCollection.Entities.First();
                    text = entity.GetAttributeValue<int>("uilanguageid").ToString();
                    SetTrace($"Language Id: {text}", ref traceLog, wfConfig);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return text;
        }

        private static string ReadWebResource(string webResourceName, ColumnSet columnSet, ref StringBuilder traceLog,
            WorkflowConfig wfConfig)
        {
            var arg = "ReadWebResource: ";
            var result = string.Empty;
            try
            {
                SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                var entityCollection = GetEntityCollection("webresource", "name", webResourceName,
                    columnSet, ref traceLog, wfConfig);
                if (entityCollection != null && entityCollection.Entities.Count > 0 &&
                    entityCollection.Entities[0].Attributes.Count > 0)
                {
                    result = ConvertContentToXMLString(entityCollection, ref traceLog, wfConfig);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return result;
        }

        private static EntityCollection GetEntityCollection(string entityName, string attributeName,
            string attributeValue, ColumnSet cols, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetEntityCollection: ";
            EntityCollection entityCollection = null;
            try
            {
                SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
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
                                Values = { (object)attributeValue }
                            }
                        }
                    }
                };
                SetTrace("Before Retrieving.", ref traceLog, wfConfig);
                entityCollection = wfConfig.Service.RetrieveMultiple(query);
                SetTrace("After Retrieving.", ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return entityCollection;
        }

        private static string ConvertContentToXMLString(EntityCollection entCol, ref StringBuilder traceLog,
            WorkflowConfig wfConfig)
        {
            var arg = "ConvertContentToXMLString: ";
            Entity entity = null;
            var empty = string.Empty;
            var result = string.Empty;
            try
            {
                SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                entity = entCol.Entities[0];
                if (entity.Attributes.Contains("content"))
                {
                    var bytes = Convert.FromBase64String(entity.Attributes["content"].ToString());
                    empty = Encoding.UTF8.GetString(bytes);
                    result = WebUtility.HtmlDecode(empty);
                }
                else
                {
                    SetTrace("Web Resource doesn't have content attribute", ref traceLog, wfConfig);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return result;
        }

        private static Dictionary<string, string> CreateLanguageCollection(string webResourceContent, string nodeString,
            List<string> mapTypes, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "CreateLanguageCollection: ";
            Dictionary<string, string> dictionary = null;
            try
            {
                SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                SetTrace($"Node String: {nodeString}", ref traceLog, wfConfig);
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(webResourceContent);
                var xmlNodeList = xmlDocument.SelectNodes(nodeString);
                if (xmlNodeList.Count > 0)
                {
                    SetTrace("Node List Count greater than 0", ref traceLog, wfConfig);
                    dictionary = new Dictionary<string, string>();
                    foreach (XmlNode item in xmlNodeList)
                    {
                        if (item == null || item.Attributes == null || item.Attributes.Count <= 0)
                        {
                            SetTrace($"{arg}: Attribute for maptype not found", ref traceLog, wfConfig);
                            continue;
                        }

                        var mapTypeName = item.Attributes["name"].Value;
                        mapTypeName = ((!string.IsNullOrEmpty(mapTypeName) && !string.IsNullOrWhiteSpace(mapTypeName))
                            ? mapTypeName.Trim()
                            : mapTypeName);
                        SetTrace($"Map Type Name: {mapTypeName}", ref traceLog, wfConfig);
                        if (!mapTypes.Any((string x) => x == mapTypeName))
                        {
                            continue;
                        }

                        SetTrace("Map Type Name matched", ref traceLog, wfConfig);
                        foreach (XmlNode item2 in item)
                        {
                            if (item2 == null || item2.Attributes == null || item2.Attributes.Count <= 0)
                            {
                                SetTrace($"{arg}: Attribute data not found", ref traceLog, wfConfig);
                                continue;
                            }

                            var value = item2.Attributes["name"].Value;
                            value = ((!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
                                ? value.Trim()
                                : value);
                            SetTrace($"Data Name: {value}", ref traceLog, wfConfig);
                            var innerText = item2.InnerText;
                            innerText = ((!string.IsNullOrEmpty(innerText) && !string.IsNullOrWhiteSpace(innerText))
                                ? innerText.Trim()
                                : innerText);
                            SetTrace($"Data Value: {innerText}", ref traceLog, wfConfig);
                            dictionary[value] = innerText;
                        }
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return dictionary;
        }

        internal static string GetConvertedDate(DateTime currentDate, ref StringBuilder traceLog,
            WorkflowConfig wfConfig)
        {
            var arg = "GetConvertedDate: ";
            var empty = string.Empty;
            var empty2 = Guid.Empty;
            try
            {
                SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                empty2 = ((wfConfig != null && wfConfig.WorkflowContext != null &&
                           wfConfig.WorkflowContext.InitiatingUserId != Guid.Empty)
                    ? wfConfig.WorkflowContext.InitiatingUserId
                    : Guid.Empty);
                if (empty2 == Guid.Empty)
                {
                    SetTrace("Loggedin user not found.", ref traceLog, wfConfig);
                    return "";
                }

                var timeZoneCode = RetrieveCurrentUsersSettings(empty2, ref traceLog, wfConfig);
                SetTrace($"currentDate {currentDate}.", ref traceLog, wfConfig);
                var dateTime = RetrieveLocalTimeFromUTCTime(currentDate, timeZoneCode, ref traceLog, wfConfig);
                SetTrace($"convertedDate {dateTime}.", ref traceLog, wfConfig);
                return dateTime.ToString();
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                SetTrace($"Exited from: {arg}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                SetTrace($"Exited from: {arg}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                SetTrace($"Exited from: {arg}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        internal static int? RetrieveCurrentUsersSettings(Guid userID, ref StringBuilder traceLog,
            WorkflowConfig wfConfig)
        {
            var arg = "RetrieveCurrentUsersSettings: ";
            try
            {
                var entity = wfConfig.Service.RetrieveMultiple(new QueryExpression("usersettings")
                {
                    ColumnSet = new ColumnSet("timezonecode"),
                    Criteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression("systemuserid", ConditionOperator.Equal, userID)
                        }
                    }
                }).Entities[0].ToEntity<Entity>();
                return (int?)entity.Attributes["timezonecode"];
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                SetTrace($"Exited from: {arg}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                SetTrace($"Exited from: {arg}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                SetTrace($"Exited from: {arg}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        internal static DateTime RetrieveLocalTimeFromUTCTime(DateTime utcTime, int? timeZoneCode,
            ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "RetrieveLocalTimeFromUTCTime: ";
            try
            {
                if (!timeZoneCode.HasValue)
                {
                    return DateTime.Now;
                }

                var request = new LocalTimeFromUtcTimeRequest
                {
                    TimeZoneCode = timeZoneCode.Value,
                    UtcTime = utcTime.ToUniversalTime()
                };
                var localTimeFromUtcTimeResponse =
                    (LocalTimeFromUtcTimeResponse)wfConfig.Service.Execute(request);
                return localTimeFromUtcTimeResponse.LocalTime;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                SetTrace($"Exited from: {arg}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                SetTrace($"Exited from: {arg}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                SetTrace($"Exited from: {arg}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        public static void SetTrace(string message, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "Trace: ";
            try
            {
                wfConfig.Tracing.Trace(message);
                traceLog.Append($" {Environment.NewLine}{message}");
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                wfConfig.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                wfConfig.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                wfConfig.Tracing.Trace($"Exited from: {arg}.");
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }
    }
}