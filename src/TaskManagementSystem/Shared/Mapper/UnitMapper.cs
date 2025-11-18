using Entities.Models;
using Shared.DataTransferObjects.Unit;

namespace Shared.Mapper;

public static class UnitMapper
{
    public static Unit ToEntity(this CreateUnitDto createUnitDto)
    {
        return new Unit()
        {
            CreatedBy = "",
            Name = createUnitDto.Name,
            UnitHeadName = createUnitDto.UnitHeadName
        };
    }

    public static UnitDto ToDto(this Unit unit)
    {
        return new UnitDto()
        {
            Name = unit.NormalizedName,
            UnitHeadName= unit.UnitHeadName,
            Id = unit.Id
        };
    }
}
