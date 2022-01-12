using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Inogic.Click2Undo.Workflows.Helper;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

namespace Inogic.Click2Undo.Workflows
{
    public class RestoreLastChange : CodeActivity
    {
        [RequiredArgument] [Input("RecordId")] public InArgument<string> INRECORDID { get; set; }

        [RequiredArgument]
        [Input("EntityName")]
        public InArgument<string> INENTITYNAME { get; set; }

        [Output("DisplayMessage")] public OutArgument<string> OUTDISPLAYMESSAGE { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var text = "Execute: ";
            var arg = "RestoreLastChange";
            WorkflowConfig workflowConfig = null;
            var traceLog = new StringBuilder();
            var dictionary = new Dictionary<string, string>();

            try
            {
                workflowConfig = new WorkflowConfig(context);
                Utility.SetTrace($"{arg} {text}", ref traceLog, workflowConfig);
                Utility.SetTrace($"Depth {workflowConfig.WorkflowContext.Depth}", ref traceLog, workflowConfig);
                Utility.SetTrace($"InitiatingUserId: {workflowConfig.WorkflowContext.InitiatingUserId} ", ref traceLog,
                    workflowConfig);
                Utility.SetTrace($"UserId: {workflowConfig.WorkflowContext.UserId} ", ref traceLog, workflowConfig);
                Utility.SetTrace($"OrganizationId: {workflowConfig.WorkflowContext.OrganizationId} ", ref traceLog,
                    workflowConfig);
                if (INRECORDID == null || INRECORDID.Get<string>(context) == null || INENTITYNAME == null ||
                    INENTITYNAME.Get<string>(context) == null)
                {
                    Utility.SetTrace("RECORDID or ENTITYNAME are not found", ref traceLog, workflowConfig);
                    return;
                }

                var empty = INRECORDID.Get<string>(context);
                Utility.SetTrace($"RecordId {empty}", ref traceLog, workflowConfig);
                var empty2 = INENTITYNAME.Get<string>(context);
                Utility.SetTrace($"EntityName {empty2}", ref traceLog, workflowConfig);
                ProcessLastChangeRestore(empty, empty2, context, ref traceLog, workflowConfig);
                Utility.SetTrace("RestoreLastChange Execute function completed", ref traceLog, workflowConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {text} FaultException: {ex.Message}.", ref traceLog, workflowConfig);
                var arg2 = ((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
                throw new InvalidPluginExecutionException(
                    $"AssemblyErrorMessage: {arg2} AssemblyTracing: {traceLog.ToString()}");
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {text} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    workflowConfig);
                var arg3 = ((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
                throw new InvalidPluginExecutionException(
                    $"AssemblyErrorMessage: {arg3} AssemblyTracing: {traceLog.ToString()}");
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {text} Exception: {ex3.Message}.", ref traceLog, workflowConfig);
                var arg4 = ((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
                throw new InvalidPluginExecutionException(
                    $"AssemblyErrorMessage: {arg4} AssemblyTracing: {traceLog.ToString()}");
            }
        }

        private void ProcessLastChangeRestore(string recordId, string entityName, CodeActivityContext context,
            ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "ProcessLastChangeRestore: ";
            var dictionary = new Dictionary<string, string>();

            try
            {
                Utility.SetTrace($"{arg}", ref traceLog, wfConfig);
                var entityCollection = Utility.RetrieveAuditDetails(1, recordId, string.Empty, string.Empty,
                    string.Empty, string.Empty, descendingOrder: true, getCount: false, getFields: false, "2",
                    string.Empty, "2", isUndoActionTriggered: true, ref traceLog, wfConfig);
                if (entityCollection == null || entityCollection.Entities == null ||
                    entityCollection.Entities.Count <= 0)
                {
                    Utility.SetTrace("No audit record found", ref traceLog, wfConfig);
                    var list = new List<string> { "click2uondobutton" };
                    dictionary = Utility.RetrieveLanguageLabelCollection(list, ref traceLog, wfConfig);
                    if (dictionary.Count > 0 && dictionary.ContainsKey("noauditdetailsfound"))
                    {
                        throw new InvalidPluginExecutionException(dictionary["noauditdetailsfound"]);
                    }
                }

                var auditDetails = Utility.GetAuditDetails(entityCollection[0].Id, ref traceLog, wfConfig);
                if (auditDetails == null || auditDetails.GetType() != typeof(AttributeAuditDetail) ||
                    auditDetails.AuditRecord == null || auditDetails.AuditRecord.Attributes == null ||
                    auditDetails.AuditRecord.Attributes.Count <= 0)
                {
                    Utility.SetTrace("No audit detail found", ref traceLog, wfConfig);
                    return;
                }

                var attributeAuditDetail = auditDetails as AttributeAuditDetail;
                if (attributeAuditDetail == null)
                {
                    Utility.SetTrace("No attribute audit detail found", ref traceLog, wfConfig);
                }
                else
                {
                    ValidateAndRestoreData(auditDetails, attributeAuditDetail, recordId, entityName, context,
                        ref traceLog, wfConfig);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg}", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg}", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg}", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        private Entity RetrieveLastAudit(string recordId, string entityName, ref StringBuilder traceLog,
            WorkflowConfig wfConfig)
        {
            var arg = "RetrieveLastAudit: ";
            Entity result = null;

            try
            {
                Utility.SetTrace($"{arg}", ref traceLog, wfConfig);
                var empty =
                    "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false' count='1'>\r\n                             <entity name='audit'>\r\n                             <all-attributes/>\r\n                               <order attribute='createdon' descending='true' />\r\n                               <filter type='and'>\r\n                                 <condition attribute='objectid' operator='eq' value='" +
                    recordId +
                    "' />\r\n                                  <condition attribute='operation' operator='eq' value='2'></condition> \r\n                                  </filter> \r\n                             </entity> \r\n                           </fetch>";
                Utility.SetTrace("Retrieving Last Audit record", ref traceLog, wfConfig);
                var entityCollection = wfConfig.Service.RetrieveMultiple(new FetchExpression(empty));
                if (entityCollection != null && entityCollection.Entities.Count > 0)
                {
                    Utility.SetTrace("Found Audit record", ref traceLog, wfConfig);
                    result = entityCollection.Entities[0];
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg}", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg}", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg}", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return result;
        }

        private void ValidateAndRestoreData(AuditDetail auditDetail, AttributeAuditDetail attributeAuditDetail,
            string recordId, string entityName, CodeActivityContext context, ref StringBuilder traceLog,
            WorkflowConfig wfConfig)
        {
            var arg = "ValidateAndRestoreData: ";

            try
            {
                Utility.SetTrace($"{arg}", ref traceLog, wfConfig);
                if ((attributeAuditDetail.NewValue == null || attributeAuditDetail.NewValue.Attributes == null ||
                     attributeAuditDetail.NewValue.Attributes.Count <= 0) && (attributeAuditDetail.OldValue == null ||
                                                                              attributeAuditDetail.OldValue
                                                                                  .Attributes == null ||
                                                                              attributeAuditDetail.OldValue.Attributes
                                                                                  .Count <= 0))
                {
                    Utility.SetTrace("attribute does not have New Value as well as old value", ref traceLog, wfConfig);
                    SetDisplayMessage("click2uondobutton", "nodatafoundtorestore", context, ref traceLog, wfConfig);
                }
                else if
                    ((auditDetail.AuditRecord.Contains("action") &&
                      auditDetail.AuditRecord.GetAttributeValue<OptionSetValue>("action") != null &&
                      (auditDetail.AuditRecord.GetAttributeValue<OptionSetValue>("action").Value == 4 ||
                       auditDetail.AuditRecord.GetAttributeValue<OptionSetValue>("action").Value == 5)) ||
                     (attributeAuditDetail.OldValue.Attributes.Count == 1 &&
                      attributeAuditDetail.NewValue.Attributes.Count == 1 &&
                      attributeAuditDetail.OldValue.Attributes.Contains("statecode") &&
                      attributeAuditDetail.NewValue.Attributes.Contains("statecode")) ||
                     (attributeAuditDetail.OldValue.Attributes.Count == 1 &&
                      attributeAuditDetail.NewValue.Attributes.Count == 1 &&
                      attributeAuditDetail.OldValue.Attributes.Contains("statuscode") &&
                      attributeAuditDetail.NewValue.Attributes.Contains("statuscode")) ||
                     (attributeAuditDetail.OldValue.Attributes.Count == 2 &&
                      attributeAuditDetail.NewValue.Attributes.Count == 2 &&
                      attributeAuditDetail.OldValue.Attributes.Contains("statuscode") &&
                      attributeAuditDetail.OldValue.Attributes.Contains("statecode") &&
                      attributeAuditDetail.NewValue.Attributes.Contains("statuscode") &&
                      attributeAuditDetail.NewValue.Attributes.Contains("statecode")))
                {
                    Utility.SetTrace("Last update was status.", ref traceLog, wfConfig);
                    SetDisplayMessage("click2uondobutton", "lastupdatewasstatus", context, ref traceLog, wfConfig);
                }
                else
                {
                    UpdateLastChange(attributeAuditDetail, recordId, entityName, context, ref traceLog, wfConfig);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg}", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg}", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg}", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        private void UpdateLastChange(AttributeAuditDetail attributeAuditDetail, string recordId, string entityName,
            CodeActivityContext context, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "UpdateLastChange: ";
            var num = 0;
            var source = new string[13]
            {
                "address1_composite", "address2_composite", "fullname", "billto_composite", "shipto_composite",
                "salesstage", "stepname", "totallineitemamount", "totalamountlessfreight", "totalamount",
                "totaldiscountamount", "totaltax", "totallineitemdiscountamount"
            };
            var text = string.Empty;
            var fieldDisplayName = string.Empty;

            try
            {
                Utility.SetTrace($"{arg}", ref traceLog, wfConfig);
                var entityMetadata = Utility.RetrieveEntityMetadata(entityName, ref traceLog, wfConfig);
                if (entityMetadata == null)
                {
                    Utility.SetTrace("Entity metadata not found.", ref traceLog, wfConfig);
                    return;
                }

                var entity = new Entity(entityName, new Guid(recordId));
                Utility.SetTrace("loop through the new attribute values", ref traceLog, wfConfig);
                foreach (var attribute in attributeAuditDetail.NewValue.Attributes)
                {
                    if (!entity.Attributes.Contains(attribute.Key) && !source.Contains(attribute.Key))
                    {
                        num++;
                        Utility.SetTrace($"Attribute count {num}", ref traceLog, wfConfig);
                    }

                    Utility.SetTrace("Attribute Name:" + attribute.Key, ref traceLog, wfConfig);
                    if (!Utility.IsValidatForUpdate(entityMetadata, attribute.Key, ref fieldDisplayName,
                            getDisplayName: true, ref traceLog, wfConfig) || attribute.Key.ToLower() == "statecode" ||
                        attribute.Key.ToLower() == "statuscode" || source.Contains(attribute.Key))
                    {
                        Utility.SetTrace("Skipping " + attribute.Key.ToLower() + " field", ref traceLog, wfConfig);
                        text = ((text == string.Empty) ? fieldDisplayName : ("," + fieldDisplayName));
                    }
                    else if (attributeAuditDetail.OldValue.Contains(attribute.Key))
                    {
                        Utility.SetTrace("Attribute Name: " + attribute.Key + " contains data", ref traceLog, wfConfig);
                        entity[attribute.Key] = attributeAuditDetail.OldValue[attribute.Key];
                    }
                    else
                    {
                        Utility.SetTrace("Attribute Name: " + attribute.Key + " is null", ref traceLog, wfConfig);
                        entity[attribute.Key] = null;
                    }
                }

                Utility.SetTrace("loop through the old attribute values", ref traceLog, wfConfig);
                foreach (var attribute2 in attributeAuditDetail.OldValue.Attributes)
                {
                    if (!entity.Attributes.Contains(attribute2.Key) && !source.Contains(attribute2.Key) &&
                        !attributeAuditDetail.NewValue.Attributes.Contains(attribute2.Key))
                    {
                        num++;
                    }

                    Utility.SetTrace("Attribute Name:" + attribute2.Key, ref traceLog, wfConfig);
                    if (!Utility.IsValidatForUpdate(entityMetadata, attribute2.Key, ref fieldDisplayName,
                            getDisplayName: true, ref traceLog, wfConfig) || attribute2.Key.ToLower() == "statecode" ||
                        attribute2.Key.ToLower() == "statuscode" || source.Contains(attribute2.Key))
                    {
                        Utility.SetTrace("Skipping " + attribute2.Key.ToLower() + " field", ref traceLog, wfConfig);
                        text = ((text == string.Empty) ? fieldDisplayName : ("," + fieldDisplayName));
                    }
                    else if (attributeAuditDetail.OldValue.Contains(attribute2.Key))
                    {
                        Utility.SetTrace("Attribute Name: " + attribute2.Key + " contains data", ref traceLog,
                            wfConfig);
                        entity[attribute2.Key] = attributeAuditDetail.OldValue[attribute2.Key];
                    }
                    else
                    {
                        Utility.SetTrace("Attribute Name: " + attribute2.Key + " is null", ref traceLog, wfConfig);
                        entity[attribute2.Key] = null;
                    }
                }

                Utility.SetTrace("Before Updating record", ref traceLog, wfConfig);
                if (entity != null && entity.Attributes.Count > 0)
                {
                    wfConfig.Service.Update(entity);
                    Utility.SetTrace("After Updating record", ref traceLog, wfConfig);
                    Utility.SetTrace($"Updated attribute count: {entity.Attributes.Count}", ref traceLog, wfConfig);
                }

                Utility.SetTrace($"Total attribute count: {num}", ref traceLog, wfConfig);
                CountUpdatedAttributeStatus(entity, num, text, context, ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg}", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg}", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg}", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        private void CountUpdatedAttributeStatus(Entity updatedRecord, int totalAttributeCount,
            string skippedFieldNames, CodeActivityContext context, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "SetOutputParameter: ";
            var dictionary = new Dictionary<string, string>();

            try
            {
                Utility.SetTrace($"{arg}", ref traceLog, wfConfig);
                if (totalAttributeCount == 0)
                {
                    Utility.SetTrace("No attributes found to update.", ref traceLog, wfConfig);
                    SetDisplayMessage("click2uondobutton", "nodatafoundtorestore", context, ref traceLog, wfConfig);
                }
                else if (totalAttributeCount > updatedRecord.Attributes.Count)
                {
                    var list = new List<string> { "click2uondobutton" };
                    dictionary = Utility.RetrieveLanguageLabelCollection(list, ref traceLog, wfConfig);
                    if (dictionary.Count > 0 && dictionary.ContainsKey("lastundostatus"))
                    {
                        var arg2 = string.Format(dictionary["lastundostatus"], updatedRecord.Attributes.Count,
                            totalAttributeCount, totalAttributeCount - updatedRecord.Attributes.Count);
                        Utility.SetTrace($"DisplayMessage: {arg2}", ref traceLog, wfConfig);
                        var empty = ((skippedFieldNames != string.Empty)
                            ? ("<SkippedFieldNames>" + skippedFieldNames + " <SkippedFieldNames>")
                            : string.Empty);
                        OUTDISPLAYMESSAGE.Set(context,
                            $"AssemblyErrorMessage: {arg2} AssemblyTracing:{empty} {traceLog.ToString()}");
                        Utility.SetTrace("After setting output parameter.", ref traceLog, wfConfig);
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg}", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg}", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg}", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        private void SetDisplayMessage(string strMapType, string strDataName, CodeActivityContext context,
            ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "SetDisplayMessage: ";
            var dictionary = new Dictionary<string, string>();

            try
            {
                Utility.SetTrace($"{arg}", ref traceLog, wfConfig);
                if (string.IsNullOrEmpty(strMapType) || string.IsNullOrEmpty(strDataName))
                {
                    Utility.SetTrace("Maptype and data name not found.", ref traceLog, wfConfig);
                    return;
                }

                var list = new List<string> { strMapType };
                dictionary = Utility.RetrieveLanguageLabelCollection(list, ref traceLog, wfConfig);
                if (dictionary.Count > 0 && dictionary.ContainsKey(strDataName))
                {
                    Utility.SetTrace($"DisplayMessage: {dictionary[strDataName]}", ref traceLog, wfConfig);
                    throw new InvalidPluginExecutionException(dictionary[strDataName]);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg}", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg}", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg}", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }
    }
}