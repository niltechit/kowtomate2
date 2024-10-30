using CutOutWiz.Core;
using CutOutWiz.Services.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CutOutWiz.Core.Utilities.Enums;

namespace CutOutWiz.Services.AutomationAppServices.OrderItemCategorySetByAutomation
{
    public class CategorySetService: ICategorySetService
    {
    
        private readonly IClientCategoryBaseRuleService _clientCategoryBaseRuleService;
        public CategorySetService(IClientCategoryBaseRuleService clientCategoryBaseRuleService)
        {
            _clientCategoryBaseRuleService = clientCategoryBaseRuleService;
        }
        /// <summary>
        /// Here detect a file category using ftp full file path and company name
        /// order by companyrules by execution order. we do it due to priority rule on base of this.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public async Task<int> DetectOrderItemCategory(string filePath,int companyId)
        {
            if (filePath == null || companyId == 0) 
            { 
                return 0;
            }

            var companyBaseRules = await _clientCategoryBaseRuleService.ClientCategoryRuleGetByCompanyId(companyId);

            if (companyBaseRules == null || !companyBaseRules.Any())
            {
                return 0;
            }


            foreach (var companyBaseRule in companyBaseRules)
            {
                var rules = await _clientCategoryBaseRuleService.GetCategoryRuleByCategorygBaseRuleId(companyBaseRule.Id);

                if (rules == null || !rules.Any())
                {
                    continue;
                }

                rules = rules.OrderBy(r => r.ExecutionOrder).ToList();

                bool isCategoryFound = false;

                foreach (var rule in rules)
                {
                   isCategoryFound = await CheckCategoryFromPath(filePath, (int)rule.Label, (int)rule.FilterType, rule.Indicator);

                    if (!isCategoryFound)
                    {
                        break;
                    }
                }

                if (isCategoryFound)
                {
                   return companyBaseRule.ClientCategoryId;
                }
            }

            return 0;  
        }

        /// <summary>
        /// At first split full file path . Then remove first index "" value. 
        /// Remove this to main label properly.
        /// Then take decision by a switch case
        /// </summary>
        /// <param name="path"></param>
        /// <param name="label"></param>
        /// <param name="filterType"></param>
        /// <param name="indicator"></param>
        /// <returns></returns>
        private async Task<bool> CheckCategoryFromPath(string path,int label,int filterType,string indicator)
        {
            await Task.Yield();
            path = path.TrimStart('/');
            var pathWords = path.Split('/').ToList();

            //For default no check need file Path
            if(label == (int)ItemCategoryLabelType.SetDefault)
            {
                return true;
            }

            if (label == (int)ItemCategoryLabelType.CheckOnFullPathWithFileName)
            {
                return GetMatchedResult(path, filterType, indicator);
            }

            if (label == (int)ItemCategoryLabelType.CheckOnOnlyFileNameWithExtension)
            {
                var fileName = Path.GetFileName(path);
                return GetMatchedResult(fileName, filterType, indicator);
            }

            if (label == (int)ItemCategoryLabelType.CheckOnOnlyFileNameWithoutExtension)
            {
                var fileName = Path.GetFileNameWithoutExtension(path);
                return GetMatchedResult(fileName, filterType, indicator);
            }

            if (label == (int)ItemCategoryLabelType.CheckOnOnlyFileNameWithoutExtension)
            {
                var diretoryName = Path.GetDirectoryName(path);
                return GetMatchedResult(diretoryName, filterType, indicator);
            }

            //Check others label (0-10)
            if (pathWords.Count >= label-1)
            {
                var targetWord = pathWords[label-1];
                return GetMatchedResult(targetWord, filterType, indicator);
            }
            else
            {
                return false;
            }
        }

        private static bool GetMatchedResult(string path, int filterType, string indicator)
        {
            switch ((ItemCategoryFilterType)filterType)
            {
                case ItemCategoryFilterType.Contains:
                    return path.Contains(indicator);

                case ItemCategoryFilterType.StartWith:
                    return path.StartsWith(indicator);
                case ItemCategoryFilterType.EndWith:
                    return path.EndsWith(indicator);
                case ItemCategoryFilterType.Equals:
                    return path.Equals(indicator);
                default:
                    return false;
            }
        }
    }
}
