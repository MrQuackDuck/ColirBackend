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
    
    public async Task<int> GetUniqueHexColor()
    {
        var random = new Random();

        int hex;
        
        do hex = random.Next(0, 16_777_216);
        while (await _unitOfWork.UserRepository.ExistsAsync(hex));

        return hex;
    }

    public async Task<List<int>> GetUniqueHexColorsList(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
        
        var result = new List<int>();

        for (int i = 0; i < count; i++)
        {
            result.Add(await GetUniqueHexColor());
        }

        return result;
    }
}