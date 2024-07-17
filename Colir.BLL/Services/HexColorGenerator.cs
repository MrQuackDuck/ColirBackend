using Colir.BLL.Interfaces;
using DAL.Interfaces;

namespace Colir.BLL.Services;

public class HexColorGenerator : IHexColorGenerator
{
    private readonly IUnitOfWork _unitOfWork;
    
    public HexColorGenerator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public long GetUniqueHexColor()
    {
        throw new NotImplementedException();
    }

    public List<long> GetUniqueHexColorsList(int count)
    {
        throw new NotImplementedException();
    }
}