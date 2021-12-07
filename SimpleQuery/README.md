# Simple GraphQL Query

This sample is a demo for performing a GraphQL query to the Visma Business Cloud API using C#.

This application is described in the [Visma Business Cloud API docs](https://business.visma.net/apidocs/docs/exploring-api/code).

## Description

Performs the following requests:
```
query read_glas($cid : Int, $pagesize : Int){
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
}
```

Required inputs:
- access token (not described here how to retrieve one; check the MvcCode sample)
- Visma.net company ID
