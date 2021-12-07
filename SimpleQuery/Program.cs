using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SimpleQuery
{
   class QueryRequest
   {
      [JsonPropertyName("query")]
      public string query { get; set; }

      [JsonPropertyName("variables")]
      public Dictionary<string, object> variables { get; set; }
   }

   public class GeneralLedgerAccountResponse
   {
      public Data data { get; set; }
   }

   public class Data
   {
      public Usecompany useCompany { get; set; }
   }

   public class Usecompany
   {
      public GeneralLedgerAccountConnection generalLedgerAccount { get; set; }
   }

   public class GeneralLedgerAccountConnection
   {
      public int totalCount { get; set; }
      public PageInfo pageInfo { get; set; }
      public GeneralLedgerAccount[] items { get; set; }
   }

   public class PageInfo
   {
      public bool hasNextPage { get; set; }
      public bool hasPreviousPage { get; set; }
      public string startCursor { get; set; }
      public string endCursor { get; set; }
   }

   public class GeneralLedgerAccount
   {
      public int accountNo { get; set; }
      public string name { get; set; }
   }

   class Program
   {
      private static readonly HttpClient _client = new();

      private static async Task<TResponse> ExecuteQuery<TResponse>(QueryRequest request,
                                                                   string url,
                                                                   string accessToken)
      {
         try
         {
            _client.DefaultRequestHeaders.Authorization =
               new AuthenticationHeaderValue("Bearer", accessToken);

            using var response = await _client.PostAsJsonAsync(url, request);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();
            var content = await response.Content.ReadFromJsonAsync<TResponse>();
            return content;
         }
         catch (HttpRequestException ex)
         {
            Console.Error.WriteLine($"{ex.Message} (code: {ex.StatusCode.Value})");
         }
         catch (Exception ex)
         {
            Console.Error.WriteLine(ex.Message);
         }

         return default;
      }

      static async Task Main(string[] args)
      {
         var url = "https://business.visma.net/api/graphql";
         var accessToken = "...";   // fill in an access token
         var companyId = 0;         // fill in a Visma.net company ID
         var pageSize = 100;

         var request = new QueryRequest()
         {
            query =
   @"query read_glas($cid : Int, $pagesize : Int){
 useCompany(no: $cid) {
     generalLedgerAccount(first: $pagesize) {
         totalCount
         pageInfo {
             hasNextPage
             hasPreviousPage
             startCursor
             endCursor
         }
         items {
             accountNo
             name
         }
     }
 }
}",
            variables = new Dictionary<string, object>
         {
            {"cid", companyId},
            {"$pagesize", pageSize}
         }
         };

         var result = await ExecuteQuery<GeneralLedgerAccountResponse>(request, url, accessToken);
         if (result?.data?.useCompany?.generalLedgerAccount?.items is object)
         {
            foreach (var gla in result.data.useCompany.generalLedgerAccount.items)
            {
               Console.WriteLine($"{gla.accountNo} - {gla.name}");
            }
         }
      }
   }
}
