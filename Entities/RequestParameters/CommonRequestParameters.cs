namespace Entities.RequestParameters;

public class CommonRequestParameters : RequestParameters
{
    // başka özelliklerde olabilir
    public int PageNumber { get; set; }
    public int Pagesize { get; set; }

    public CommonRequestParameters() : this(1,15)
    {
        
    }
    
    public CommonRequestParameters(int pageNumber=1,int pagesize=15)
    {
        PageNumber = pageNumber;
        Pagesize = pagesize;
    }
}