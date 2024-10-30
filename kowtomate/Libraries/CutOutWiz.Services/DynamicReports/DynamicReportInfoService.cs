using CutOutWiz.Core;
using CutOutWiz.Services.Models.DynamicReports;
using CutOutWiz.Services.DbAccess;
using static CutOutWiz.Core.Utilities.Enums;
using CutOutWiz.Data;

namespace CutOutWiz.Services.DynamicReports
{
    public class DynamicReportInfoService : IDynamicReportInfoService
    {

        private readonly ISqlDataAccess _db;

        public DynamicReportInfoService(ISqlDataAccess db)
        {
            _db = db;
        }

        #region DynamicReportInfo
        /// <summary>
        /// Get All Dynamic Report Info
        /// </summary>
        /// <returns></returns>
        public async Task<List<DynamicReportInfoModel>> GetAll()
        {
            return await _db.LoadDataUsingProcedure<DynamicReportInfoModel, dynamic>(storedProcedure: "dbo.SP_Report_DynamicReportInfo_GetAll", new { });
        }

        /// <summary>
        /// Get All Dynamic Reports by RoleModel Object Id
        /// </summary>
        /// <returns></returns>
        public async Task<List<DynamicReportInfoModel>> GetOnlyAssignedReportsByRoleObjectId(string userObjectId)
        {
            string query = @"SELECT * FROM [dbo].[Report_DynamicReportInfo] dr WITH(NOLOCK)
            WHERE dr.[Status] = 1 AND (dr.PermissionObjectId IS NULL OR dr.PermissionObjectId IN (SELECT [PermissionObjectId] from [dbo].[Security_RolePermission] rp  WITH(NOLOCK)
            INNER JOIN [dbo].[Security_UserRole] ur WITH(NOLOCK) on ur.RoleObjectId = rp.RoleObjectId
            WHERE ur.UserObjectId = @UserObjectId )) ORDER BY dr.Name
            ";
            return await _db.LoadDataUsingQuery<DynamicReportInfoModel, dynamic>(query, new { UserObjectId = userObjectId });
        }

        /// <summary>
        /// Get All Dynamic Report Info For Dropdownlist
        /// </summary>
        /// <returns></returns>
        public async Task<List<DynamicReportInfoSlim>> GetAllDynamicReportInfoForDropdownlist()
        {
            string query = @"SELECT Id,[Name] FROM [dbo].[Report_DynamicReportInfo] dr WITH(NOLOCK) ORDER BY dr.Name;";
            return await _db.LoadDataUsingQuery<DynamicReportInfoSlim, dynamic>(query, new {  });
        }


