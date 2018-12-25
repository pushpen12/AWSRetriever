using Amazon;
using Amazon.ElasticsearchService;
using Amazon.ElasticsearchService.Model;
using Amazon.Runtime;

namespace CloudOps.Operations
{
    public class ElasticsearchServiceDescribeReservedElasticsearchInstanceOfferingsOperation : Operation
    {
        public override string Name => "DescribeReservedElasticsearchInstanceOfferings";

        public override string Description => "Lists available reserved Elasticsearch instance offerings.";
 
        public override string RequestURI => "/2015-01-01/es/reservedInstanceOfferings";

        public override string Method => "GET";

        public override string ServiceName => "ElasticsearchService";

        public override string ServiceID => "Elasticsearch Service";

        public override void Invoke(AWSCredentials creds, RegionEndpoint region, int maxItems)
        {
            AmazonElasticsearchServiceClient client = new AmazonElasticsearchServiceClient(creds, region);
            DescribeReservedElasticsearchInstanceOfferingsResponse resp = new DescribeReservedElasticsearchInstanceOfferingsResponse();
            do
            {
                DescribeReservedElasticsearchInstanceOfferingsRequest req = new DescribeReservedElasticsearchInstanceOfferingsRequest
                {
                    NextToken = resp.NextToken
                    ,
                    MaxResults = maxItems
                                        
                };

                resp = client.DescribeReservedElasticsearchInstanceOfferings(req);
                CheckError(resp.HttpStatusCode, "200");                
                
                foreach (var obj in resp.ReservedElasticsearchInstanceOfferings)
                {
                    AddObject(obj);
                }
                
            }
            while (!string.IsNullOrEmpty(resp.NextToken));
        }
    }
}