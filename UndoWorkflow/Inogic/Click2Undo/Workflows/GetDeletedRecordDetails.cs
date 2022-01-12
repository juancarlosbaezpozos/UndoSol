using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Text;
using Inogic.Click2Undo.Workflows.Entities;
using Inogic.Click2Undo.Workflows.Helper;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Workflow;

namespace Inogic.Click2Undo.Workflows
{
    public class GetDeletedRecordDetails : CodeActivity
    {
        [RequiredArgument]
        [Input("EntityTypeCode")]
        public InArgument<string> INENTITYTYPECODE { get; set; }

        [RequiredArgument]
        [Input("EntityName")]
        public InArgument<string> INENTITYNAME { get; set; }

        [RequiredArgument]
        [Input("StartDate")]
        public InArgument<string> INSTARTDATE { get; set; }

        [RequiredArgument] [Input("EndDate")] public InArgument<string> INENDDATE { get; set; }

        [RequiredArgument]
        [Input("PagingCookie")]
        public InArgument<string> PagingCookie { get; set; }

        [Input("DescendingOrder")] public InArgument<bool> DescendingOrder { get; set; }

        [Output("DeletedAuditsJson")] public OutArgument<string> OUTDELETEDAUDITJSON { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            WorkflowConfig wfConfig = null;
            var dictionary = new Dictionary<string, object>();
            var arg = "Execute: ";
            var entityName = string.Empty;
            var pagingCookie = string.Empty;
            var traceLog = new StringBuilder();

            try
            {
                wfConfig = new WorkflowConfig(context);
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                if (INENTITYTYPECODE == null || INENTITYTYPECODE.Get<string>(context) == null || INSTARTDATE == null ||
                    INSTARTDATE.Get<string>(context) == null || INENDDATE == null ||
                    INENDDATE.Get<string>(context) == null)
                {
                    Utility.SetTrace("Input parameters are not valid.", ref traceLog, wfConfig);
                    return;
                }

                ProcessGetDeletedRecordsFromAudit(ref entityName, ref pagingCookie, context, ref traceLog, wfConfig);
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

        private void ProcessGetDeletedRecordsFromAudit(ref string entityName, ref string pagingCookie,
            CodeActivityContext context, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "ProcessGetDeletedRecordsFromAudit: ";
            var empty = string.Empty;
            var entityTypeCode = string.Empty;
            var startDate = string.Empty;
            var endDate = string.Empty;
            var descendingOrder = true;
            var totalRecordsCount = string.Empty;
            var recordsPerPage = 0;

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                GetInputParameters(ref entityName, ref entityTypeCode, ref pagingCookie, ref descendingOrder,
                    ref startDate, ref endDate, context, ref traceLog, wfConfig);
                var entityCollection = GetFormattedAuditCollection(entityTypeCode, ref pagingCookie, descendingOrder,
                    startDate, endDate, ref totalRecordsCount, ref recordsPerPage, ref traceLog, wfConfig);
                if (entityCollection == null)
                {
                    Utility.SetTrace("Audit details not found.", ref traceLog, wfConfig);
                    return;
                }

                var array = GetDeletedRecordsCollection(entityName, entityCollection, ref traceLog, wfConfig);
                if (array == null)
                {
                    Utility.SetTrace("No deleted records collection found.", ref traceLog, wfConfig);
                    return;
                }

                var serializeDeletedRecords = new SerializeDeletedRecords();
                serializeDeletedRecords.DeletedRecordsCollection = array;
                serializeDeletedRecords.TotalDeletedRecordscount = totalRecordsCount;
                serializeDeletedRecords.PagingCookie = pagingCookie;
                serializeDeletedRecords.RecordsPerPage = recordsPerPage.ToString();
                empty = GenerateXml(serializeDeletedRecords, typeof(SerializeDeletedRecords), ref traceLog, wfConfig);
                if (string.IsNullOrEmpty(empty))
                {
                    Utility.SetTrace("No JSON string found.", ref traceLog, wfConfig);
                }

                Utility.SetTrace($"JSON length: {empty.Length}", ref traceLog, wfConfig);
                OUTDELETEDAUDITJSON.Set(context, empty);
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

        private EntityCollection GetFormattedAuditCollection(string entityTypeCode, ref string pagingCookie,
            bool descendingOrder, string startDate, string endDate, ref string totalRecordsCount,
            ref int recordsPerPage, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetFormattedAuditCollection: ";
            EntityCollection entityCollection;

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                recordsPerPage = Utility.GetRecordsPerPage(ref traceLog, wfConfig);
                entityCollection = Utility.RetrieveAuditDetails(recordsPerPage, string.Empty, startDate, endDate,
                    pagingCookie, string.Empty, descendingOrder, getCount: false, getFields: false, "3", entityTypeCode,
                    "3", isUndoActionTriggered: false, ref traceLog, wfConfig);
                if (entityCollection == null || entityCollection.Entities.Count <= 0)
                {
                    Utility.SetTrace("Audit entity collection not found.", ref traceLog, wfConfig);
                    return null;
                }

                Utility.SetTrace(
                    $"Total deleted records count for {entityTypeCode} entity : {entityCollection.Entities.Count}",
                    ref traceLog, wfConfig);
                if (entityCollection == null || entityCollection.Entities.Count <= 0)
                {
                    Utility.SetTrace("Formatted audit collection not found.", ref traceLog, wfConfig);
                    return null;
                }

                var entityCollection2 = Utility.RetrieveAuditDetails(recordsPerPage, string.Empty, startDate, endDate,
                    string.Empty, string.Empty, descendingOrder: false, getCount: true, getFields: false, "3",
                    entityTypeCode, "3", isUndoActionTriggered: false, ref traceLog, wfConfig);
                if (entityCollection2 != null && entityCollection2.Entities.Count > 0)
                {
                    _ = entityCollection2.TotalRecordCount;
                    totalRecordsCount = entityCollection2.TotalRecordCount.ToString();
                }

                if (entityCollection != null && entityCollection.Entities.Count > 0)
                {
                    pagingCookie = entityCollection.PagingCookie;
                }

                Utility.SetTrace($"Deleted records count: {entityCollection.Entities.Count}", ref traceLog, wfConfig);
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

            return entityCollection;
        }

        private void GetInputParameters(ref string entityName, ref string entityTypeCode, ref string pagingCookie,
            ref bool descendingOrder, ref string startDate, ref string endDate, CodeActivityContext context,
            ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetInputParameters: ";

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                entityName = ((INENTITYNAME != null && INENTITYNAME.Get<string>(context) != null)
                    ? INENTITYNAME.Get<string>(context)
                    : string.Empty);
                Utility.SetTrace($"Entity name: {entityName}", ref traceLog, wfConfig);
                entityTypeCode = ((INENTITYTYPECODE != null && INENTITYTYPECODE.Get<string>(context) != null)
                    ? INENTITYTYPECODE.Get<string>(context)
                    : string.Empty);
                Utility.SetTrace($"Entity type code: {entityTypeCode}", ref traceLog, wfConfig);
                startDate = ((INSTARTDATE != null && INSTARTDATE.Get<string>(context) != null)
                    ? INSTARTDATE.Get<string>(context)
                    : string.Empty);
                Utility.SetTrace($"Start date: {startDate}", ref traceLog, wfConfig);
                endDate = ((INENDDATE != null && INENDDATE.Get<string>(context) != null)
                    ? INENDDATE.Get<string>(context)
                    : string.Empty);
                Utility.SetTrace($"End date: {endDate}", ref traceLog, wfConfig);
                pagingCookie = PagingCookie.Get<string>(context);
                Utility.SetTrace($"pagingCookie: {pagingCookie}", ref traceLog, wfConfig);
                descendingOrder = ((DescendingOrder != null) ? DescendingOrder.Get<bool>(context) : descendingOrder);
                Utility.SetTrace($"descendingOrder: {descendingOrder.ToString()}", ref traceLog, wfConfig);
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

        private DeletedRecordsCollection[] GetDeletedRecordsCollection(string entityName,
            EntityCollection formattedAuditCollection, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetDeletedRecordsCollections: ";
            DeletedRecordsCollection[] array;

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                var entityMetadata = Utility.RetrieveEntityMetadata(entityName, ref traceLog, wfConfig);
                array = new DeletedRecordsCollection[formattedAuditCollection.Entities.Count];
                for (var i = 0; i < formattedAuditCollection.Entities.Count; i++)
                {
                    var deletedRecordsCollection = new DeletedRecordsCollection();
                    if (formattedAuditCollection[i] == null || formattedAuditCollection[i].Id == Guid.Empty)
                    {
                        Utility.SetTrace("Audit id not found.", ref traceLog, wfConfig);
                        continue;
                    }

                    var auditDetail = Utility.GetAuditDetails(formattedAuditCollection[i].Id, ref traceLog, wfConfig);
                    if (auditDetail == null || auditDetail.AuditRecord == null ||
                        auditDetail.GetType() != typeof(AttributeAuditDetail))
                    {
                        Utility.SetTrace("Audit details not found.", ref traceLog, wfConfig);
                        continue;
                    }

                    Utility.SetTrace("Audit details found.", ref traceLog, wfConfig);
                    var attributeAuditDetail = auditDetail as AttributeAuditDetail;
                    deletedRecordsCollection.DeletedRecordAttributes = GetDeletedRecordAttributes(entityName,
                        formattedAuditCollection.Entities[i].Id, entityMetadata, auditDetail, attributeAuditDetail,
                        ref traceLog, wfConfig);
                    if (deletedRecordsCollection == null || deletedRecordsCollection.DeletedRecordAttributes == null)
                    {
                        Utility.SetTrace("No attribute collection found.", ref traceLog, wfConfig);
                    }
                    else
                    {
                        array[i] = deletedRecordsCollection;
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

            return array;
        }

        private DeletedRecordAttributes GetDeletedRecordAttributes(string entityName, Guid deletedRecordId,
            EntityMetadata entityMetadata, AuditDetail auditDetail, AttributeAuditDetail attributeAuditDetail,
            ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetDeletedRecordAttributes: ";
            DeletedRecordAttributes deletedRecordAttributes;

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                deletedRecordAttributes = new DeletedRecordAttributes();
                deletedRecordAttributes.AuditDetailId = deletedRecordId.ToString();
                deletedRecordAttributes.DeletedBy =
                    ((auditDetail.AuditRecord.Contains("userid") &&
                      auditDetail.AuditRecord.GetAttributeValue<EntityReference>("userid") != null)
                        ? auditDetail.AuditRecord.GetAttributeValue<EntityReference>("userid").Name
                        : string.Empty);
                deletedRecordAttributes.DeletedOn =
                    ((auditDetail.AuditRecord.Attributes.Contains("createdon") &&
                      auditDetail.AuditRecord.Attributes["createdon"] != null)
                        ? Utility.GetConvertedDate(Convert.ToDateTime(auditDetail.AuditRecord.Attributes["createdon"]),
                            ref traceLog, wfConfig).ToString()
                        : string.Empty);
                deletedRecordAttributes.FormattedDeletedOn =
                    ((auditDetail.AuditRecord.FormattedValues.Contains("createdon") &&
                      auditDetail.AuditRecord.FormattedValues["createdon"] != null)
                        ? auditDetail.AuditRecord.FormattedValues["createdon"]
                        : string.Empty);
                deletedRecordAttributes.RecordName = GetPrimaryAttributeValue(entityName, entityMetadata,
                    attributeAuditDetail, ref traceLog, wfConfig);
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

            return deletedRecordAttributes;
        }

        private string GetPrimaryAttributeValue(string entityName, EntityMetadata entityMetadata,
            AttributeAuditDetail attributeAuditDetail, ref StringBuilder traceLog, WorkflowConfig wfConfig)
        {
            var arg = "GetPrimaryAttributeValue: ";
            string empty;

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                if (entityMetadata == null || string.IsNullOrEmpty(entityMetadata.PrimaryNameAttribute))
                {
                    Utility.SetTrace("No entity metadata or PrimaryNameAttribute found.", ref traceLog, wfConfig);
                    return "";
                }

                Utility.SetTrace($"Primary attribute name: {entityMetadata.PrimaryNameAttribute}", ref traceLog,
                    wfConfig);
                empty = ((attributeAuditDetail.OldValue != null &&
                          attributeAuditDetail.OldValue.Attributes.Contains(entityMetadata.PrimaryNameAttribute))
                    ? attributeAuditDetail.OldValue.Attributes[entityMetadata.PrimaryNameAttribute].ToString()
                    : string.Empty);
                Utility.SetTrace($"Primary attribute value: {empty}", ref traceLog, wfConfig);
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

            return empty;
        }

        private string GenerateXml(SerializeDeletedRecords entity, Type objecttype, ref StringBuilder traceLog,
            WorkflowConfig wfConfig)
        {
            var arg = "GenerateXml: ";
            var result = string.Empty;

            try
            {
                Utility.SetTrace($"Inside {arg} ", ref traceLog, wfConfig);
                var assemblyQualifiedName = objecttype.AssemblyQualifiedName;
                var dataContractJsonSerializer = new DataContractJsonSerializer(objecttype);
                using var memoryStream = new MemoryStream();
                dataContractJsonSerializer.WriteObject(memoryStream, entity);
                memoryStream.Position = 0L;
                var streamReader = new StreamReader(memoryStream);
                result = streamReader.ReadToEnd();
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