        /// <summary>
        /// Get Dynami cReport Info by dynamicReportInfo Id
        /// </summary>
        /// <param name="DynamicReportInfoId"></param>
        /// <returns></returns>
        public async Task<DynamicReportInfoModel> GetById(int dynamicReportInfoId)
        {
            var result = await _db.LoadDataUsingProcedure<DynamicReportInfoModel, dynamic>(storedProcedure: "dbo.SP_Report_DynamicReportInfo_GetById", new { Id = dynamicReportInfoId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get Dynami cReport Info by dynamicReportInfo Name
        /// </summary>
        /// <param name="DynamicReportInfoId"></param>
        /// <returns></returns>
        public async Task<DynamicReportInfoModel> GetReportInfoByName(string reportName)
        {
            var query = "SELECT * FROM [dbo].[Report_DynamicReportInfo] dr WITH(NOLOCK) WHERE Name = @Name";
            var result = await _db.LoadDataUsingQuery<DynamicReportInfoModel, dynamic>(query, new { Name = reportName });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get by Object Id
        /// </summary>
        /// <param name="DynamicReportInfoId"></param>
        /// <returns></returns>
        public async Task<DynamicReportInfoModel> GetByObjectId(string objectId)
        {
            var result = await _db.LoadDataUsingProcedure<DynamicReportInfoModel, dynamic>(storedProcedure: "dbo.SP_Report_DynamicReportInfo_GetByObjectId", new { ObjectId = objectId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Insert Dynamic Report Info
        /// </summary>
        /// <param name="dynamicReportInfo"></param>
        /// <returns></returns>
        public async Task<Response<int>> Insert(DynamicReportInfoModel dynamicReportInfo)
        {
            var response = new Core.Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Report_DynamicReportInfo_Insert", new
                {
                    dynamicReportInfo.Name,
                    dynamicReportInfo.Description,
                    dynamicReportInfo.SqlType,
                    dynamicReportInfo.SqlScript,
                    dynamicReportInfo.AllowCompanyFilter,
                    dynamicReportInfo.AllowStartDateFilter,
                    dynamicReportInfo.AllowEndDateFilter,
                    dynamicReportInfo.AllowDateOnlyFilter,
                    dynamicReportInfo.AllowFiltering,
                    dynamicReportInfo.AllowPaging,
                    dynamicReportInfo.AllowSorting,
                    dynamicReportInfo.AllowHtmlPreview,
                    dynamicReportInfo.DefaultSortColumn,
                    dynamicReportInfo.DefaultSortOrder,
                    dynamicReportInfo.AllowVirtualization,
                    dynamicReportInfo.PageSize,
                    dynamicReportInfo.PermissionObjectId,
                    dynamicReportInfo.Status,
                    dynamicReportInfo.CreatedByContactId,
                    dynamicReportInfo.ObjectId,
                    dynamicReportInfo.ReportCode,
                    dynamicReportInfo.ReportType,
                    dynamicReportInfo.WhereClause
                });

                dynamicReportInfo.Id = newId;
                response.Result = newId;
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        /// <summary>
        /// Update Dynamic Report Info
        /// </summary>
        /// <param name="dynamicReportInfo"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Update(DynamicReportInfoModel dynamicReportInfo)
        {
            var response = new Core.Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Report_DynamicReportInfo_Update", new
                {
                    dynamicReportInfo.Id,
                    dynamicReportInfo.Name,
                    dynamicReportInfo.Description,
                    dynamicReportInfo.SqlType,
                    dynamicReportInfo.SqlScript,
                    dynamicReportInfo.AllowCompanyFilter,
                    dynamicReportInfo.AllowStartDateFilter,
                    dynamicReportInfo.AllowEndDateFilter,
                    dynamicReportInfo.AllowDateOnlyFilter,
                    dynamicReportInfo.AllowFiltering,
                    dynamicReportInfo.AllowPaging,
                    dynamicReportInfo.AllowSorting,
                    dynamicReportInfo.AllowHtmlPreview,
                    dynamicReportInfo.DefaultSortColumn,
                    dynamicReportInfo.DefaultSortOrder,
                    dynamicReportInfo.AllowVirtualization,
                    dynamicReportInfo.PageSize,
                    dynamicReportInfo.PermissionObjectId,
                    dynamicReportInfo.Status,
                    dynamicReportInfo.UpdatedByContactId,
                    dynamicReportInfo.ReportCode,
                    dynamicReportInfo.ReportType,
                    dynamicReportInfo.WhereClause

                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        /// <summary>
        /// Delete Dynamic Report Info by id
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<Response<bool>> Delete(string objectId)
        {
            var response = new Core.Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Report_DynamicReportInfo_Delete", new { ObjectId = objectId });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<Response<bool>> CloneDynamicReportInfo(DynamicReportInfoModel model)
        {
            var response = new Response<bool>();

            try
            {
                string query = @$"
                    DECLARE @LastInsertedID INT;

                    INSERT INTO [dbo].[Report_DynamicReportInfo]
                    ([Name], [Description], [SqlType], [SqlScript], [Status], [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], 
                    [ObjectId], [AllowCompanyFilter], [AllowStartDateFilter], [AllowEndDateFilter], [AllowDateOnlyFilter], [AllowFiltering], [AllowPaging], 
                    [AllowSorting], [AllowHtmlPreview], [DefaultSortColumn], [DefaultSortOrder], [AllowVirtualization], [PageSize], [PermissionObjectId], 
                    [AllowDetailReport], [SqlTypeForDetailReport], [SqlScriptForDetailReport], [FilterByForDetailReport], [ReportCode],[ReportType],WhereClause )
                    SELECT 
                    [Name]+'-Copy', [Description], [SqlType], [SqlScript],2, [CreatedDate], [CreatedByContactId], [UpdatedDate], [UpdatedByContactId], 
                    [ObjectId], [AllowCompanyFilter], [AllowStartDateFilter], [AllowEndDateFilter], [AllowDateOnlyFilter], [AllowFiltering], [AllowPaging], 
                    [AllowSorting], [AllowHtmlPreview], [DefaultSortColumn], [DefaultSortOrder], [AllowVirtualization], [PageSize], [PermissionObjectId], 
                    [AllowDetailReport], [SqlTypeForDetailReport], [SqlScriptForDetailReport], [FilterByForDetailReport], [ReportCode]+'-Copy',ReportType,WhereClause
                    FROM [Report_DynamicReportInfo] WHERE Id={model.Id};

                    SET @LastInsertedID= SCOPE_IDENTITY();

                    INSERT INTO [dbo].[Report_TableColumn]
                    (
                    [DynamicReportInfoId], [DisplayName], [FieldName], [FieldWithPrefix], [IsVisible], [Filterable], [Sortable], [TextAlign], 
                    [DisplayOrder], [Width], [TextColor], [FieldType], [CreatedDate], [CreatedByContactId], [BackgroundColor], [ShowFooterTotal], 
                    [FooterTotalLabel], [ShowFooterAverage], [FooterAverageLabel], [ApplyInFilter1], [ApplyInFilter2], [ApplyInFilter3], [BackgroundColorRules],
                    [DispalyFormat], [Groupable], [IsDefaultGroup], [ShowGroupTotal], [JoinInfoId], [SortingType], [JoinInfo2Id], [JoinInfo3Id]
                    )
                    SELECT 
                    @LastInsertedID, [DisplayName], [FieldName], [FieldWithPrefix], [IsVisible], [Filterable], [Sortable], [TextAlign], 
                    [DisplayOrder], [Width], [TextColor], [FieldType], [CreatedDate], [CreatedByContactId], [BackgroundColor], [ShowFooterTotal], 
                    [FooterTotalLabel], [ShowFooterAverage], [FooterAverageLabel], [ApplyInFilter1], [ApplyInFilter2], [ApplyInFilter3], [BackgroundColorRules],
                    [DispalyFormat], [Groupable], [IsDefaultGroup], [ShowGroupTotal], [JoinInfoId], [SortingType], [JoinInfo2Id], [JoinInfo3Id]
                    FROM [dbo].[Report_TableColumn]
                    WHERE [DynamicReportInfoId]={model.Id};

                    SELECT @LastInsertedID;
                ";
                var newId = await _db.SaveDataUsingQueryAndReturnId<int, dynamic>(query,
                new
                {
                    
                });

                model.Id = newId;                

                response.IsSuccess = true;
                response.Message = "Report Copy Completed Successfully!";
            }
            catch (Exception ex)
            {
                response.Message = "Error on Report Copy: " + StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        #endregion DynamicReportInfo

        #region Dynamic Table Column
        public async  Task<Response<bool>> DeleteTableColumn(int dynamicReportInfoId, int tableColumn)
        {
            var response = new Core.Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Report_TableColumn_Delete", new { DynamicReportInfoId = dynamicReportInfoId, TableColumnId = tableColumn });
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        /// <summary>
        /// Get TableColumn by  Id
        /// </summary>
        /// <param name="DynamicReportInfoId"></param>
        /// <returns></returns>
        public async Task<ReportTableColumnModel> GetTableColumnByTableColumnId(int tableColumnId)
        {
            var result = await _db.LoadDataUsingProcedure<ReportTableColumnModel, dynamic>(storedProcedure: "dbo.SP_Report_TableColumn_GetById", new { Id = tableColumnId });
            return result.FirstOrDefault();
        }

        /// <summary>
        /// Get TableColumn by  Field name
        /// </summary>
        /// <param name="DynamicReportInfoId"></param>
        /// <returns></returns>
        public async Task<ReportTableColumnModel> GetTableColumnByTableFieldName(string fieldName)
        {
            var result = await _db.LoadDataUsingProcedure<ReportTableColumnModel, dynamic>(storedProcedure: "[dbo].[SP_Report_TableColumn_GetByFieldName]", new { FieldName = fieldName });
            return result.FirstOrDefault();
        }

        public async  Task<Response<int>> InsertTableColumn(ReportTableColumnModel reportTableColumn)
        {
            var response = new Core.Response<int>();
            try
            {
                var newId = await _db.SaveDataUsingProcedureAndReturnId<short, dynamic>(storedProcedure: "dbo.SP_Report_TableColumn_Insert", new
                {
                    reportTableColumn.DynamicReportInfoId,
                    reportTableColumn.DisplayName,
                    reportTableColumn.FieldName,
                    reportTableColumn.FieldWithPrefix,
                    reportTableColumn.IsVisible,
                    reportTableColumn.Filterable,
                    reportTableColumn.Sortable,
                    reportTableColumn.TextAlign,
                    reportTableColumn.DisplayOrder,
                    reportTableColumn.Width,
                    reportTableColumn.TextColor,
                    reportTableColumn.FieldType,
                    reportTableColumn.DispalyFormat,
                    reportTableColumn.CreatedDate,
                    reportTableColumn.CreatedByContactId,
                    reportTableColumn.BackgroundColor,
                    reportTableColumn.BackgroundColorRules,
                    reportTableColumn.ShowFooterTotal,
                    reportTableColumn.FooterTotalLabel,
                    reportTableColumn.ShowFooterAverage,
                    reportTableColumn.FooterAverageLabel,
                    reportTableColumn.ApplyInFilter1,
                    reportTableColumn.ApplyInFilter2,
                    reportTableColumn.ApplyInFilter3,
                    reportTableColumn.Groupable,
                    reportTableColumn.IsDefaultGroup,
                    reportTableColumn.ShowGroupTotal,
                    reportTableColumn.JoinInfoId,
                    reportTableColumn.JoinInfo2Id,
                    reportTableColumn.JoinInfo3Id,
                    reportTableColumn.JoinInfo4Id,
                    reportTableColumn.JoinInfo5Id,
                    reportTableColumn.SortingType
                });
                
                reportTableColumn.Id = newId;
                response.Result = newId;
                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async  Task<Response<bool>> UpdateTableColumn(ReportTableColumnModel reportTableColumn)
        {
            var response = new Core.Response<bool>();

            try
            {
                await _db.SaveDataUsingProcedure(storedProcedure: "dbo.SP_Report_TableColumn_Update", new
                {
                    reportTableColumn.Id,
                    reportTableColumn.DynamicReportInfoId,
                    reportTableColumn.DisplayName,
                    reportTableColumn.FieldName,
                    reportTableColumn.FieldWithPrefix,
                    reportTableColumn.IsVisible,
                    reportTableColumn.Filterable,
                    reportTableColumn.Sortable,
                    reportTableColumn.TextAlign,
                    reportTableColumn.DisplayOrder,
                    reportTableColumn.Width,
                    reportTableColumn.TextColor,
                    reportTableColumn.FieldType,
                    reportTableColumn.DispalyFormat,
                    reportTableColumn.CreatedDate,
                    reportTableColumn.CreatedByContactId,
                    reportTableColumn.BackgroundColor,
                    reportTableColumn.BackgroundColorRules,
                    reportTableColumn.ShowFooterTotal,
                    reportTableColumn.FooterTotalLabel,
                    reportTableColumn.ShowFooterAverage,
                    reportTableColumn.FooterAverageLabel,
                    reportTableColumn.ApplyInFilter1,
                    reportTableColumn.ApplyInFilter2,
                    reportTableColumn.ApplyInFilter3,
                    reportTableColumn.Groupable,
                    reportTableColumn.IsDefaultGroup,
                    reportTableColumn.ShowGroupTotal,
                    reportTableColumn.JoinInfoId,
                    reportTableColumn.JoinInfo2Id,
                    reportTableColumn.JoinInfo3Id,
                    reportTableColumn.JoinInfo4Id,
                    reportTableColumn.JoinInfo5Id,
                    reportTableColumn.SortingType
                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        /// <summary>
        /// Get Table Column By Dynamic Report Info Id
        /// </summary>
        /// <returns></returns>
        public async Task<List<ReportTableColumnModel>> GetAllTableColumnByDynamicReportInfoId(int dynamicReportInfoId)
        {
            var query = "SELECT *FROM [dbo].[Report_TableColumn] WITH(NOLOCK) WHERE DynamicReportInfoId = @DynamicReportInfoId ORDER BY DisplayOrder ASC";

            return await _db.LoadDataUsingQuery<ReportTableColumnModel, dynamic>(query, new
            {
                DynamicReportInfoId = dynamicReportInfoId
            });
        }

        #endregion

        #region Dynamic Report Join

        public async Task<List<ReportJoinInfoModel>> GetReportJoinInfoListByDynamicReportInfoId(int dynamicReportInfoId)
        {
            try
            {
                string queryString = "";

                queryString = @"                   
                    SELECT [Id]
                      ,[DynamicReportInfoId]
                      ,[JoinName]
                      ,[JoinScript]
                      ,[DisplayOrder]
                    FROM [dbo].[Report_JoinInfo] 
                    WHERE DynamicReportInfoId=@DynamicReportInfoId
                    ORDER BY  [DisplayOrder] ASC
                    ";

                var filteredList = await _db.LoadDataUsingQuery<ReportJoinInfoModel, dynamic>(queryString,
                new
                {
                    DynamicReportInfoId = dynamicReportInfoId
                });

                return filteredList;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return new List<ReportJoinInfoModel>();
        }

        public async Task<ReportJoinInfoModel> GetReportJoinInfoById(int id)
        {
            try
            {
                string queryString = "";

                queryString = @"                   
                    SELECT [Id]
                      ,[DynamicReportInfoId]
                      ,[JoinName]
                      ,[JoinScript]
                      ,[DisplayOrder]
                    FROM [dbo].[Report_JoinInfo] 
                    WHERE Id=@Id";

                var results = await _db.LoadDataUsingQuery<ReportJoinInfoModel, dynamic>(queryString,
                    new
                    {
                        Id = id
                    });

                if (results != null && results.Any())
                {
                    return results.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return null;
        }

        public async Task<Response<int>> InsertReportJoinInfo(ReportJoinInfoModel model)
        {
            var response = new Response<int>();

            try
            {
                string insertQuery = @"
                
                INSERT INTO [dbo].[Report_JoinInfo]
                       ([DynamicReportInfoId]
                       ,[JoinName]
                       ,[JoinScript]
                       ,[DisplayOrder])
                 VALUES
                       (@DynamicReportInfoId
                       ,@JoinName
                       ,@JoinScript
                       ,@DisplayOrder);

                SELECT SCOPE_IDENTITY();
                ";

                var newId = await _db.SaveDataUsingQueryAndReturnId<int, dynamic>(insertQuery,
                new
                {
                    model.DynamicReportInfoId,
                    model.JoinName,
                    model.JoinScript,
                    model.DisplayOrder
                });

                model.Id = newId;

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<Response<bool>> UpdateReportJoinInfo(ReportJoinInfoModel model)
        {
            var response = new Response<bool>();

            try
            {
                string query = @"
                UPDATE [dbo].[Report_JoinInfo]
                   SET [DynamicReportInfoId] = @DynamicReportInfoId
                      ,[JoinName] = @JoinName
                      ,[JoinScript] = @JoinScript
                      ,[DisplayOrder] = @DisplayOrder
                 WHERE Id=@Id;
                ";

                await _db.SaveDataUsingQuery(query, new
                {
                    model.Id,
                    model.DynamicReportInfoId,
                    model.JoinName,
                    model.JoinScript,
                    model.DisplayOrder
                });

                response.IsSuccess = true;
                response.Message = "Updated Report Join";
            }
            catch (Exception ex)
            {
                response.Message = "Error on Report Join Update: " + StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        public async Task<Response<bool>> DeleteReportJoinInfo(int id)
        {
            var response = new Response<bool>();

            try
            {
                string query = @"
                    DELETE FROM [dbo].[Report_JoinInfo]
                    WHERE Id=@Id;
                ";

                await _db.SaveDataUsingQuery(query, new
                {
                    Id = id
                });

                response.IsSuccess = true;
                response.Message = StandardDataAccessMessages.SuccessMessaage;
            }
            catch (Exception ex)
            {
                response.Message = StandardDataAccessMessages.GetSqlErrorMessage(ex);
            }

            return response;
        }

        #endregion

        #region Table Columns
        /// <summary>
        /// Get Table Column By Dynamic Report Info Id
        /// </summary>
        /// <returns></returns>
        public async Task<List<ReportTableColumnModel>> GetTableColumnByDynamicReportInfoId(int dynamicReportInfoId)
        {
            var query = "SELECT *FROM [dbo].[Report_TableColumn] WITH(NOLOCK) WHERE DynamicReportInfoId = @DynamicReportInfoId ORDER BY DisplayOrder";

            return await _db.LoadDataUsingQuery<ReportTableColumnModel, dynamic>(query, new {
                DynamicReportInfoId = dynamicReportInfoId
            });
        }

        #endregion

        #region Get Reports
        #region Dynamic Columns Directory
        /// <summary>
        /// Get Main Products with pagaing as Directionary
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IDictionary<string, object>>> GetRecordsDirectoryByFilterWithPaging(DynamicReportFilter filter)
        {
            if (string.IsNullOrWhiteSpace(filter.SortColumn))
                filter.SortColumn = "[GPcode]";

            if (string.IsNullOrWhiteSpace(filter.SortDirection))
                filter.SortDirection = "ASC";

            try
            {
                var queryString = String.Format("dbo.BS_SP_PIM_MainProduct_GetByFilter '{0}','{1}',{2},{3},'{4}','{5}'",
              filter.Where,
              filter.IsCalculateTotal.ToString().ToLower(),
              filter.Skip,
              filter.Top,
              filter.SortColumn,
              filter.SortDirection
              );

                var filteredList = await _db.LoadDictionaryDataUsingQuery<dynamic>(queryString,
                    new
                    {
                    });

                //var filteredList = await _db.LoadDataUsingProcedure<MainProduct, dynamic>(storedProcedure: "dbo.BS_SP_PIM_MainProduct_GetByFilter",
                //    new
                //    {
                //        Where = filter.Where,
                //        IsCalculateTotal = filter.IsCalculateTotal,
                //        PageNumber = filter.PageNumber,
                //        PageSize = filter.PageSize,
                //        SortColumn = filter.SortColumn,
                //        SortDirection = filter.SortDirection
                //    }, _connString);

                if (filteredList.Any() && filter.IsCalculateTotal)
                {
                    var firstItem = filteredList.FirstOrDefault();
                    filter.TotalCount = Convert.ToInt32(firstItem["TotalCount"]);
                }
                else if (!filteredList.Any() && filter.IsCalculateTotal)
                {
                    filter.TotalCount = 0;
                }

                return filteredList;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return null;
        }



        /// <summary>
        /// Get Main Products with pagaing as Directionary
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IDictionary<string, object>>> GetRecordsDirectoryByFilterWithoutPaging(DynamicReportFilter filter)
        {
            try
            {
                if (filter.SqlType == (byte)DynamicReportSqlType.Procedure)
                {
                    var filteredList1 = await _db.LoadDictionaryDataUsingProcedure<dynamic>(filter.SqlQuery,
                    new
                    {
                    });

                    if (filteredList1.Any())
                    {
                        var firstItem = filteredList1.FirstOrDefault();
                        filter.TotalCount = filteredList1.Count();
                    }
                    else if (!filteredList1.Any() && filter.IsCalculateTotal)
                    {
                        filter.TotalCount = 0;
                    }

                    return filteredList1;
                }

                //For Query
                filter.Where = filter.Where.Replace("''","'");

                var queryString = filter.SqlQuery;

                if (!string.IsNullOrWhiteSpace(filter.Where))
                {
                    if (filter.SqlQuery.Contains("{NewWhere}"))
                    {
                        queryString = queryString.Replace("{NewWhere}", $" WHERE {filter.Where}");
                    }
                    else if (filter.SqlQuery.Contains("{AppendWhere}"))
                    {
                        queryString = filter.SqlQuery.Replace("{AppendWhere}", $" AND ({filter.Where})");
                    }
                }
                else
                {
                    queryString = queryString.Replace("{NewWhere}", "");
                    queryString = filter.SqlQuery.Replace("{AppendWhere}", "");
                }

                if (!string.IsNullOrWhiteSpace(filter.SortColumn))
                {
                    if (string.IsNullOrWhiteSpace(filter.SortDirection))
                        filter.SortDirection = "ASC";

                    queryString = queryString.Replace("{OrderBy}", $" ORDER BY {filter.SortColumn} {filter.SortDirection}");
                }
                else
                {
                    queryString = queryString.Replace("{OrderBy}", "");
                }

                var filteredList = await _db.LoadDictionaryDataUsingQuery<dynamic>(queryString,
                    new
                    {
                    });

                //var filteredList = await _db.LoadDataUsingProcedure<MainProduct, dynamic>(storedProcedure: "dbo.BS_SP_PIM_MainProduct_GetByFilter",
                //    new
                //    {
                //        Where = filter.Where,
                //        IsCalculateTotal = filter.IsCalculateTotal,
                //        PageNumber = filter.PageNumber,
                //        PageSize = filter.PageSize,
                //        SortColumn = filter.SortColumn,
                //        SortDirection = filter.SortDirection
                //    }, _connString);

                if (filteredList.Any())
                {
                    var firstItem = filteredList.FirstOrDefault();
                    filter.TotalCount = filteredList.Count();
                }
                else if (!filteredList.Any() && filter.IsCalculateTotal)
                {
                    filter.TotalCount = 0;
                }

                return filteredList;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return null;
        }


        /// <summary>
        /// Get Directory Using Dynamic Query By Filter With Paging
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IDictionary<string, object>>> GetDirectoryUsingDynamicQueryByFilterWithPaging(DynamicReportFilter filter)
        {
            //if (string.IsNullOrWhiteSpace(filter.SortColumn))
            //    filter.SortColumn = "BS_MAIN.[GPcode]";

            //if (string.IsNullOrWhiteSpace(filter.SortDirection))
            //    filter.SortDirection = "ASC";

            try
            {
                var queryString = String.Format("dbo.BS_SP_PIM_CommonReportWihPaging_GetByFilter '{0}','{1}',{2},{3},'{4}','{5}','{6}','{7}','{8}'",
                  filter.Where,
                  filter.IsCalculateTotal.ToString().ToLower(),
                  filter.Skip,
                  filter.Top,
                  filter.SortColumn,
                  filter.SortDirection,
                  filter.SelectedColumns,
                  filter.ExtraJoin,
                  filter.MainQuery
                  );

                var filteredList = await _db.LoadDictionaryDataUsingQuery<dynamic>(queryString,
                    new
                    {
                    });


                if (filteredList.Any() && filter.IsCalculateTotal)
                {
                    var firstItem = filteredList.FirstOrDefault();
                    string totalCountObj = firstItem["TotalCount"].ToString();
                    filter.TotalCount = Convert.ToInt32(totalCountObj);
                }
                else if (!filteredList.Any() && filter.IsCalculateTotal)
                {
                    filter.TotalCount = 0;
                }

                return filteredList;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return null;
        }

        /// <summary>
        /// Get Directory Using Dynamic Query By Filter Without Paging
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IDictionary<string, object>>> GetDirectoryUsingDynamicQueryByFilterWithoutPaging(DynamicReportFilter filter)
        {
            //if (string.IsNullOrWhiteSpace(filter.SortColumn))
            //    filter.SortColumn = "BS_MAIN.[GPcode]";

            //if (string.IsNullOrWhiteSpace(filter.SortDirection))
            //    filter.SortDirection = "ASC";

            try
            {
                var queryString = String.Format("dbo.BS_SP_PIM_CommonReportWithoutPaging_GetByFilter '{0}','{1}','{2}','{3}','{4}','{5}'",
                  filter.Where,
                  filter.SortColumn,
                  filter.SortDirection,
                  filter.SelectedColumns,
                  filter.ExtraJoin,
                  filter.MainQuery
                  );

                return await _db.LoadDictionaryDataUsingQuery<dynamic>(queryString,
                    new
                    {
                    });

            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return null;
        }

        #endregion



        #endregion

        #region Manage Join 
        /// <summary>
        /// Get Join Info by DynamicReportInfoId
        /// </summary>
        /// <param name="dynamicReportInfoId"></param>
        /// <returns></returns>
        public async Task<List<ReportJoinInfoModel>> GetJoinInfosByDynamicReportInfoId(int dynamicReportInfoId)
        {
            var query = "SELECT *FROM [dbo].[Report_JoinInfo] WITH(NOLOCK) WHERE DynamicReportInfoId = @DynamicReportInfoId ORDER BY DisplayOrder";

            return await _db.LoadDataUsingQuery<ReportJoinInfoModel, dynamic>(query, new
            {
                DynamicReportInfoId = dynamicReportInfoId
            });
        }
        #endregion
    }
}
