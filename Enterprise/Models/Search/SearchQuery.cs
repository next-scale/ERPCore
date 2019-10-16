using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ERPCore.Enterprise.Models.Search
{

    public class SearchParameter
    {
        public string FullParameter { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public SearchParameter(string parameterString)
        {
            if (parameterString == null)
                return;
            this.FullParameter = parameterString.Trim();
            this.Key = parameterString.Trim();

            if (this.Key.Length > 2)
            {
                this.Key = this.FullParameter.Substring(0, 2);
                this.Value = this.FullParameter.Substring(2, this.FullParameter.Length - 2);
            }
        }
    }

    public class SearchModel
    {
        public string FullSearchString { get; set; }

        public string SearchType { get; set; }
        public string SearchString { get; set; }
        public List<string> ParameterStrList
        {
            get
            {
                if (this.SearchString == null)
                    return null;
                else
                    return this.SearchString?.Split(' ')?.ToList();
            }
        }

        public List<SearchParameter> Parameters { get; private set; }

        public SearchModel(string searchStr)
        {
            int prefixLength = 3;

            if (searchStr == null)
                return;
            
            this.Parameters = new List<SearchParameter>();

            this.FullSearchString = searchStr;

            if (searchStr.Length >= prefixLength)
            {
                this.SearchType = searchStr.Substring(0, prefixLength).ToLower();
                this.SearchString = searchStr.Substring(prefixLength, searchStr.Length - prefixLength);
                this.SearchString = this.SearchString?.Replace("  ", " ")?.Trim();
            }

            this.ParameterProcess();
        }

        private bool ParameterProcess()
        {

            if (ParameterStrList == null || ParameterStrList.Count == 0)
                return false;


            this.ParameterStrList.ForEach(parameterString =>
            {
                var parameter = new SearchParameter(parameterString);
                this.Parameters.Add(parameter);
            });

            return true;
        }


    }
}
