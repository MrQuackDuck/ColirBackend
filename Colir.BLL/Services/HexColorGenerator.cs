using Colir.BLL.Interfaces;
using DAL.Interfaces;

namespace Colir.BLL.Services;

public class HexColorGenerator : IHexColorGenerator
{
    private IUnitOfWork _unitOfWork;
    
    public HexColorGenerator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public string GetUniqueHexColor()
    {
        throw new NotImplementedException();
    }

    public List<string> GetUniqueHexColorsList(int count)
    {
        throw new NotImplementedException();
    }
}