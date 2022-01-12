using System;
using System.Activities;
using System.Collections.Generic;
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
    public class RestoreDeletedRecords : CodeActivity
    {
        [RequiredArgument]
        [Input("EntityName")]
        public InArgument<string> INENTITYNAME { get; set; }

        [RequiredArgument]
        [Input("AuditDetailId")]
        public InArgument<string> INAUDITDETAILID { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            WorkflowConfig workflowConfig = null;
            var dictionary = new Dictionary<string, object>();
            var arg = "Execute: ";
            var traceLog = new StringBuilder();
            var entityName = string.Empty;
            var audtiDetailId = string.Empty;

            try
            {
                workflowConfig = new WorkflowConfig(context);
                var workflowConfig2 = new WorkflowConfig(context);
                Utility.SetTrace($"Inside {arg} ", ref traceLog, workflowConfig);
                Utility.SetTrace($"InitiatingUserId: {workflowConfig.WorkflowContext.InitiatingUserId} ", ref traceLog,
                    workflowConfig);
                Utility.SetTrace($"UserId: {workflowConfig.WorkflowContext.UserId} ", ref traceLog, workflowConfig);
                if (INENTITYNAME == null || INENTITYNAME.Get<string>(context) == null || INAUDITDETAILID == null ||
                    INAUDITDETAILID.Get<string>(context) == null)
                {
                    Utility.SetTrace("Input parameters are not valid.", ref traceLog, workflowConfig);
                    return;
                }

                GetInputParameters(ref entityName, ref audtiDetailId, context, ref traceLog, workflowConfig);
                RestoreRecord(entityName, audtiDetailId, ref traceLog, workflowConfig);
                Utility.SetTrace("Execution completed.", ref traceLog, workflowConfig);
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                Utility.SetTrace($"Exited from: {arg} FaultException: {ex.Message}.", ref traceLog, workflowConfig);
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
                Utility.SetTrace($"Exited from: {arg} Exception: {ex3.Message}.", ref traceLog, workflowConfig);
                var arg4 = ((ex3 == null)
                    ? "Exception not found"
                    : ((ex3.InnerException != null && ex3.InnerException.Message != null)
                        ? ex3.InnerException.Message
                        : ex3.Message));
                throw new InvalidPluginExecutionException(
                    $"AssemblyErrorMessage: {arg4} AssemblyTracing: {traceLog.ToString()}");
            }
        }


        private void GetInputParameters(ref string entityName, ref string auditDetailId, CodeActivityContext context,
            ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetInputParameters: ";

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                entityName = INENTITYNAME.Get<string>(context);
                Utility.SetTrace($"Entity Name: {entityName}", ref traceLog, wfConfig);
                auditDetailId = INAUDITDETAILID.Get<string>(context);
                Utility.SetTrace($"Audit Detail Id: {auditDetailId}", ref traceLog, wfConfig);
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

        private void RestoreRecord(string entityName, string audtiDetailId, ref StringBuilder traceLog,
            WorkflowConfig wfConfig)
        {
            var arg = "RestoreRecord: ";

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                var entity = GetDeletedRecord(new Guid(audtiDetailId), ref traceLog, wfConfig);
                if (entity == null)
                {
                    Utility.SetTrace("No deleted record found.", ref traceLog, wfConfig);
                    return;
                }

                if (entity.Attributes == null)
                {
                    Utility.SetTrace("No deleted record attributes found..", ref traceLog, wfConfig);
                    return;
                }

                if (entity.Id == Guid.Empty)
                {
                    Utility.SetTrace("No deleted record Id found.", ref traceLog, wfConfig);
                    return;
                }

                if (entity.Attributes.Count <= 0)
                {
                    Utility.SetTrace("Deleted record attributes are 0.", ref traceLog, wfConfig);
                    return;
                }

                CheckIfRecordAlreadyExist(entityName, entity.Id, ref traceLog, wfConfig);
                Utility.SetTrace($"Deleted record attributes count: {entity.Attributes.Count}", ref traceLog, wfConfig);
                entity.Attributes.Remove("statecode");
                entity.Attributes.Remove("statuscode");
                Utility.SetTrace(
                    $"Deleted record attributes count after removing status and status reason: {entity.Attributes.Count}",
                    ref traceLog, wfConfig);
                wfConfig.Service.Create(entity);
                Utility.SetTrace("Record restore successfully.", ref traceLog, wfConfig);
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

        private Entity GetDeletedRecord(Guid audtiDetailId, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetDeletedRecord: ";
            Entity entity;

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                var auditDetail = Utility.GetAuditDetails(audtiDetailId, ref traceLog, wfConfig);
                if (auditDetail == null || auditDetail.AuditRecord == null ||
                    auditDetail.GetType() != typeof(AttributeAuditDetail))
                {
                    Utility.SetTrace("Audit details not found.", ref traceLog, wfConfig);
                    return null;
                }

                Utility.SetTrace("Audit details found.", ref traceLog, wfConfig);
                var attributeAuditDetail = auditDetail as AttributeAuditDetail;
                if (attributeAuditDetail == null || attributeAuditDetail.OldValue == null)
                {
                    Utility.SetTrace("No audit attributes found.", ref traceLog, wfConfig);
                    return null;
                }

                Utility.SetTrace("Audit attribute details found.", ref traceLog, wfConfig);
                entity = attributeAuditDetail.OldValue;
                if (entity.Id == Guid.Empty && attributeAuditDetail.AuditRecord != null &&
                    attributeAuditDetail.AuditRecord.Attributes.Contains("objectid") &&
                    attributeAuditDetail.AuditRecord.Attributes["objectid"] != null)
                {
                    entity.Id = attributeAuditDetail.AuditRecord.GetAttributeValue<EntityReference>("objectid").Id;
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

            return entity;
        }

        private void CheckIfRecordAlreadyExist(string entityName, Guid recordId, ref StringBuilder traceLog,
            WorkflowConfig wfConfig)
        {
            var arg = "CheckIfRecordAlreadyExist: ";
            Entity entity = null;
            var dictionary = new Dictionary<string, string>();

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                var entityMetadata = Utility.RetrieveEntityMetadata(entityName, ref traceLog, wfConfig);
                if (entityMetadata != null && !string.IsNullOrEmpty(entityMetadata.PrimaryIdAttribute))
                {
                    Utility.SetTrace($"PrimaryIdAttribute: {entityMetadata.PrimaryIdAttribute} ", ref traceLog,
                        wfConfig);
                    entity = RetrieveEntityRecord(entityName, recordId, entityMetadata.PrimaryIdAttribute, ref traceLog,
                        wfConfig);
                }
                else
                {
                    Utility.SetTrace("Metadata not found or PrimaryIdAttribute in metadata not found.", ref traceLog,
                        wfConfig);
                }

                if (entity != null)
                {
                    Utility.SetTrace($"Record with id: {recordId} already exist.", ref traceLog, wfConfig);
                    var list = new List<string> { "restoredelete" };
                    dictionary = Utility.RetrieveLanguageLabelCollection(list, ref traceLog, wfConfig);
                    if (dictionary.Count > 0 && dictionary.ContainsKey("recordisalreadyexist"))
                    {
                        throw new InvalidPluginExecutionException(dictionary["recordisalreadyexist"]);
                    }
                }

                Utility.SetTrace("Record not exist in crm.", ref traceLog, wfConfig);
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

        private Entity RetrieveEntityRecord(string entityName, Guid recordId, string primaryIdAttribute,
            ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "RetrieveEntityRecord: ";
            Entity result = null;

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                var empty =
                    $"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>\r\n                              <entity name='{entityName}'> \r\n                                <attribute name='{primaryIdAttribute}' />    \r\n                                <filter type='and'>\r\n                                  <condition attribute='{primaryIdAttribute}' operator='eq' value='{recordId}' />\r\n                                </filter>\r\n                              </entity>\r\n                            </fetch>";
                Utility.SetTrace("Before retrieve entity record", ref traceLog, wfConfig);
                var entityCollection = wfConfig.Service.RetrieveMultiple(new FetchExpression(empty));
                if (entityCollection != null && entityCollection.Entities.Count > 0)
                {
                    Utility.SetTrace("Deleted record entity collection found.", ref traceLog, wfConfig);
                    result = entityCollection.Entities[0];
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

            return result;
        }
    }
}