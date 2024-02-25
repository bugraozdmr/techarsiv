namespace Entities.RequestParameters;

public class SubjectRequestParameters : RequestParameters
{
    // başka özelliklerde olabilir
    public int PageNumber { get; set; }
    public int Pagesize { get; set; }

    public SubjectRequestParameters() : this(1,15)
    {
        
    }
    
    public SubjectRequestParameters(int pageNumber=1,int pagesize=15)
    {
        PageNumber = pageNumber;
        Pagesize = pagesize;
    }
}