using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Text;
using Inogic.Click2Undo.Workflows.Entities;
using Inogic.Click2Undo.Workflows.Helper;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;

namespace Inogic.Click2Undo.Workflows
{
    public class RestoreSelectedChanges : CodeActivity
    {
        [RequiredArgument] [Input("RecordId")] public InArgument<string> INRECORDID { get; set; }

        [RequiredArgument]
        [Input("EntityName")]
        public InArgument<string> INENTITYNAME { get; set; }

        [RequiredArgument]
        [Input("AuditDetailsJson")]
        public InArgument<string> INAUDITDETAILSJSON { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            WorkflowConfig wfConfig = null;
            var dictionary = new Dictionary<string, object>();
            var arg = "Execute: ";
            var recordId = string.Empty;
            var entityName = string.Empty;
            var traceLog = new StringBuilder();

            try
            {
                wfConfig = new WorkflowConfig(context);
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                if (INRECORDID == null || INRECORDID.Get<string>(context) == null || INENTITYNAME == null ||
                    INENTITYNAME.Get<string>(context) == null || INAUDITDETAILSJSON == null ||
                    INAUDITDETAILSJSON.Get<string>(context) == null)
                {
                    Utility.SetTrace("Input parameters are not valid.", ref traceLog, wfConfig);
                    return;
                }

                ProcessRestoreSelectedChanges(ref recordId, ref entityName, context, ref traceLog, wfConfig);
                Utility.SetTrace("Execution completed.", ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
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
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
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
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                var arg4 = ((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
                throw new InvalidPluginExecutionException(
                    $"AssemblyErrorMessage: {arg4} AssemblyTracing: {traceLog.ToString()}");
            }
        }

        private void ProcessRestoreSelectedChanges(ref string recordId, ref string entityName,
            CodeActivityContext context, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "ProcessRestoreSelectedChanges: ";
            var audtiDetailsJSON = string.Empty;

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                GetInputParameters(ref recordId, ref entityName, ref audtiDetailsJSON, context, ref traceLog, wfConfig);
                var deserializeAuditDetails = DeserializeJSON(audtiDetailsJSON, ref traceLog, wfConfig);
                if (deserializeAuditDetails == null && deserializeAuditDetails.ChangedAuditRecords == null &&
                    deserializeAuditDetails.ChangedAuditRecords.Length == 0)
                {
                    Utility.SetTrace("No chnaged audits collection found.", ref traceLog, wfConfig);
                }
                else
                {
                    GetAuditCollection(recordId, entityName, deserializeAuditDetails, ref traceLog, wfConfig);
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        private void GetInputParameters(ref string recordId, ref string entityName, ref string audtiDetailsJSON,
            CodeActivityContext context, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetInputParameters: ";

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                recordId = INRECORDID.Get<string>(context);
                Utility.SetTrace($"Record ID: {recordId}", ref traceLog, wfConfig);
                entityName = INENTITYNAME.Get<string>(context);
                Utility.SetTrace($"Entity Name: {entityName}", ref traceLog, wfConfig);
                audtiDetailsJSON = INAUDITDETAILSJSON.Get<string>(context);
                Utility.SetTrace($"Audit Details JSON: {audtiDetailsJSON}", ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        private DeserializeAuditDetails DeserializeJSON(string audtiDetailsJSON, ref StringBuilder traceLog,
            WorkflowConfig wfConfig)
        {
            var arg = "DeserializeAuditJSON: ";
            DeserializeAuditDetails deserializeAuditDetails;

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                audtiDetailsJSON = WebUtility.UrlDecode(audtiDetailsJSON);
                deserializeAuditDetails = new DeserializeAuditDetails();
                var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(audtiDetailsJSON));
                var dataContractJsonSerializer = new DataContractJsonSerializer(deserializeAuditDetails.GetType());
                deserializeAuditDetails =
                    dataContractJsonSerializer.ReadObject(memoryStream) as DeserializeAuditDetails;
                memoryStream.Close();
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return deserializeAuditDetails;
        }

        private void GetAuditCollection(string recordId, string entityName, DeserializeAuditDetails changedAudits,
            ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "RestoreChange: ";

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                Utility.SetTrace($"Audit collection count: {changedAudits.ChangedAuditRecords.ToList().Count}",
                    ref traceLog, wfConfig);
                var list = GetFormateAuditDetailsCollection(changedAudits, ref traceLog, wfConfig);
                if (list == null || list.Count <= 0)
                {
                    Utility.SetTrace("Formatted audit details by id not found.", ref traceLog, wfConfig);
                    return;
                }

                Utility.SetTrace($"formattedAuditCollection count: {list.Count}", ref traceLog, wfConfig);
                RestoreChange(recordId, entityName, list, changedAudits, ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        private List<IGrouping<string, ChangedAuditRecords>> GetFormateAuditDetailsCollection(
            DeserializeAuditDetails changedAudits, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetFormateAuditDetailsCollection: ";
            List<ChangedAuditRecords> list = null;
            List<IGrouping<string, ChangedAuditRecords>> result = null;

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                var list2 = changedAudits.ChangedAuditRecords
                    .OrderByDescending((ChangedAuditRecords attrCol) => attrCol.ChangedDate).Distinct().ToList();
                if (list2 == null || list2.Count <= 0)
                {
                    Utility.SetTrace("Sorted audit details not found.", ref traceLog, wfConfig);
                    return result;
                }

                Utility.SetTrace($"sortedAuditDetails count: {list2.Count}", ref traceLog, wfConfig);
                list = (from p in list2
                    group p by p.ChangedFieldName
                    into p
                    select p.FirstOrDefault()).ToList();
                if (list == null || list.Count <= 0)
                {
                    Utility.SetTrace("Grouped audit details by field name not found.", ref traceLog, wfConfig);
                    return result;
                }

                return (from p in list
                    group p by p.AuditDetailId
                    into p
                    select (p)).ToList();
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        private void RestoreChange(string recordId, string entityName,
            List<IGrouping<string, ChangedAuditRecords>> formattedAuditCollection,
            DeserializeAuditDetails changedAudits, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "RestoreChange: ";

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                var entity = new Entity(entityName, new Guid(recordId));
                for (var i = 0; i < formattedAuditCollection.Count; i++)
                {
                    if (formattedAuditCollection[i] == null || formattedAuditCollection[i].ToList() == null ||
                        formattedAuditCollection[i].ToList().Count <= 0)
                    {
                        Utility.SetTrace("Audit collection not found.", ref traceLog, wfConfig);
                        continue;
                    }

                    var list = formattedAuditCollection[i].ToList();
                    if (list.Count <= 0 || list[0] == null || list[0].AuditDetailId == null)
                    {
                        Utility.SetTrace($"Audit id for collection at {i} not found.", ref traceLog, wfConfig);
                        continue;
                    }

                    var attributeAuditDetail = ValidateAndGetAuditAttributes(list, ref traceLog, wfConfig);
                    AddSelectedAttributesToUpdate(list, ref entity, attributeAuditDetail, ref traceLog, wfConfig);
                }

                if (entity == null && entity.Attributes.Count <= 0)
                {
                    Utility.SetTrace("No data to update.", ref traceLog, wfConfig);
                    return;
                }

                Utility.SetTrace($"updateEntity.Attributes.Count: {entity.Attributes.Count}", ref traceLog, wfConfig);
                wfConfig.Service.Update(entity);
                Utility.SetTrace("Restored successfully.", ref traceLog, wfConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }

        private AttributeAuditDetail ValidateAndGetAuditAttributes(List<ChangedAuditRecords> currentCollection,
            ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "ValidateAndGetAuditAttributes: ";
            AttributeAuditDetail attributeAuditDetail;

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                var auditDetail = Utility.GetAuditDetails(new Guid(currentCollection[0].AuditDetailId), ref traceLog,
                    wfConfig);
                if (auditDetail == null || auditDetail.AuditRecord == null ||
                    auditDetail.GetType() != typeof(AttributeAuditDetail) ||
                    auditDetail.AuditRecord.Attributes == null || auditDetail.AuditRecord.Attributes.Count <= 0)
                {
                    Utility.SetTrace($"No audit details found for audit id: {currentCollection[0].AuditDetailId}",
                        ref traceLog, wfConfig);
                    return null;
                }

                attributeAuditDetail = auditDetail as AttributeAuditDetail;
                if (attributeAuditDetail == null || attributeAuditDetail.AuditRecord == null ||
                    attributeAuditDetail.AuditRecord.Attributes == null ||
                    attributeAuditDetail.AuditRecord.Attributes.Count <= 0)
                {
                    Utility.SetTrace($"No attribute details found for audit id: {currentCollection[0].AuditDetailId}",
                        ref traceLog, wfConfig);
                    return null;
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }

            return attributeAuditDetail;
        }

        private void AddSelectedAttributesToUpdate(List<ChangedAuditRecords> currentCollection, ref Entity updateEntity,
            AttributeAuditDetail attributeAuditDetail, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "AddSelectedAttributesToUpdate: ";
            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                for (var i = 0; i < currentCollection.Count; i++)
                {
                    Utility.SetTrace($"For attribute: {currentCollection[i].ChangedFieldName}", ref traceLog, wfConfig);
                    if (string.IsNullOrEmpty(currentCollection[i].ChangedFieldName))
                    {
                        Utility.SetTrace("Changed field name not found.", ref traceLog, wfConfig);
                    }

                    if (updateEntity != null && !updateEntity.Contains(currentCollection[i].ChangedFieldName) &&
                        attributeAuditDetail.OldValue.Contains(currentCollection[i].ChangedFieldName))
                    {
                        updateEntity[currentCollection[i].ChangedFieldName] =
                            attributeAuditDetail.OldValue[currentCollection[i].ChangedFieldName];
                    }
                    else if (!attributeAuditDetail.OldValue.Contains(currentCollection[i].ChangedFieldName))
                    {
                        updateEntity[currentCollection[i].ChangedFieldName] = null;
                    }

                    if (updateEntity.Attributes.Contains(currentCollection[i].ChangedFieldName))
                    {
                        Utility.SetTrace(
                            $"Attribute new value: {updateEntity.Attributes[currentCollection[i].ChangedFieldName]}",
                            ref traceLog, wfConfig);
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex == null)
                    ? "Exception not found"
                    : ((ex.InnerException != null && ex.InnerException.Message != null)
                        ? ex.InnerException.Message
                        : ex.Message));
            }
            catch (InvalidPluginExecutionException ex2)
            {
                Utility.SetTrace($"Exited from: {arg} InvalidPluginExecutionException: {ex2.Message}.", ref traceLog,
                    wfConfig);
                throw new InvalidPluginExecutionException((ex2 == null)
                    ? "Exception not found"
                    : ((ex2.InnerException != null && ex2.InnerException.Message != null)
                        ? ex2.InnerException.Message
                        : ex2.Message));
            }
            catch (Exception ex3)
            {
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, wfConfig);
                throw new InvalidPluginExecutionException((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
            }
        }
    }
}