using Amazon;
using Amazon.Inspector;
using Amazon.Inspector.Model;
using Amazon.Runtime;

namespace CloudOps.Inspector
{
    public class ListAssessmentTargetsOperation : Operation
    {
        public override string Name => "ListAssessmentTargets";

        public override string Description => "Lists the ARNs of the assessment targets within this AWS account. For more information about assessment targets, see Amazon Inspector Assessment Targets.";
 
        public override string RequestURI => "/";

        public override string Method => "POST";

        public override string ServiceName => "Inspector";

        public override string ServiceID => "Inspector";

        public override void Invoke(AWSCredentials creds, RegionEndpoint region, int maxItems)
        {
            AmazonInspectorConfig config = new AmazonInspectorConfig();
            config.RegionEndpoint = region;
            ConfigureClient(config);            
            AmazonInspectorClient client = new AmazonInspectorClient(creds, config);
            
            ListAssessmentTargetsResponse resp = new ListAssessmentTargetsResponse();
            do
            {
                ListAssessmentTargetsRequest req = new ListAssessmentTargetsRequest
                {
                    NextToken = resp.NextToken
                    ,
                    MaxResults = maxItems
                                        
                };

                resp = client.ListAssessmentTargets(req);
                CheckError(resp.HttpStatusCode, "200");                
                
                foreach (var obj in resp.AssessmentTargetArns)
                {
                    AddObject(obj);
                }
                
            }
            while (!string.IsNullOrEmpty(resp.NextToken));
        }
    }
}