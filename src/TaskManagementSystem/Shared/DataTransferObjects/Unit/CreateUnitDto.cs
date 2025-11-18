namespace Shared.DataTransferObjects.Unit;

public record class CreateUnitDto
{
    public string Name { get; set; }
    public string UnitHeadName { get; set; }
}
